using System;
using System.Collections.Generic;
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
	
	protected PEBObject FindEnemyTarget(float range)
	{
		Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, range);
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

		if (validObject.Count == 0) return null;
		int aim = UnityEngine.Random.Range(0, validObject.Count - 1);
		return validObject[aim];
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