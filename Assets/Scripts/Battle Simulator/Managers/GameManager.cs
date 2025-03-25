using UnityEngine;

public class GameManager : BaseManager
{
    [Range(0.1f, 1f)]
    [SerializeField] float _gameSpeed;

    public override void Tick()
    {
    }

	private void OnValidate()
	{
		Time.timeScale = _gameSpeed;
	}
}
