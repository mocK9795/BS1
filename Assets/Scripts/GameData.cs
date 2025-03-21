using UnityEngine;

public class GameData : MonoBehaviour
{
	[Header("Player Settings")]
	public float playerZoomSpeedBonus;
	public float playerMoveSpeed;
	public float playerZoomSpeed;
	public Vector3 topdownRotation;

	[Header("attacker Settings")]
	public float attackerSpeed;
	public float gravity;

	[Header("Global Transforms")]
	public Transform objectTransform;
	public RectTransform canvasTransform;
	public RectTransform worldCanvasTransform;

	public static GameData Instance;

	private void Start()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(Instance); Instance = this; }
	}
}
