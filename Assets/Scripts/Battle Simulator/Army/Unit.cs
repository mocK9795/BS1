using Unity.Netcode;
using UnityEngine;

public class Unit : NetworkBehaviour
{
	protected PEBObject _data;
	protected Vector3 _objective;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		_data = GetComponentInChildren<PEBObject>();
	}

	[ServerRpc(RequireOwnership = false)]
	public virtual void OnCommandServerRpc(Vector3 objective)
	{
		_objective = objective;
	}
}