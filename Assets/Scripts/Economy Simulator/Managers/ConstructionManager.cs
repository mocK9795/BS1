using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class ConstructionManager : BaseManager
{
	public static ConstructionManager Instance = null;
	public NetworkList<Construction> contructedObjects = new();
	public string modelPrefabPath { get { return _modelPrefabPath; } }
	[SerializeField] string _modelPrefabPath;
	GameObject[] _prefabs;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		if (IsServer) _prefabs = Resources.LoadAll<GameObject>(modelPrefabPath);
	}

	private void Start()
	{
		if (Instance != null) { Destroy(Instance); Instance = this; }
		else Instance = this;
	}

	public override void Tick() 
	{
		if (contructedObjects.Count == 0) return;
		
		Construction contructedObj = contructedObjects[0];
		string contructedObjName = contructedObj.name.ToString();
		contructedObjects.RemoveAt(0);

		GameObject prefab = Find(contructedObjName);
		if (prefab == null) return;
		
		var instantiatedObj = Instantiate(prefab);
		instantiatedObj.transform.position = contructedObj.position;
		instantiatedObj.transform.up = contructedObj.up;
		instantiatedObj.GetComponent<NetworkObject>().Spawn();
	}

	[ServerRpc(RequireOwnership = false)]
	public void AddContructionServerRpc(Construction item)
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

[Serializable]
public struct Construction : INetworkSerializable, IEquatable<Construction>
{
	public FixedString64Bytes name;
	public Vector3 position;
	public Vector3 up; // The transform.up of the object, which can be the raycast normal

	public Construction(FixedString64Bytes name, Vector3 position, Vector3 up) : this()
	{
		this.name = name;
		this.position = position;
		this.up = up;
	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref name);
		serializer.SerializeValue(ref position);
		serializer.SerializeValue(ref up);
	}

	public bool Equals(Construction other)
	{
		return name.Equals(other.name) &&
			   position.Equals(other.position) &&
			   up.Equals(other.up);
	}

	public override bool Equals(object obj)
	{
		if (obj is Construction other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + name.GetHashCode();
			hash = hash * 31 + position.GetHashCode();
			hash = hash * 31 + up.GetHashCode();
			return hash;
		}
	}
}