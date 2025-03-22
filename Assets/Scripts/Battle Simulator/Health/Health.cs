using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
	NetworkObject _object;
    public NetworkVariable< float > health = new(5);
	public NetworkVariable< bool > active = new(true);
    public enum HealthMode {Composite, Self}
    [SerializeField] HealthMode _healthMode;
    public HealthMode healthMode {get { return _healthMode;}}

	[ServerRpc(RequireOwnership = false)]
	public void DamageServerRpc(float value)
	{
		if (healthMode == HealthMode.Composite) return;
		health.Value -= value;
	}

	[ServerRpc(RequireOwnership = false)]
	public void UpdateHealthServerRpc()
	{
		if (healthMode == HealthMode.Self) return;

		float totalHealth = 0;
		Health[] healthComposition = GetComponentsInChildren<Health>();
		foreach (Health health in healthComposition)
		{
			if (health.healthMode == HealthMode.Composite) continue;
			totalHealth += health.health.Value;
		}
		health.Value = totalHealth;
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		_object = GetComponent<NetworkObject>();

		UpdateHealthServerRpc();
	}

	private void Update()
	{
		gameObject.SetActive(active.Value);
		if (!IsServer) return;
		if (health.Value <= 0f)
		{
			if (_object)
			_object.Despawn();

			else active.Value = false;
		}
	}
}
