using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inspector : MonoBehaviour
{
    public static Inspector Instance = null;
    [SerializeField] GameObject _popupPrefab;
    [SerializeField] RectTransform _popupGroup;
    public enum InspectionLayout {LayoutGroup, AroundMouse}
    public InspectionLayout layout;
    [SerializeField] bool _utilizeWorldCanvas;

    [SerializeField] Vector2 _popupSize;

    [Header("Around Mouse")]
    [SerializeField] float _radius;

    List<RectTransform> _activeInspectionUI;
    GameObject _activeInspectionObject = null;

	private void Start()
	{
		if (Instance) {Destroy(Instance); Instance = this;}
        else Instance = this;

        _activeInspectionUI = new List<RectTransform>();
	}

	public void Inspect(GameObject inspectedObject)
    {
        if (_activeInspectionObject &&
            _activeInspectionObject.GetInstanceID() == inspectedObject.GetInstanceID()) {
            if (layout == InspectionLayout.AroundMouse) PositionUIAroundMouse(); return;
        }
        ClearInspection();
        _activeInspectionObject = inspectedObject;
        Inspectable[] allInspectableObject = inspectedObject.GetComponentsInChildren<Inspectable>();

        foreach (var inspectable in allInspectableObject)
        {
            InspectionData data = inspectable.GetInspectableData();
            string message = data.data;

            GameObject inspectionUI = Instantiate(_popupPrefab,
                (_utilizeWorldCanvas) 
                ? GameData.Instance.worldCanvasTransform : GameData.Instance.canvasTransform);

            inspectionUI.GetComponentInChildren<TMP_Text>().text = message;
            RectTransform inspectionUITransform = inspectionUI.GetComponent<RectTransform>();
			_activeInspectionUI.Add(inspectionUITransform);

            if (layout == InspectionLayout.LayoutGroup)
            inspectionUI.transform.SetParent(_popupGroup, false);

			SetUISize(inspectionUITransform, data.aspectRatio);
		}

		if (layout == InspectionLayout.AroundMouse) PositionUIAroundMouse();
        if (_utilizeWorldCanvas) SetWorldCanvasData(inspectedObject);
	}


	void PositionUIAroundMouse()
    {
		int elementCount = _activeInspectionUI.Count;
		Vector2 canvasMousePosition = PlayerInputManager.Instance.mousePosition -
	        (GameData.Instance.canvasTransform.sizeDelta * .5f);
		for (int i = 0; i < elementCount; i++)
		{
			float angle = i * Mathf.PI * 2f / elementCount; // Evenly distribute around a circle
			Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _radius;

			_activeInspectionUI[i].anchoredPosition = offset + canvasMousePosition;
		}
	}
    void SetUISize(RectTransform inspectionUI, Vector2 aspectRatio)
    {
        inspectionUI.sizeDelta = _popupSize * aspectRatio;
    }
    void SetWorldCanvasData(GameObject inspectedObject)
    {
		GameData.Instance.worldCanvasTransform.position = inspectedObject.transform.position;
		GameData.Instance.worldCanvasTransform.LookAt(PlayerInputManager.Instance.localPlayerCamera.transform);
		GameData.Instance.worldCanvasTransform.sizeDelta =
			inspectedObject.GetComponent<Collider>().bounds.size;
	}
    public void ClearInspection()
    {
        foreach (var inspectionUI in _activeInspectionUI)
        {
            Destroy(inspectionUI.gameObject);
        }

        _activeInspectionObject = null;
        _activeInspectionUI.Clear();
    }
}
