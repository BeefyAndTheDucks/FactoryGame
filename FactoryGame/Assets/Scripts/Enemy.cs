using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[Tooltip("Cooldown before the enemy attacks again (Seconds)")]
	[Range(0, 10)]
	public float attackCooldown = 1;
	[Range(1, 10)]
	public int damage = 1;

	float cooldown = 1;

	public void Attack()
	{
		if (cooldown <= 0)
		{
			cooldown = attackCooldown;
			PlayerHealthManager.TakeDamage(damage);
		}
	}

	void Update()
	{
		if (cooldown > 0)
			cooldown -= Time.deltaTime;
	}
}
