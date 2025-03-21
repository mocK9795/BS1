using UnityEngine;
using System.Collections.Generic;
using System;

public class TabManager : MonoBehaviour
{
    public static TabManager Instance = null;
	public Action<Player.PlayerInteraction> onTabSelectionChange;
    [SerializeField] List<TabButton> _selectables;

	private void Start()
	{
		if (Instance != null) Destroy(Instance);
		Instance = this;
	}

	public void OnTabSelect(TabButton selectedTab) 
	{
		foreach (var tab in _selectables)
		{if (tab.panel != null) tab.panel.SetActive(false);}
		if (selectedTab.panel != null) selectedTab.panel.SetActive(true);

		onTabSelectionChange?.Invoke(selectedTab.interaction);
	}
}
