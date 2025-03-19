using Unity.Netcode;
using UnityEngine;

public class Nationality : NetworkBehaviour
{
    [SerializeField] string nation;
    [HideInInspector] public NetworkVariable<string> net_nation = null;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		ApplyNationality();
	}

	void ApplyNationality () {
		if (net_nation == null) 
			net_nation = new NetworkVariable<string>(writePerm:NetworkVariableWritePermission.Server);
		net_nation.Value = nation;
	}
}
