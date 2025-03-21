using UnityEngine;

public class GameData : MonoBehaviour
{
	[Header("Player Settings")]
	[SerializeField] float _playerZoomSpeedBonus;
	[SerializeField] float _playerMoveSpeed;
	[SerializeField] float _playerZoomSpeed;
	[SerializeField] Transform _objectTransform;
	[SerializeField] RectTransform _canvasTransform;
	[SerializeField] RectTransform _worldCanvasTransform;

	public float playerZoomSpeed { get { return _playerZoomSpeed; } }
	public float playerZoomSpeedBonus { get { return _playerZoomSpeedBonus; } }
	public float playerMoveSpeed { get { return _playerMoveSpeed; } }
	public RectTransform canvasTransform { get { return _canvasTransform; } }
	public RectTransform worldCanvasTransform {  get { return _worldCanvasTransform; } }

	public Transform objectTransform { get { return _objectTransform; } }

	public static GameData Instance;

	private void Start()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(Instance); Instance = this; }
	}
}
