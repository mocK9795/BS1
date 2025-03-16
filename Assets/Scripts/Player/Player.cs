using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : Leader
{
	Rigidbody _body;
	Camera _camera;
	Animator _animator;

	Vector2 _movement;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		PlayerInputManager.Instance.onJump += OnJump;	
		PlayerInputManager.Instance.onMove += OnMove;
		PlayerInputManager.Instance.onLook += OnLook;

		_body = GetComponent<Rigidbody>();
		_camera = GetComponentInChildren<Camera>();
		_animator = GetComponentInChildren<Animator>();

		if (!IsOwner) {
			_camera.enabled = false;
		}
	}

	private void Update()
	{
		if (!IsOwner) return;

		_body.linearVelocity = ((transform.forward * _movement.y + transform.right * _movement.x) * GameData.Instance.playerMoveSpeed) + Vector3.up * _body.linearVelocity.y;
		if (_animator != null) {
			if (_movement.magnitude > 0.1)
				_animator.SetBool("Walk", true);

			else
				_animator.SetBool("Idle", true);
		}
	}

	void OnJump() {
		if (!IsOwner) return;
		_body.linearVelocity += Vector3.up * GameData.Instance.playerJumpHeight;
	}
	void OnMove(Vector2 value)
	{
		if (!IsOwner) return;
        _movement = value;
	}
	void OnLook(Vector2 value) {
		if (!IsOwner) return;
		_camera.transform.rotation = 
			Quaternion.Euler(new Vector3(value.x, 0) + _camera.transform.eulerAngles);
		transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, value.y));
	}
}
