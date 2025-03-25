using Unity.Netcode;
using UnityEngine;
using GameFunctions;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
	[SerializeField] float _maxInteractionDistance;

	enum MovementMode { Free, Topdown, Lead, Orthographic}
	[SerializeField] MovementMode movementMode;
	[SerializeField] bool _utilizeOrthographicCamera;

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
	[SerializeField] List<Infantry> selectedInfantry = new();

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
		if (movementMode == MovementMode.Orthographic) OrthographicMovement();

		_raycast = Raycast();
		OnHover();

		ClearInspectionOnChange();
	}
	

	// Movement Based Functions
	void TopDownMovement()
	{
		_controller.Move(
			Time.deltaTime * GameData.Instance.playerMoveSpeed *
			GameData.Instance.playerZoomSpeedBonus * transform.position.y *
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
	void OrthographicMovement()
	{
		_controller.Move(
			Time.deltaTime * GameData.Instance.playerMoveSpeed *
			GameData.Instance.playerZoomSpeedBonus * _camera.orthographicSize *
			(_movement.y * Vector3.forward + _movement.x * Vector3.right)
		);

		if (PlayerInputManager.Instance.mouseDown) {
			if (_camera.orthographic) ZoomOrthographicSize();
			else ZoomY();
		}
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
	void OnPlayerInteraction()
	{
		if (!IsOwner) return;

		if (interactionMode == PlayerInteraction.BuildMode) OnPlayerBuild();
		if (interactionMode == PlayerInteraction.Lead) OnPlayerLead();
		if (interactionMode == PlayerInteraction.Battle) OnBattleInteraction();
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


	// Interaction Mode Based Functions
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
		if (!activeInfantry) { AssignLeader(); return; }
		activeInfantry.weapon.Damage(_raycast);
	}
	void OnBattleInteraction()
	{
		print(_raycast.collider);
		if (!_raycast.collider) return;

		Infantry infantry = _raycast.collider.GetComponent<Infantry>();
		if (!infantry) infantry = _raycast.collider.transform.parent.GetComponent<Infantry>();

		if (infantry) {
			selectedInfantry.Add(infantry);
			return; 
		}

		foreach (Infantry selected in selectedInfantry) {
			if (!selected) continue;
			selected.OnCommandServerRpc(_raycast.point);
		}
	}


	// Interaction Mode Switch Based Functions
	void OnBattleModeActivate()
	{
		transform.eulerAngles = new(90, 0, 0);
		movementMode = MovementMode.Orthographic;
		if (_utilizeOrthographicCamera) _camera.orthographic = true;
		_camera.orthographicSize = GameData.Instance.orthographicSize;
		Teleport(new(transform.position.x, GameData.Instance.orthographicYPosition, transform.position.z));
	}
	void AssignLeader()
	{
		if (!_raycast.collider) return;
		Infantry infantryComponent = _raycast.collider.GetComponent<Infantry>();
		if (!infantryComponent && _raycast.collider.transform.parent)
			infantryComponent = _raycast.collider.transform.parent.GetComponent<Infantry>();
		
		raycastMode = RaycastMode.Centre;
		movementMode = MovementMode.Lead;
		activeInfantry = infantryComponent;
		activeInfantry.OnControllServerRpc();
		GameData.Instance.crossHair.SetActive(true);
	}
	void OnInteractionModeChange(PlayerInteraction mode)
	{
		if (!IsOwner) return;

		if (interactionMode is PlayerInteraction.Battle or PlayerInteraction.Lead) ResetTransform();
		interactionMode = mode;

		if (activeInfantry) activeInfantry.OnExitControllServerRpc();
		GameData.Instance.crossHair.SetActive(false);
		movementMode = MovementMode.Topdown;
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
