using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using GameFunctions;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance = null;

	public Action onPlayerJump;
	public Action onPlayerClickEnd;
	public Action onPlayerClickStart;
	public Action<Vector2> onPlayerMove;
	public Action<Vector2> onPlayerLook;
	public Vector2 mousePosition { get; private set; }
	public bool mouseDown { get; private set; }
	public bool isDraging { get; private set; }
	public float dragThresshold;
	float currentDrag;

	public Camera localPlayerCamera;

	EventSystem eventSystem;

	private void Start()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(Instance); Instance = this; }

		eventSystem = EventSystem.current;
	}

	public void OnJump(InputAction.CallbackContext value)
	{
		if (!value.canceled) return;
		onPlayerJump?.Invoke();
	}

	public void OnMove(InputAction.CallbackContext value)
	{
		Vector2 moveValue = value.ReadValue<Vector2>();
		onPlayerMove?.Invoke(moveValue);
	}

	public void OnLook(InputAction.CallbackContext value)
	{
		Vector2 lookValue = Basic.LookValue(value.ReadValue<Vector2>());
		onPlayerLook?.Invoke(lookValue);

		if (mouseDown) {
			currentDrag += lookValue.magnitude;
			if (currentDrag > dragThresshold) isDraging = true; 
		} 
	}

	public void OnClick(InputAction.CallbackContext value)
	{
		mouseDown = !value.canceled;
		if (value.started) onPlayerClickStart?.Invoke();
		if (!value.canceled) return;

		isDraging = false;
		currentDrag = 0;

		onPlayerClickEnd?.Invoke();
	} 

	public void GrantMousePosition(InputAction.CallbackContext value)
	{
		mousePosition = value.ReadValue<Vector2>();
	}
}