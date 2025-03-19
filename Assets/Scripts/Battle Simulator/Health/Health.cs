using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour, Inspectable
{
	public NetworkVariable<float> net_health;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		net_health = new(10, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);
	}

	public string GetInspectableData()
	{
		return "Health " + net_health.Value.ToString();
	}
}
