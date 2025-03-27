using GameFunctions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class Infantry : Unit
{
	CharacterController _controller;
	NavMeshAgent _agent;

	public Weapon weapon { get; protected set; }
	
	Vector2 _movement = new();
	Vector3 _objective = new();



	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		_controller = GetComponentInChildren<CharacterController>();
		_agent = GetComponentInChildren<NavMeshAgent>();
		_agent.speed = GameData.Instance.infantrySpeed;
		_controller.enabled = false;
		
		InfantryTick.Instance.Logic += LocalLogic;
		SetWeapon();
	}

	private void Update()
	{
		if (!IsServer) return;
		if (_controller.enabled) ControlledMovement();
	}

	void SetWeapon() { weapon = GetComponentInChildren<Weapon>(); }


	// Controll Management
	[ServerRpc(RequireOwnership = false)]
	public void OnControllServerRpc()
	{
		InfantryTick.Instance.Logic -= LocalLogic;
		_controller.enabled = true;
		_agent.enabled = false;
	}
	[ServerRpc(RequireOwnership = false)]
	public void OnExitControllServerRpc()
	{
		InfantryTick.Instance.Logic += LocalLogic;
		_controller.enabled = false;
		_agent.enabled = true;
	}

	[ServerRpc(RequireOwnership = false)]
	public override void OnCommandServerRpc(Vector3 objective)
	{
		if (!_agent.enabled) return; 
		base.OnCommandServerRpc(objective);
		_agent.SetDestination(_objective);
	}


	// Controlled Movement
	[ServerRpc(RequireOwnership = false)]
	public void MoveServerRpc(Vector2 value)
	{
		if (!_controller.enabled) return;
		_movement = value;
	}

	[ServerRpc(RequireOwnership = false)]
	public void RotateServerRpc(float value)
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
		if (_controller.enabled || !weapon) return;

		Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, weapon.range);
		List<PEBObject> validObject = new();

		foreach (Collider obj in nearbyObjects)
		{
			PEBObject nationality = obj.GetComponent<PEBObject>();
			if (!nationality && obj.transform.parent)
				nationality = obj.transform.parent.GetComponent<PEBObject>();

			if (!nationality) continue;
			if (nationality.nation.Value.ToString() == _data.nation.Value.ToString()) continue;

			validObject.Add(nationality);
		}

		if (validObject.Count == 0) return;

		int aim = Random.Range(0, validObject.Count - 1);
		LookAtTarget(validObject[aim].transform.position);
		weapon.Damage(validObject[aim]);
	}

	IEnumerator LookAtTarget(Vector3 point)
	{
		Vector3 initialRotation = transform.eulerAngles;
		transform.LookAt(point);
		Vector3 finalRotation = new(initialRotation.x, transform.eulerAngles.y, initialRotation.z);
		transform.eulerAngles = initialRotation;

		float yAngle = initialRotation.y;
		float currentVelocity = 0;

		while (Basic.AreAnglesEqual(yAngle, finalRotation.y, 5))
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
} 
