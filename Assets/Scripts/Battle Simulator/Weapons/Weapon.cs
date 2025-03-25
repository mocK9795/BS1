using System;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour, Inspectable
{
    public float damage;
    public float range;
    public float speed;

    [SerializeField] AudioClip _attackSound;
    Animator _animator;
    AudioSource _audioSource;
    float _timeSinceLastDamage;


    public void Damage(RaycastHit raycast)
    {
		Health aim = raycast.collider.GetComponent<Health>();
		if (aim == null)
			aim = raycast.collider.transform.parent.GetComponent<Health>();
        if (aim == null) {ProduceDamageVisuals(); return;}

        Damage(aim, raycast.point);
	}

    public void Damage(Health aim, Nullable<Vector3> _point = null)
    {
		if (_timeSinceLastDamage < speed) return;
        if (!aim) return;
        
        Vector3 point;

        if (_point != null) point = (Vector3)_point;
        else point = aim.transform.position; 

		if (Vector3.Distance(transform.position, point) > range) return;

        Health[] healthObjects;
        if (aim.healthMode == Health.HealthMode.Composite)
            healthObjects = aim.GetComponentsInChildren<Health>();
        else if (aim.healthMode == Health.HealthMode.Self)
            healthObjects = new Health[1] { aim };
        else return;

        int count = healthObjects.Length;
        foreach (Health health in healthObjects)
        {
            if (health.healthMode == Health.HealthMode.Composite) continue;
            health.DamageServerRpc(damage / count);
        }
        if (aim.healthMode == Health.HealthMode.Composite) aim.UpdateHealthServerRpc();

        _timeSinceLastDamage = 0;
		ProduceDamageVisuals();
	}

	void ProduceDamageVisuals()
	{
		_animator.SetTrigger("Fire");
		_audioSource.PlayOneShot(_attackSound);
	}

	public InspectionData GetInspectableData()
	{
        return new(
            "Weapon\n" +
            "Damage " + damage.ToString() + "\n" +
            "Range " + range.ToString() + "\n" +
            "Speed " + speed.ToString(),
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
        if (_timeSinceLastDamage < speed) _timeSinceLastDamage += Time.deltaTime;
	}
}
