using UnityEngine;
using UnityEngine.UI;

public class ContructionOption : MonoBehaviour
{
    public Sprite contructionSprite;
    public string contructionPrefab;
    public Button selectButton;

	private void Start()
	{
		selectButton.onClick.AddListener(OnSelect);
	}

	void OnSelect()
	{
		ContructionOptionsManager.Instance.OnContructionOptionSelect(contructionPrefab);
	}
}
