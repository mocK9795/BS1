using Unity.Netcode;
using UnityEngine;
using GameFunctions;
using UnityEngine.Rendering;

public class Player : NetworkBehaviour
{
	[SerializeField] GameObject placementPrefab;
	[SerializeField] float maxPlaceDistance;
	enum Mode { Political, Battle, Economic }
	[SerializeField] Mode mode;

	Vector2 _movement;
	Camera _camera;
	CharacterController _controller;

	RaycastHit _raycast;

	//[Header("Economic Interactions")]
	bool isInspecting;
	GameObject initialyInspectedObject;

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
		_raycast = CastRay();

		ClearInspectionOnChange();
	}

	/// <summary>
	/// Clears inspection if the object looked at is diffrent
	/// </summary>
	void ClearInspectionOnChange()
	{
		if (isInspecting)
		{
			if (!_raycast.collider) return;
			if (_raycast.collider.gameObject.GetInstanceID() 
				!= initialyInspectedObject.GetInstanceID()) 
				return;

			Inspector.Instance.ClearInspection();
			isInspecting = false;
		}
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
		if (_raycast.collider == null) return;
		GameObject inspectedObject = _raycast.collider.gameObject;

		if (inspectedObject.GetComponentsInChildren<Inspectable>().Length == 0) 
			inspectedObject = inspectedObject.transform.parent.gameObject;

		Inspector.Instance.Inspect(inspectedObject);
		initialyInspectedObject = inspectedObject;
		isInspecting = true;
	}

	void OnPlayerBuild()
	{
		if (_raycast.collider == null) return;

		GameObject instantiatedObj = Instantiate(placementPrefab, GameData.Instance.objectTransform);

		instantiatedObj.transform.position = _raycast.point;
		instantiatedObj.transform.up = _raycast.normal;
	}

	RaycastHit CastRay()
	{
		Ray ray = _camera.ScreenPointToRay(PlayerInputManager.Instance.mousePosition);
		Physics.Raycast(ray, out RaycastHit hit, maxPlaceDistance);
		return hit;
	}
}
