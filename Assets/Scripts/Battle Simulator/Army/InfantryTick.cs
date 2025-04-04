using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class InfantryTick : BaseManager
{
	public static InfantryTick Instance;
	public Action Logic;

	[SerializeField] GameObject defaultArmament;
	[SerializeField] string armamentPrefabPath;
	GameObject[] armamentPrefabs;

	private void Start()
	{
		if (Instance) Destroy(Instance);
		Instance = this;

		armamentPrefabs = Resources.LoadAll<GameObject>(armamentPrefabPath);
	}

	public override void Tick()
	{
		Logic?.Invoke();
	}

	[ContextMenu("Grant Default Weapon")]
	void GiftDefaultWeapon()
	{
		Infantry[] army = FindObjectsByType<Infantry>(FindObjectsSortMode.None);
		foreach (Infantry infantry in army) 
		{
			infantry.GetArmamentClientRpc(defaultArmament.name);
		}
	}

	public GameObject FindArmament(string name)
	{
		foreach (GameObject armament in armamentPrefabs) 
		{ if (armament.name == name) return armament; }
		return null;
	}
}
