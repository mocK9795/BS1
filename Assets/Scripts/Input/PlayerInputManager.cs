using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using GameFunctions;
using System.Collections.Generic;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance = null;
	List<IReceivePlayerLook> lookRecipients;
	List<IReceivePlayerMove> moveRecipients;
	List<IReceivePlayerJump> jumpRecipients;
	EventSystem eventSystem;

	private void Start()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(Instance); Instance = this; }

		eventSystem = EventSystem.current;
		lookRecipients = new List<IReceivePlayerLook>();
		moveRecipients = new List<IReceivePlayerMove>();
		jumpRecipients = new List<IReceivePlayerJump>();
	}

	public void OnJump(InputAction.CallbackContext value)
	{
		CleanRecipients();

		if (!value.canceled) return;
		foreach (var  recipient in jumpRecipients) 
		{ recipient.OnPlayerJump(); }
    }

	public void OnMove(InputAction.CallbackContext value)
	{
		CleanRecipients();

		Vector2 moveValue = value.ReadValue<Vector2>();
		foreach (var recipient in moveRecipients) 
		{ recipient.OnPlayerMove(moveValue); }
	}

	public void OnLook(InputAction.CallbackContext value)
	{
		CleanRecipients();

		Vector2 lookValue = Basic.LookValue(value.ReadValue<Vector2>());
		foreach (IReceivePlayerLook recipient in lookRecipients)
		{ recipient.OnPlayerLook(lookValue); }
	}

	public void Subscribe(IReceivePlayerLook recipient)
	{
		lookRecipients.Add(recipient);
	}
	public void Subscribe(IReceivePlayerJump recipient)
	{
		jumpRecipients.Add(recipient);
	}
	public void Subscribe(IReceivePlayerMove recipient)
	{
		moveRecipients.Add(recipient);
	}
	void CleanRecipients() 
	{
		for (int i = 0; i < lookRecipients.Count; i++) 
		{ if (lookRecipients[i] == null) { lookRecipients.RemoveAt(i); i--; } }
		for (int i = 0; i < moveRecipients.Count; i++)
		{ if (moveRecipients[i] == null) { moveRecipients.RemoveAt(i); i--; } }
		for (int i = 0; i < jumpRecipients.Count; i++)
		{ if (jumpRecipients[i] == null) { jumpRecipients.RemoveAt(i); i--; } }
	}
}

public interface IReceivePlayerLook
{
	public void OnPlayerLook(Vector2 value);
}

public interface IReceivePlayerJump
{
	public void OnPlayerJump();
}

public interface IReceivePlayerMove
{
	public void OnPlayerMove(Vector2 value);
}