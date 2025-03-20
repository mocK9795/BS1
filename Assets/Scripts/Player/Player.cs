using Unity.Netcode;
using UnityEngine;
using GameFunctions;
using UnityEngine.Rendering;

public class Player : NetworkBehaviour
{
	[SerializeField] GameObject placementPrefab;
	[SerializeField] float maxPlaceDistance;
	enum Mode {Political, Battle, Economic}
	[SerializeField] Mode mode;

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
			PlayerInputManager.Instance.onPlayerJump += OnPlayerJump;
			PlayerInputManager.Instance.onPlayerMove += OnPlayerMove;
			PlayerInputManager.Instance.onPlayerLook += OnPlayerLook;
			PlayerInputManager.Instance.onPlayerClick += OnPlayerInspect;
			PlayerInputManager.Instance.localPlayerCamera = _camera;
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

	void OnPlayerJump() 
	{
		if (!IsOwner) return;
	}
	void OnPlayerLook(Vector2 value) 
	{
		if (!IsOwner) return;
		_camera.transform.rotation = Quaternion.Euler(_camera.transform.eulerAngles + Basic.vector3(value));
	}
	void OnPlayerMove(Vector2 value) { 
		if (!IsOwner) return;
		_movement = value;
	}

	void OnPlayerInspect() 
	{
		RaycastHit hit = CastRay();
		if (hit.collider == null) return;
		print("Inspecting " + hit.collider.gameObject.name);
		Inspector.Instance.Inspect(hit.collider.gameObject);
	}

	void OnPlayerBuild()
	{
		RaycastHit hit = CastRay();
		if (hit.collider == null) return;

		GameObject instantiatedObj = Instantiate(placementPrefab, GameData.Instance.objectTransform);

		instantiatedObj.transform.position = hit.point;
		instantiatedObj.transform.up = hit.normal;
	}

	RaycastHit CastRay()
	{
		Ray ray = _camera.ScreenPointToRay(PlayerInputManager.Instance.mousePosition);
		Physics.Raycast(ray, out RaycastHit hit, maxPlaceDistance);
		return hit;
	}

}
