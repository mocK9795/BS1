using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour, Inspectable
{
    [SerializeField] float _damage;
    [SerializeField] float _range;
    [SerializeField] float _speed;
    [SerializeField] AudioClip _attackSound;
    Animator _animator;
    AudioSource _audioSource;
    float _timeSinceLastDamage;

    public void Damage(RaycastHit raycast)
    {
		_animator.SetTrigger("Fire");
		_audioSource.PlayOneShot(_attackSound);

		if (_timeSinceLastDamage < _speed) return;
        if (!raycast.collider) return;
        if (Vector3.Distance(transform.position, raycast.point) > _range) return;
        Health obj = raycast.collider.GetComponent<Health>();
        if (obj == null) 
            obj = raycast.collider.transform.parent.GetComponent<Health>();
        if (obj == null) return;

        Health[] healthObjects;
        if (obj.healthMode == Health.HealthMode.Composite)
            healthObjects = obj.GetComponentsInChildren<Health>();
        else if (obj.healthMode == Health.HealthMode.Self)
            healthObjects = new Health[1] { obj };
        else return;

        int count = healthObjects.Length;
        foreach (Health health in healthObjects)
        {
            if (health.healthMode == Health.HealthMode.Composite) continue;
            health.DamageServerRpc(_damage / count);
        }
        if (obj.healthMode == Health.HealthMode.Composite) obj.UpdateHealthServerRpc();
    }

	public InspectionData GetInspectableData()
	{
        return new(
            "Damage " + _damage.ToString() + "\n" +
            "Range " + _range.ToString() + "\n" +
            "Speed " + _speed.ToString(),
            new(1.5f, 1.2f)
            );
	}

	private void Start()
	{
		_animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponentInChildren<AudioSource>();
	}

	private void Update()
	{
        if (_timeSinceLastDamage < _speed) _timeSinceLastDamage += Time.deltaTime;
	}

}
