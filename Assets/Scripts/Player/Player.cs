using Unity.Netcode;
using UnityEngine;
using GameFunctions;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour
{
	[SerializeField] float _maxInteractionDistance;

	enum MovementMode { Free, AirView, Lead, TopDown}
	[SerializeField] MovementMode movementMode;

	// Lead means that the player is controlling infantry
	public enum PlayerInteraction { InspectMode, BuildMode, Lead, Battle}
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
	Infantry activeInfantry;

	// Battle Mode
	[SerializeField] List<Unit> selectedInfantry = new();
	Vector3 _selectionStart;
	Vector3 _selectionEnd;
	bool _isSelecting;

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
			PlayerInputManager.Instance.onPlayerClickStart += OnPlayerInteractionStart;
			PlayerInputManager.Instance.onPlayerClickEnd += OnPlayerInteractionEnd;
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
		if (movementMode == MovementMode.AirView) AirViewMovement();
		if (movementMode == MovementMode.Lead) LeadMovement();
		if (movementMode == MovementMode.TopDown) TopdownMovement();

		_raycast = Raycast();
		OnHover();

		ClearInspectionOnChange();
	}
	

	// Movement Based Functions
	void AirViewMovement()
	{
		_controller.Move(
			MovementSpeed() *
			(_movement.y * Basic.XZPlane(transform.forward) +
			_movement.x * Basic.XZPlane(transform.right))
			);

		if (PlayerInputManager.Instance.mouseDown) ZoomY();
	}
	void LeadMovement()
	{
		if (!activeInfantry) return;
		FreeRotation();
		activeInfantry.MoveServerRpc(_movement);
		activeInfantry.RotateServerRpc(transform.eulerAngles.y);
		transform.position = activeInfantry.transform.position + GameData.Instance.leadFaceOffset;
	}
	void TopdownMovement()
	{
		_controller.Move(
			MovementSpeed() *
			(_movement.y * Vector3.forward + _movement.x * Vector3.right)
		);

		if (PlayerInputManager.Instance.mouseDown) ZoomY();
	}
	float MovementSpeed() {
		return Time.deltaTime * GameData.Instance.playerMoveSpeed *
			GameData.Instance.playerZoomSpeedBonus * transform.position.y;
	}
	void ZoomY() { _controller.Move(Vector3.up * _look.x * GameData.Instance.playerZoomSpeed); }
	void ZoomOrthographicSize()
	{
		_camera.orthographicSize += _look.x * GameData.Instance.playerZoomSpeed;
		_camera.orthographicSize = Mathf.Max(_camera.orthographicSize, 0.1f);
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


	// Pointer Based Functions
	void OnPlayerInteractionStart() 
	{ 
		if (interactionMode == PlayerInteraction.Battle) OnBattleInteractionStart();
	}
	void OnPlayerInteractionEnd()
	{
		if (!IsOwner) return;

		if (interactionMode == PlayerInteraction.BuildMode) OnPlayerBuild();
		if (interactionMode == PlayerInteraction.Lead) OnPlayerLead();
		if (interactionMode == PlayerInteraction.Battle) OnBattleInteractionComplete();
	}
	void OnHover()
	{
		if (interactionMode == PlayerInteraction.InspectMode) OnPlayerInspect();
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


	// Basic Input Functions
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


	// Interaction Mode Based Functions (On Click Cancel)
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
		if (!_raycast.collider
			|| !PlayerInputManager.Instance.isDraging
			|| _raycast.collider.gameObject.layer != 6
		) return;

		Construction construction = new Construction(
			ConstructionOptionsManager.Instance.selectedPrefab,
			_raycast.point,
			_raycast.normal
			);

		ConstructionManager.Instance.AddContructionServerRpc(construction);
	}
	void OnPlayerLead()
	{
		if (!activeInfantry) { AssignLeader(); return; }
		activeInfantry.weapon.Damage(_raycast);
	}
	void OnBattleInteractionComplete()
	{
		_selectionEnd = _raycast.point;
		
		if (_isSelecting)
		{
			SelectArmyInSelection();
			return;
		}

		_isSelecting = false;
	}

	void SelectArmyInSelection()
	{
		Vector3 pointA = new(_selectionStart.x, GameData.Instance.selectionBoundsY.x, _selectionStart.z);
		Vector3 pointB = new(_selectionEnd.x, GameData.Instance.selectionBoundsY.y, _selectionEnd.z);
		Collider[] selectedObjects = BasicPhysics.GetAllWithinBox(pointA, pointB);
        foreach (var item in selectedObjects)
        {
			var component = BasicComponent.GetInParent<Unit>(item);
			if (component) selectedInfantry.Add(component);
        }
    }


	// Interaction Mode Based Functions (On Click Start)
	void OnBattleInteractionStart() 
	{
		_isSelecting = false;
		_selectionStart = _raycast.point;
		if (_raycast.collider) _isSelecting = _raycast.collider.gameObject.layer == 6;
	}


	// Interaction Mode Switch Based Functions
	void OnBattleModeActivate()
	{
		transform.eulerAngles = new(90, 0, 0);
		movementMode = MovementMode.TopDown;
	}
	void AssignLeader()
	{
		if (!_raycast.collider) return;
		Infantry infantryComponent = _raycast.collider.GetComponent<Infantry>();
		if (!infantryComponent && _raycast.collider.transform.parent)
			infantryComponent = _raycast.collider.transform.parent.GetComponent<Infantry>();
		if (!infantryComponent) return;

		raycastMode = RaycastMode.Centre;
		movementMode = MovementMode.Lead;
		activeInfantry = infantryComponent;
		activeInfantry.OnControllServerRpc();
		GameData.Instance.crossHair.SetActive(true);
	}
	void OnInteractionModeChange(PlayerInteraction mode)
	{
		if (!IsOwner) return;

		if (interactionMode is PlayerInteraction.Lead) ResetTransform();
		interactionMode = mode;

		if (activeInfantry) { activeInfantry.OnExitControllServerRpc(); activeInfantry = null; }
		GameData.Instance.crossHair.SetActive(false);
		movementMode = MovementMode.AirView;
		raycastMode = RaycastMode.Mouse;
		_camera.orthographic = false;
		ResetRotation();

		if (interactionMode == PlayerInteraction.Battle) OnBattleModeActivate();
	}


	// Commonly Used Functions
	/// <summary>
	/// Only resets the y position & rotation
	/// </summary>
	void ResetTransform() 
	{
		transform.position = 
			new(transform.position.x, GameData.Instance.defaultPlayerY, transform.position.z);
		ResetRotation();
	}
	void ResetRotation() { 
		transform.eulerAngles = GameData.Instance.topdownRotation;
	}
	RaycastHit Raycast()
	{
		Ray ray;
		if (raycastMode == RaycastMode.Mouse && MouseInScreen())
			ray = _camera.ScreenPointToRay(PlayerInputManager.Instance.mousePosition);
		else if (raycastMode == RaycastMode.Centre)
			ray = new Ray(transform.position, transform.forward);
		else return new RaycastHit();
		Physics.Raycast(ray, out RaycastHit hit, _maxInteractionDistance);
		return hit;
	}
	bool MouseInScreen()
	{
		return Basic.Within(
			PlayerInputManager.Instance.mousePosition,
			new Vector2(),
			new(Screen.width, Screen.height));
	}
	void Teleport(Vector3 positon)
	{
		_controller.Move(positon - transform.position);
	}
}
