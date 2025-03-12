using UnityEngine;

public abstract class BaseManager : MonoBehaviour
{
    [Range(0.1f, 20f)]
    [SerializeField] float tickRate;
	float _tickTimer;
	float _tickSpeed;

	private void Start()
	{
		_tickSpeed = 1 / tickRate;
	}

	public void Update()
	{
		_tickTimer += Time.deltaTime;
		if (_tickTimer > _tickSpeed) { 
			_tickTimer = 0;
			Tick();
		}
	}

	public abstract void Tick();
}
