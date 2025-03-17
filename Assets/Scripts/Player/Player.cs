using Unity.Netcode;
using UnityEngine;
using GameFunctions;

public class Player : NetworkBehaviour, IReceivePlayerJump, IReceivePlayerLook, IReceivePlayerMove
{
	Vector2 _movement;
	Camera _camera;
	CharacterController _controller;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		_camera = GetComponent<Camera>();
		_controller = GetComponent<CharacterController>();

		if (IsOwner)
		{
			PlayerInputManager.Instance.Subscribe(this as IReceivePlayerJump);
			PlayerInputManager.Instance.Subscribe(this as IReceivePlayerMove);
			PlayerInputManager.Instance.Subscribe(this as IReceivePlayerLook);
		}
		else
		{
			_camera.enabled = false;
		}
	}

	private void Update()
	{
		if (!IsOwner) return;
		_controller.Move(
			Time.deltaTime * GameData.Instance.playerMoveSpeed * 
			(_movement.y * transform.forward + _movement.x * transform.right)
		);
	}

	public void OnPlayerJump() 
	{
		if (!IsOwner) return;
	}
	public void OnPlayerLook(Vector2 value) 
	{
		if (!IsOwner) return;
		_camera.transform.rotation = Quaternion.Euler(_camera.transform.eulerAngles + Basic.vector3(value));
	}
	public void OnPlayerMove(Vector2 value) { 
		if (!IsOwner) return;
		_movement = value;
	}

}
