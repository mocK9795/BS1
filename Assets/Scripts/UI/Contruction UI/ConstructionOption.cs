using UnityEngine;
using UnityEngine.UI;

public class ConstructionOption : MonoBehaviour
{
    public Sprite contructionSprite;
    public string contructionPrefab;
    public Button selectButton;
	public Image image;

	private void Start()
	{
		selectButton.onClick.AddListener(OnSelect);
		image.sprite = contructionSprite;
	}

	void OnSelect()
	{
		ConstructionOptionsManager.Instance.OnContructionOptionSelect(contructionPrefab);
	}
}
