using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour
{
	Rigidbody _body;
	Camera _camera;

	Vector2 _movement;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		InputManager.Instance.onJump += OnJump;	
		InputManager.Instance.onMove += OnMove;
		InputManager.Instance.onLook += OnLook;

		_body = GetComponent<Rigidbody>();
		_camera = GetComponentInChildren<Camera>();

		if (!IsOwner) {
			_camera.enabled = false;
		}
	}

	private void Update()
	{
		if (!IsOwner) return;

		_body.linearVelocity = _movement * GameData.Instance.playerMoveSpeed;
	}

	void OnJump() {
		if (!IsOwner) return;
		_body.linearVelocity += Vector3.up * GameData.Instance.playerJumpHeight;
	}
	void OnMove(Vector2 value)
	{
		_movement = value;
	}
	void OnLook(Vector2 value) { _camera.transform.rotation = Quaternion.Euler(value); }
}
