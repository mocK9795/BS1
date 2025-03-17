using Unity.Netcode;
using UnityEngine;

public class Nationality : NetworkBehaviour
{
    [SerializeField] string nation;
    [HideInInspector] public NetworkVariable<string> net_nation;

	private void OnValidate()
	{
		net_nation.Value = nation;
	}
}
