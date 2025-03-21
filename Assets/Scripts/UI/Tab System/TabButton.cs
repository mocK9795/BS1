using UnityEngine;

public class TabButton : MonoBehaviour
{
    public Player.PlayerInteraction interaction;
    public GameObject panel;

    public void OnSelect()
    {
        TabManager.Instance.OnTabSelect(this);
    }
}
