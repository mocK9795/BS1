using System;
using Unity.Netcode;
using UnityEngine;

public class Unit : NetworkBehaviour
{
	protected PEBObject _data;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		_data = GetComponentInChildren<PEBObject>();
	}
}

public interface IControllable
{
	[ServerRpc(RequireOwnership = false)]
	void ControlLookServerRpc(float value); // Only y rotation needed

	[ServerRpc(RequireOwnership = false)]
	void ControlMoveServerRpc(Vector2 value);

	[ServerRpc(RequireOwnership = false)]
	void OnControlEnterServerRpc();

	[ServerRpc(RequireOwnership = false)]
	void OnControlExitServerRpc();
	void ControlDamage(RaycastHit raycast);

	Vector3 Position();
}

public interface ICommandable
{
	[ServerRpc(RequireOwnership = false)]
	void OnCommandServerRpc(Vector3 objective);
}