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

	[ServerRpc(RequireOwnership = false)]
	public virtual void OnCommandServerRpc(Vector3 objective)
	{
	}
}