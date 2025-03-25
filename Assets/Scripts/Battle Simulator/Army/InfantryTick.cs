using System;

public class InfantryTick : BaseManager
{
	public static InfantryTick Instance;
	public Action Logic;

	private void Start()
	{
		if (Instance) Destroy(Instance);
		Instance = this;
	}

	public override void Tick()
	{
		Logic?.Invoke();
	}
}
