using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Leader : Attacker
{
	public Action<Vector3, Attacker> follow;
	NavMeshAgent agent;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		agent = GetComponent<NavMeshAgent>();
	}
}

public enum OrderLevel {Low, Basic, Important, Critical, Forced}
