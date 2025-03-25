using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Leader : Infantry
{
	public Action<Vector3, Infantry> follow;
	NavMeshAgent agent;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		agent = GetComponent<NavMeshAgent>();
	}
}

public enum OrderLevel {Low, Basic, Important, Critical, Forced}
