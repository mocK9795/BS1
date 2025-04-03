using System;
using UnityEngine;

public class BattleModeUI : MonoBehaviour
{
    public Action<Player.BattleModeInteraction> onBattleModeInteractionChange;
    public static BattleModeUI Instance;

	private void Start()
	{
		if (Instance) Destroy(Instance);
		Instance = this;
	}

	public void OptionSelected(Player.BattleModeInteraction value) 
    {
        onBattleModeInteractionChange.Invoke(value);
    }
}
