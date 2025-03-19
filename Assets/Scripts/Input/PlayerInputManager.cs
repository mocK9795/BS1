using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using GameFunctions;
using System.Collections.Generic;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance = null;

	public Action onPlayerJump;
	public Action onPlayerClick;
	public Action<Vector2> onPlayerMove;
	public Action<Vector2> onPlayerLook;
	public Vector2 mousePosition;
	
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
	}

	public void OnClick(InputAction.CallbackContext value)
	{
		if (!value.canceled) return;
		onPlayerClick?.Invoke();
	} 

	public void GrantMousePosition(InputAction.CallbackContext value)
	{
		mousePosition = value.ReadValue<Vector2>();
	}
}