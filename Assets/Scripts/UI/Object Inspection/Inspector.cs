using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inspector : MonoBehaviour
{
    public static Inspector Instance = null;
    [SerializeField] GameObject popupPrefab;
    [SerializeField] Transform popupGroup;
    List<GameObject> activeInspectionUI;

	private void Start()
	{
		if (Instance) {Destroy(Instance); Instance = this;}
        else Instance = this;

        activeInspectionUI = new List<GameObject>();
	}

	public void Inspect(GameObject inspectedObject)
    {
        Inspectable[] allInspectableObject = inspectedObject.GetComponentsInChildren<Inspectable>();

        foreach (var inspectable in allInspectableObject)
        {
            string data = inspectable.GetInspectableData();
            GameObject inspectionUI = Instantiate(popupPrefab, popupGroup);
            inspectionUI.GetComponentInChildren<TMP_Text>().text = data;
            activeInspectionUI.Add(inspectionUI);

		}
    }

    public void ClearInspection()
    {
        foreach (var inspectionUI in activeInspectionUI)
        {
            Destroy(inspectionUI);
        }

        activeInspectionUI.Clear();
    }
}
