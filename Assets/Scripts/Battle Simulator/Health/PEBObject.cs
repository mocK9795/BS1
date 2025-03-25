using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PEBObject : Health, Inspectable
{
    [SerializeField] string _nation;
    [HideInInspector] public NetworkVariable<FixedString32Bytes> nation = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer) nation.Value = _nation;
    }

    public InspectionData GetInspectableData()
    {
        return new(
            "Nation " + nation.Value.ToString() + "\nHealth " + health.Value.ToString(),
            new(1.2f, 1.5f)
        );
    }
}
