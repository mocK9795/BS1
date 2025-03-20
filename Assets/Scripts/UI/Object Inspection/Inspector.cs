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

	private void Start()
	{
		if (Instance) {Destroy(Instance); Instance = this;}
        else Instance = this;

        _activeInspectionUI = new List<RectTransform>();
	}

	public void Inspect(GameObject inspectedObject)
    {
        ClearInspection();
        Inspectable[] allInspectableObject = inspectedObject.GetComponentsInChildren<Inspectable>();

        foreach (var inspectable in allInspectableObject)
        {
            string data = inspectable.GetInspectableData();
            GameObject inspectionUI = Instantiate(_popupPrefab,
                (_utilizeWorldCanvas) 
                ? GameData.Instance.worldCanvasTransform : GameData.Instance.canvasTransform);

            inspectionUI.GetComponentInChildren<TMP_Text>().text = data;
            _activeInspectionUI.Add(inspectionUI.GetComponent<RectTransform>());

            if (layout == InspectionLayout.LayoutGroup)
            inspectionUI.transform.SetParent(_popupGroup, false);
		}

        if (layout == InspectionLayout.AroundMouse) PositionUIAroundMouse();
        if (_utilizeWorldCanvas) SetWorldCanvasData(inspectedObject);
		SetUISize();

	}


	void PositionUIAroundMouse()
    {
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
				_popupGroup,
				PlayerInputManager.Instance.mousePosition,
				PlayerInputManager.Instance.localPlayerCamera,
				out Vector2 localPoint
		);

		int elementCount = _activeInspectionUI.Count;
		for (int i = 0; i < elementCount; i++)
		{
			float angle = i * Mathf.PI * 2f / elementCount; // Evenly distribute around a circle
			Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * _radius;

			_activeInspectionUI[i].anchoredPosition = localPoint + (Vector2)offset;
		}
	}
    void SetUISize()
    {
        foreach (var inspectionUI in _activeInspectionUI)
        {
            inspectionUI.sizeDelta = _popupSize;
        }
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

        _activeInspectionUI.Clear();
    }
}
