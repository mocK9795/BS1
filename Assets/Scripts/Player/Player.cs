using Unity.Netcode;
using UnityEngine;
using GameFunctions;
using UnityEngine.Rendering;

public class Player : NetworkBehaviour
{
	[SerializeField] float _maxInteractionDistance;
	enum Mode { Political, Battle, Economic }
	[SerializeField] Mode mode;

	enum MovementMode { Free, Topdown }
	[SerializeField] MovementMode movementMode;

	public enum PlayerInteraction { InspectMode, BuildMode, Lead }
	public PlayerInteraction interactionMode;

	Vector2 _movement;
	Vector2 _look;

	Camera _camera;
	CharacterController _controller;

	RaycastHit _raycast;

	bool isInspecting;
	GameObject initialyInspectedObject;

	bool isLeading;
	GameObject leader;

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
			PlayerInputManager.Instance.onPlayerClick += OnPlayerInteraction;
			PlayerInputManager.Instance.localPlayerCamera = _camera;
			TabManager.Instance.onTabSelectionChange += (mode) => { interactionMode = mode; };
		}
		else
		{
			_camera.enabled = false;
		}
	}

	private void Update()
	{
		if (!IsOwner) return;

		if (movementMode == MovementMode.Free) FreeMovement();
		if (movementMode == MovementMode.Topdown) TopDownMovement();

		_raycast = CastRay();
		OnHover();

		ClearInspectionOnChange();
	}


	void FreeMovement()
	{
		_controller.Move(
			Time.deltaTime * GameData.Instance.playerMoveSpeed * 
			(_movement.y * transform.forward + _movement.x * transform.right)
		);
		transform.eulerAngles = transform.eulerAngles + new Vector3(_look.x, _look.y);
		_look = Vector2.zero;
	}

	void OnPlayerInteraction()
	{
		if (!IsOwner) return;

		if (interactionMode == PlayerInteraction.BuildMode) OnPlayerBuild();
	}

	void OnHover()
	{
		if (interactionMode == PlayerInteraction.InspectMode) OnPlayerInspect();
	}

	void TopDownMovement()
	{
		_controller.Move(
			Time.deltaTime * GameData.Instance.playerMoveSpeed *
			GameData.Instance.playerZoomSpeedBonus * transform.position.y *
			(_movement.y * Basic.XZPlane(transform.forward) +
			_movement.x * Basic.XZPlane(transform.right))
			);

		if (PlayerInputManager.Instance.mouseDown)
			_controller.Move(
				Vector3.up * _look.x
				* GameData.Instance.playerZoomSpeed
				);
	}

	/// <summary>
	/// Clears inspection if the object looked at is diffrent
	/// </summary>
	void ClearInspectionOnChange()
	{
		if (isInspecting)
		{
			if (_raycast.collider &&
			_raycast.collider.gameObject.GetInstanceID() 
			== initialyInspectedObject.GetInstanceID()) 
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
		_look = value;
	}
	void OnPlayerMove(Vector2 value) { 
		if (!IsOwner) return;
		_movement = value;
	}

	void OnPlayerInspect() 
	{
		if (_raycast.collider == null) return;
		GameObject inspectedObject = _raycast.collider.gameObject;
		initialyInspectedObject = inspectedObject;

		if (inspectedObject.GetComponentsInChildren<Inspectable>().Length == 0 && inspectedObject.transform.parent) 
			inspectedObject = inspectedObject.transform.parent.gameObject;

		Inspector.Instance.Inspect(inspectedObject);
		isInspecting = true;
	}

	void OnPlayerBuild()
	{
		if (_raycast.collider == null && !PlayerInputManager.Instance.isDraging) return;

		Construction construction = new Construction(
			ConstructionOptionsManager.Instance.selectedPrefab,
			_raycast.point,
			_raycast.normal
			);

		ConstructionManager.Instance.AddContructionServerRpc(construction);
	}

	void OnLead()
	{

	}

	RaycastHit CastRay()
	{
		Ray ray = _camera.ScreenPointToRay(PlayerInputManager.Instance.mousePosition);
		Physics.Raycast(ray, out RaycastHit hit, _maxInteractionDistance);
		return hit;
	}
}
