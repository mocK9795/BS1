using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using GameFunctions;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance = null;
	EventSystem eventSystem;

	public Action onJump;
	public Action<Vector2> onMove;
	public Action<Vector2> onLook;

	private void Start()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(Instance); Instance = this; }
	
		eventSystem = EventSystem.current;
	}

	public void OnJump(InputAction.CallbackContext value)
	{
		if (value.canceled) onJump?.Invoke();
    }

	public void OnMove(InputAction.CallbackContext value)
	{
		onMove?.Invoke(value.ReadValue<Vector2>());
	}

	public void OnLook(InputAction.CallbackContext value)
	{
		onLook?.Invoke(Basic.LookValue(value.ReadValue<Vector2>()));
	}
}
