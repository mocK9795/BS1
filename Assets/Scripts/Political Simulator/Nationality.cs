using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Nationality : NetworkBehaviour, Inspectable
{
    [SerializeField] string nation;
    [HideInInspector] public NetworkVariable<FixedString32Bytes> net_nation;

	public override void OnNetworkSpawn()
	{
		if (!IsServer) return;
		base.OnNetworkSpawn();
		ApplyNationality();
	}

	void ApplyNationality () {
		if (net_nation == null) 
			net_nation = new NetworkVariable<FixedString32Bytes>(writePerm:NetworkVariableWritePermission.Server);
		net_nation.Value = nation;
	}

	public InspectionData GetInspectableData()
	{
		return new("Nationality " + net_nation.Value.ToString());
	}
}
