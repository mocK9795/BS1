using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
	NetworkVariable<float> health;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		health = new(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}
}
