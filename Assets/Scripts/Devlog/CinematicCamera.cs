using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class CinematicCamera : Player
{
    [Range(24, 120)]
    [SerializeField] float playSpeed;
    List<Vector3> positions;
    List<Vector3> rotations;

    void OnPlayerJump() 
    {
        positions.Add(transform.position);
        rotations.Add(transform.eulerAngles);
    }

    void OnPlayAnimation() 
    {
        throw(new NotImplementedException());
    }

    IEnumerator PlayAnimation()
    {
        float step = 1 / playSpeed;

        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 initialMovementLerp = Vector3.Lerp(transform.position, positions[i], step) - transform.position;
            Vector3 initialRotationLerp = Vector3.Lerp(transform.eulerAngles, rotations[i], step);

            transform.position = initialMovementLerp;
            transform.eulerAngles = initialRotationLerp;
            yield return new WaitForSeconds(1);
        }

        yield break;
    }

	private void Start()
	{
		positions = new List<Vector3>();
        rotations = new List<Vector3>();

        PlayerInputManager.Instance.onPlayerJump += OnPlayerJump;
        PlayerInputManager.Instance.onPlayerClickEnd += OnPlayAnimation;
	}
}
