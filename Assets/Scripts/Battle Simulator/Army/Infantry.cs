using GameFunctions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.Netcode.Components;
using Unity.Collections;

public class Infantry : Unit, ICommandable, IControllable
{
	[SerializeField] GameObject handBone;

	NetworkAnimator _animator;
	CharacterController _controller;
	NavMeshAgent _agent;

	public Armament activeArmament { get; protected set; }
	
	Vector2 _movement = new();
	Vector3 _objective = new();
	PEBObject _enemy = null;

	// Animation
	bool _marching;
	bool _idle;


	public Vector3 Position() { return transform.position; }


	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		_controller = GetComponentInChildren<CharacterController>();
		_animator = GetComponentInChildren<NetworkAnimator>();
		_agent = GetComponentInChildren<NavMeshAgent>();
		_agent.speed = GameData.Instance.infantrySpeed;
		_objective = transform.position;
		_controller.enabled = false;
		
		InfantryTick.Instance.Logic += LocalLogic;
		SetActiveArmament();
	}

	void SetActiveArmament(){ activeArmament = GetComponentInChildren<Armament>(); }

	private void Update()
	{
        if (!IsServer) return;
		if (_agent) StopAgentOnCompletion();
		if (_animator)	ApplyAnimations();
		if (_controller && _controller.enabled) ControlledMovement();
	}

	void ApplyAnimations()
	{
		bool marching = (_controller && _controller.enabled && _movement.magnitude > 0)
					|| (_agent && _agent.enabled && !_agent.isStopped);
		bool idle = !marching;

		// _marching for previous animation state and marching for current animation state
		if (!_marching && marching) _animator.SetTrigger("March");
		if (!_idle && idle) _animator.SetTrigger("Idle");

		_marching = marching;
		_idle = idle;
	}

	void StopAgentOnCompletion()
	{
		float distance = Vector2.Distance(Basic.vector2(transform.position), Basic.vector2(_objective));
		if (distance < GameData.Instance.navMeshAgentStopRange) _agent.isStopped = true; 
	}
		 
	// Controll Management
	[ServerRpc(RequireOwnership = false)]
	public void OnControlEnterServerRpc()
	{
		InfantryTick.Instance.Logic -= LocalLogic;
		_controller.enabled = true;
		_agent.enabled = false;
	}
	[ServerRpc(RequireOwnership = false)]
	public void OnControlExitServerRpc()
	{
		InfantryTick.Instance.Logic += LocalLogic;
		_controller.enabled = false;
		_agent.enabled = true;
	}
	[ServerRpc(RequireOwnership = false)]
	public void OnCommandServerRpc(Vector3 objective)
	{
		if (!_agent.enabled) return; 
		_objective = objective;
		_agent.isStopped = false;
		_agent.SetDestination(_objective);
	}


	// Controlled Movement
	[ServerRpc(RequireOwnership = false)]
	public void ControlMoveServerRpc(Vector2 value)
	{
		if (!_controller.enabled) return;
		_movement = value;
	}

	[ServerRpc(RequireOwnership = false)]
	public void ControlLookServerRpc(float value)
	{
		if (!_controller.enabled) return;
		transform.eulerAngles = Basic.XZPlane(transform.eulerAngles) + Vector3.up * value;
	}
	
	void ControlledMovement()
	{
		_controller.Move((_movement.y * Basic.XZPlane(transform.forward) +
				_movement.x * Basic.XZPlane(transform.right))
				* Time.deltaTime * GameData.Instance.infantrySpeed);
		_controller.Move(GameData.Instance.gravity * Vector3.down * Time.deltaTime);
	}
	
	void LocalLogic()
	{
		if (_controller.enabled || !activeArmament) return;
		if (_enemy) activeArmament.Damage(_enemy);
		if (!_enemy)
		{
			_enemy = FindEnemyTarget(activeArmament.range);
			if (_enemy) StartCoroutine(LookAtTarget(_enemy.transform.position));
		}
	}

	IEnumerator LookAtTarget(Vector3 point)
	{
		Vector3 initialRotation = transform.eulerAngles;
		transform.LookAt(point);
		Vector3 finalRotation = new(initialRotation.x, transform.eulerAngles.y, initialRotation.z);
		transform.eulerAngles = initialRotation;

		float yAngle = initialRotation.y;
		float currentVelocity = 0;

		while (!Basic.AreAnglesEqual(yAngle, finalRotation.y, 5))
		{
			yAngle = Mathf.SmoothDampAngle(
				yAngle, 
				finalRotation.y, 
				ref currentVelocity, 
				Time.deltaTime * GameData.Instance.infantryRotationSpeed);
			yield return null;
		}
	}

	public override void OnNetworkDespawn()
	{
		if (IsServer) InfantryTick.Instance.Logic -= LocalLogic;
		base.OnNetworkDespawn();
	}

	public void ControlDamage(RaycastHit raycast)
	{
		activeArmament.Damage(raycast);
	}

	public int Identity()
	{
		return gameObject.GetInstanceID();
	}


	[ClientRpc]
	public void GetArmamentClientRpc(FixedString32Bytes name)
	{
		GameObject armament = InfantryTick.Instance.FindArmament(name.ToString());
		if (!armament) { Debug.LogError("Invalid Armament " + name.ToString()); return; }
		Instantiate(armament, transform);
		SetActiveArmament();
	}
} 
