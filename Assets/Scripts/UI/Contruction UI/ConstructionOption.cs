using UnityEngine;
using UnityEngine.UI;

public class ConstructionOption : MonoBehaviour
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
		ConstructionOptionsManager.Instance.OnContructionOptionSelect(contructionPrefab);
	}
}
