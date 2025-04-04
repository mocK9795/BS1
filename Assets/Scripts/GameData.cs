using UnityEngine;

public class GameData : MonoBehaviour
{
	[Header("Player Settings")]
	public float playerZoomSpeedBonus;
	public float playerMoveSpeed;
	public float playerZoomSpeed;
	public Vector3 topdownRotation;
	[Tooltip("When the player controls a leader, this offset will be applied to the position of the camera")]
	public Vector3 leadFaceOffset;
	public float defaultPlayerY;
	public Vector2 selectionBoundsY;

	[Header("Player Orthographic Mode")]
	public float orthographicSize;
	public float orthographicYPosition;

	[Header("Infantry Settings")]
	public float navMeshAgentStopRange;
	public float infantryRotationSpeed;
	public float infantrySpeed;
	public float gravity;

	[Header("Global Transforms")]
	public Transform objectTransform;
	public RectTransform canvasTransform;
	public RectTransform worldCanvasTransform;

	[Header("UI Refrence")]
	public GameObject crossHair;

	public static GameData Instance;

	private void Start()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(Instance); Instance = this; }
	}
}

public enum Bool { True, False }