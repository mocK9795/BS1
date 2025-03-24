using Unity.Netcode;
using UnityEngine;
using GameFunctions;
using UnityEngine.Rendering.Universal;

public class Player : NetworkBehaviour
{
	[SerializeField] float _maxInteractionDistance;
	enum Mode { Political, Battle, Economic}
	[SerializeField] Mode mode;

	enum MovementMode { Free, Topdown, Lead}
	[SerializeField] MovementMode movementMode;

	public enum PlayerInteraction { InspectMode, BuildMode, Lead, Political}
	public PlayerInteraction interactionMode;

	public enum RaycastMode {Mouse, Centre}
	public RaycastMode raycastMode;

	Vector2 _movement;
	Vector2 _look;

	Camera _camera;
	CharacterController _controller;
	AudioListener _audioListener;

	RaycastHit _raycast;

	// Inspect Mode
	bool isInspecting;
	GameObject initialyInspectedObject;

	// Lead Mode
	bool isLeading;
	Leader leader;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		_camera = GetComponent<Camera>();
		_controller = GetComponent<CharacterController>();
		_audioListener = GetComponent<AudioListener>();

		if (IsOwner)
		{
			PlayerInputManager.Instance.onPlayerJump += OnPlayerJump;
			PlayerInputManager.Instance.onPlayerMove += OnPlayerMove;
			PlayerInputManager.Instance.onPlayerLook += OnPlayerLook;
			PlayerInputManager.Instance.onPlayerClick += OnPlayerInteraction;
			PlayerInputManager.Instance.localPlayerCamera = _camera;
			TabManager.Instance.onTabSelectionChange += OnInteractionModeChange;
		}
		else
		{
			_camera.enabled = false;
			_audioListener.enabled = false;
		}
	}

	private void Update()
	{
		if (!IsOwner) return;

		if (movementMode == MovementMode.Free) FreeMovement();
		if (movementMode == MovementMode.Topdown) TopDownMovement();
		if (movementMode == MovementMode.Lead) LeadMovement();

		_raycast = CastRay();
		OnHover();

		ClearInspectionOnChange();
	}

	void LeadMovement()
	{
		if (!leader) return;
		leader.MoveServerRpc(_movement);
		leader.RotateServerRpc(transform.eulerAngles.y);
		transform.position = leader.transform.position + GameData.Instance.leadFaceOffset;
		FreeRotation();
	}

	void FreeMovement()
	{
		_controller.Move(
			Time.deltaTime * GameData.Instance.playerMoveSpeed * 
			(_movement.y * transform.forward + _movement.x * transform.right)
		);
		FreeRotation();
	}

	void FreeRotation()
	{
		transform.eulerAngles = transform.eulerAngles + new Vector3(_look.x, _look.y);
		_look = Vector2.zero;
	}

	void OnPlayerInteraction()
	{
		if (!IsOwner) return;

		if (interactionMode == PlayerInteraction.BuildMode) OnPlayerBuild();
		if (interactionMode == PlayerInteraction.Lead) OnPlayerLead();
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

	void OnPlayerLead()
	{
		if (!leader) { AssignLeader(); return; }
		leader.weapon.Damage(_raycast);
	}

	void OnPoliticalModeActivate()
	{
		_camera.orthographic = true;
		_camera.orthographicSize = GameData.Instance.orthographicSize;
		transform.position = new(transform.position.x, GameData.Instance.orthographicYPosition, transform.position.z);
	}
	void AssignLeader()
	{
		if (!_raycast.collider) return;
		Leader leaderComponent = _raycast.collider.GetComponent<Leader>();
		if (!leaderComponent && _raycast.collider.transform.parent)
			leaderComponent = _raycast.collider.transform.parent.GetComponent<Leader>();
		
		leader = leaderComponent;
		movementMode = MovementMode.Lead;
		raycastMode = RaycastMode.Centre;
		GameData.Instance.crossHair.SetActive(true);
	}

	void OnInteractionModeChange(PlayerInteraction mode)
	{
		if (!IsOwner) return;

		if (interactionMode == PlayerInteraction.Political) ResetY();
		interactionMode = mode;
		
		transform.eulerAngles = GameData.Instance.topdownRotation;
		GameData.Instance.crossHair.SetActive(false);
		movementMode = MovementMode.Topdown;
		raycastMode = RaycastMode.Mouse;
		_camera.orthographic = false;

		if (interactionMode == PlayerInteraction.Political) OnPoliticalModeActivate();
	}

	void ResetY() 
	{ 
		transform.position = 
			new(transform.position.x, GameData.Instance.defaultPlayerY, transform.position.z);
	}

	RaycastHit CastRay()
	{
		Ray ray;
		if (raycastMode == RaycastMode.Mouse) 
			ray = _camera.ScreenPointToRay(PlayerInputManager.Instance.mousePosition);
		else if (raycastMode == RaycastMode.Centre)
			ray = new Ray(transform.position, transform.forward);
		else return new RaycastHit();
		Physics.Raycast(ray, out RaycastHit hit, _maxInteractionDistance);
		return hit;
	}
}
