using UnityEngine;

public class GameData : MonoBehaviour
{
	[Header("Player Settings")]
	[SerializeField] float _playerJumpHeight;
	[SerializeField] float _playerMoveSpeed;

	public float playerJumpHeight { get { return _playerJumpHeight; } }
	public float playerMoveSpeed { get { return _playerMoveSpeed; } }

	public static GameData Instance;

	private void Start()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(Instance); Instance = this; }
	}
}
