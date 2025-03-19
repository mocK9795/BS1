using UnityEngine;

public class GameData : MonoBehaviour
{
	[Header("Player Settings")]
	[SerializeField] float _playerJumpHeight;
	[SerializeField] float _playerMoveSpeed;
	[SerializeField] Transform _objectTransform;

	public float playerJumpHeight { get { return _playerJumpHeight; } }
	public float playerMoveSpeed { get { return _playerMoveSpeed; } }

	public Transform objectTransform { get { return _objectTransform; } }

	public static GameData Instance;

	private void Start()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(Instance); Instance = this; }
	}
}
