using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;

public class ContructionOptionsManager : MonoBehaviour
{
	public static ContructionOptionsManager Instance;

    [SerializeField] string modelPreviewPath;

	[Space(1)]
	[SerializeField] RectTransform optionContainer;
	[Tooltip("Must have a contruction option component")]
	[SerializeField] GameObject optionPrefab;
	public string selectedPrefab;

	private void Start()
	{
		if (Instance) {Destroy(Instance); Instance = this; }
		else { Instance = this;}

		Sprite[] contructionPreviews = Resources.LoadAll<Sprite>(modelPreviewPath);
		GameObject[] contructionPrefabs = Resources.LoadAll<GameObject>(ContructionManager.Instance.modelPrefabPath);

		if (contructionPreviews.Length != contructionPrefabs.Length) Debug.LogError("Prefabs & Previews Mismatch");

		contructionPreviews = contructionPreviews.OrderBy(obj => obj.name).ToArray();
		contructionPrefabs = contructionPrefabs.OrderBy(obj => obj.name).ToArray();

		for (int i = 0; i < contructionPreviews.Length; i++)
		{
			GameObject uiElement = Instantiate(optionPrefab, optionContainer);
			ContructionOption option = uiElement.GetComponentInChildren<ContructionOption>();
			option.contructionSprite = contructionPreviews[i];
			option.contructionPrefab = contructionPrefabs[i].name;
		}
	}

	public void OnContructionOptionSelect(string prefab)
	{
		selectedPrefab = prefab;
	}
}
