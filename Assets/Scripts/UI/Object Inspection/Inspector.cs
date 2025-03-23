using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inspector : MonoBehaviour
{
    public static Inspector Instance = null;
    [SerializeField] GameObject _popupPrefab;
    [SerializeField] GameObject _textPrefab;
    [SerializeField] GameObject _imagePrefab;
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

            GameObject inspectionUI = Instantiate(_popupPrefab,
                (_utilizeWorldCanvas) 
                ? GameData.Instance.worldCanvasTransform : GameData.Instance.canvasTransform);


            RectTransform inspectionUITransform = inspectionUI.GetComponent<RectTransform>();
			_activeInspectionUI.Add(inspectionUITransform);

            ApplyLayout(inspectionUITransform, data);

			if (layout == InspectionLayout.LayoutGroup)
            inspectionUI.transform.SetParent(_popupGroup, false);
		}

		if (layout == InspectionLayout.AroundMouse) PositionUIAroundMouse();
        if (_utilizeWorldCanvas) SetWorldCanvasData(inspectedObject);
	}


    void ApplyLayout(RectTransform UIParent, InspectionData data)
    {
		UIParent.sizeDelta = _popupSize * data.aspectRatio;
	
        if (data.layout == InspectionData.Layout.Basic) SetBasicLayout(UIParent, data);
        if (data.layout == InspectionData.Layout.Grid) SetGridLayout(UIParent, data);
        if (data.layout == InspectionData.Layout.Horizontal) SetHorizontalLayout(UIParent, data);
        if (data.layout == InspectionData.Layout.Vertical) SetVerticalLayout(UIParent, data);
    }

    void SetBasicLayout(RectTransform UIParent, InspectionData data)
    {
        AddTextObject(UIParent, data.basicTextData);
    }

    void AddTextObject(RectTransform UIParent, string data)
    {
		var textObject = Instantiate(_textPrefab, UIParent);
		textObject.GetComponent<TMP_Text>().text = data;
	}

    void AddImageObject(RectTransform UIParent, Sprite data)
    {
        var imageObject = Instantiate(_imagePrefab, UIParent);
        imageObject.GetComponent<Image>().sprite = data;
    } 

    void SetGridLayout(RectTransform UIParent, InspectionData data)
    {
        GridLayoutGroup layout = UIParent.AddComponent<GridLayoutGroup>();
        layout.cellSize = data.gridCellSize;

        SetElements(UIParent, data);
    }

    void SetVerticalLayout(RectTransform UIParent, InspectionData data)
    {
        VerticalLayoutGroup layout = UIParent.AddComponent<VerticalLayoutGroup>();
        SetElements(UIParent, data);
    }

    void SetHorizontalLayout(RectTransform UIParent, InspectionData data)
    {
        HorizontalLayoutGroup layout = UIParent.AddComponent<HorizontalLayoutGroup>();
        SetElements(UIParent, data);
    }

    void SetElements(RectTransform UIParent, InspectionData data)
    {
		foreach (var element in data.elements)
		{
			if (element.type == InspectionElement.Type.String)
			{
				AddTextObject(UIParent, element.text);
			}

			else if (element.type == InspectionElement.Type.Image)
			{
				AddImageObject(UIParent, element.sprite);
			}
		}
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
