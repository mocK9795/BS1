using GameFunctions;
using Unity.Netcode;
using UnityEngine;

public class Attacker : NetworkBehaviour
{
	CharacterController _controller;
	NetworkVariable<Vector2> _movement = new();

	[ServerRpc(RequireOwnership = false)]
	public void MoveServerRpc(Vector2 value)
	{
		_movement.Value = value;
	}

	[ServerRpc(RequireOwnership = false)]
	public void RotateServerRpc(float value)
	{
		transform.eulerAngles = Basic.XZPlane(transform.eulerAngles) + Vector3.up * value;
	}

	private void Start()
	{
		_controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		if (!IsServer) return;

		_controller.Move((_movement.Value.y * Basic.XZPlane(transform.forward) +
			_movement.Value.x * Basic.XZPlane(transform.right)) 
			* Time.deltaTime * GameData.Instance.attackerSpeed);
		_controller.Move(GameData.Instance.gravity * Vector3.down * Time.deltaTime);
	}
} 
