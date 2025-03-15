using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
	public NetworkVariable<float> health;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		health = new(10, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);
	}
}
