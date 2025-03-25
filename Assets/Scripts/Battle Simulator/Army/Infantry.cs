using GameFunctions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Infantry : NetworkBehaviour
{
	CharacterController _controller;
	NavMeshAgent _agent;
	PEBObject _data;

	public Weapon weapon { get; protected set; }
	
	Vector2 _movement = new();
	Vector3 _objective = new();



	private void Start()
	{
		_controller = GetComponentInChildren<CharacterController>();
		_agent = GetComponentInChildren<NavMeshAgent>();
		_data = GetComponentInChildren<PEBObject>();
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
		_controller.enabled = true;
		_agent.enabled = false;
	}
	[ServerRpc(RequireOwnership = false)]
	public void OnExitControllServerRpc()
	{
		_controller.enabled = false;
		_agent.enabled = true;
	}
	[ServerRpc(RequireOwnership = false)]
	public void OnCommandServerRpc(Vector3 objective)
	{
		if (!_agent.enabled) return; 
		_objective = objective;
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
		weapon.Damage(validObject[0]);
	}
} 
