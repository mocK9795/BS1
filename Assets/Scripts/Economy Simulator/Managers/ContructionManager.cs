using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Collections;
using Unity.VisualScripting;
using System;

public class ContructionManager : BaseManager
{
	public static ContructionManager Instance;
	public NetworkList<Contruction> contructedObjects;
	public string modelPrefabPath { get { return _modelPrefabPath; } }
	[SerializeField] string _modelPrefabPath;
	GameObject[] _prefabs;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		contructedObjects = new();

		if (IsServer) _prefabs = Resources.LoadAll<GameObject>(modelPrefabPath);
	}

	public override void Tick() 
	{
		if (contructedObjects.Count == 0) return;
		
		Contruction contructedObj = contructedObjects[0];
		string contructedObjName = contructedObj.name.ToString();
		contructedObjects.RemoveAt(0);

		GameObject prefab = Find(contructedObjName);
		if (prefab == null) return;
		
		Instantiate(prefab);
		prefab.transform.position = contructedObj.position;
		prefab.transform.up = contructedObj.up;
		prefab.GetComponent<NetworkObject>().Spawn();
	}

	[ServerRpc]
	public void AddContructionServerRpc(Contruction item)
	{
		contructedObjects.Add(item);
	}

	GameObject Find(string prefab)
	{
		foreach (GameObject obj in _prefabs)
		{
			if (obj.name == prefab) return obj;
		}

		return null;
	} 
}

[System.Serializable]
public struct Contruction : IEquatable<Contruction>
{
	public FixedString64Bytes name;
	public Vector3 position;
	public Vector3 up; // The transform.up of the object, which can be the raycast normal

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref name);
		serializer.SerializeValue(ref position);
		serializer.SerializeValue(ref up);
	}

	public override bool Equals(object obj)
	{
		return name == ((Contruction)obj).name && ((Contruction)obj).position == position;
	}

	public override bool Equals(Contruction obj)
	{
		return name == ((Contruction)obj).name && ((Contruction)obj).position == position;
	}

	public static bool operator ==(Contruction left, Contruction right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Contruction left, Contruction right)
	{
		return !(left == right);
	}
}
