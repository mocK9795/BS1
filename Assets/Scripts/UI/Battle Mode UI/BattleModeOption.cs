using UnityEngine;

public class BattleModeOption : MonoBehaviour
{
    public Player.BattleModeInteraction interaction;
    public void OnSelect() {BattleModeUI.Instance.OptionSelected(interaction); }
}
