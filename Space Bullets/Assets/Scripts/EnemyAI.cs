using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	[SerializeField] GameObject player;
	[SerializeField] PlayerController playerController;
	[SerializeField] Transform spawnBulletPosition;
	[SerializeField] Transform bulletPF;

	public Vector3 aimVector;
	private Vector3 playerPosition;
	private bool isFiring = false;
	private bool isSpinning;
	private bool isStoppedSpinning;
	private bool isAttackingPlayer;

	[SerializeField] float lookSpeed = 2f;

	public float distanceToPlayer;

	public enum State
	{
		Patrol,
		FollowPlayer,
		Combat,
		Die
	}

	public enum Combat
	{
		Aim,
		WarmUp,
		Shoot,
		CoolDown,
	}

	[SerializeField] float followSpeed = 1f;
	[SerializeField] float aimTime = 1f;
	[SerializeField] float warmUpTime = 2f;
	[SerializeField] float shootTime = 3f;
	[SerializeField] float coolDownTime = 2f;

	private State _state;
	private Combat combatState;
	private float timer = 0f;


	// Start is called before the first frame update
	void Start()
    {
		_state = State.Patrol;
	}

    // Update is called once per frame
    void Update()
    {
		//Check if player is within range
		if (GetDistanceToPlayer() < 8 && _state != State.Combat)
		{
			timer = 0f;
			_state = State.Combat;
			combatState = Combat.Aim;
			//isAttackingPlayer = true;
			//UpdateAimVector();
			//StartCoroutine(AttackPlayer());
		}

		else if (GetDistanceToPlayer() >= 8 || GetDistanceToPlayer() < 15)
		{
			_state = State.FollowPlayer;
		}

		if (GetDistanceToPlayer() >= 15)
		{
			_state = State.Patrol;
		}

		switch (_state)
		{
			case State.Patrol:
				Patrol();
				break;
			case State.FollowPlayer:
				FollowPlayer();
				break;
			case State.Combat:
				InCombat();
				break;
			case State.Die:
				Die();
				break;
		}
		
    }

	void Patrol()
	{
		//TODO
	}

	void FollowPlayer()
	{
		LookAtPlayer();
		Vector3.MoveTowards(gameObject.transform.position, player.transform.position, followSpeed * Time.deltaTime);
	}

	void InCombat()
	{
		if(combatState == Combat.Aim)
		{
			LookAtPlayer();
			UpdateAimVector();
			timer += Time.deltaTime;
			if (timer > aimTime)
			{
				timer = 0f;
				combatState = Combat.WarmUp;
			}
		}

		if(combatState == Combat.WarmUp)
		{
			Spin();

			timer += Time.deltaTime;
			if(timer > warmUpTime)
			{
				timer = 0f;
				combatState = Combat.Shoot;
			}
		}

		if(combatState == Combat.Shoot)
		{
			ShootAtPlayer();
			Spin();
			timer += Time.deltaTime;
			if (timer > shootTime)
			{
				timer = 0f;
				combatState = Combat.CoolDown;
			}
		}

		if(combatState == Combat.CoolDown)
		{
			StopSpinning();
			timer += Time.deltaTime;
			if (timer > coolDownTime)
			{
				timer = 0f;
				combatState = Combat.Aim;
			}
		}
	}

	void Die()
	{
		//TODO
	}

	public float GetDistanceToPlayer()
	{
		distanceToPlayer = Vector3.Distance(this.gameObject.transform.position, player.transform.position);
		return distanceToPlayer;
	}


	void UpdateAimVector()
	{
		aimVector = (player.transform.position - spawnBulletPosition.position).normalized;
	}

	void LookAtPlayer()
	{
		Vector3 lookDir = player.transform.position - gameObject.transform.position;
		lookDir.z = 0;
		Quaternion lookRotation = Quaternion.LookRotation(lookDir);
		transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, lookRotation, lookSpeed * Time.deltaTime);
	}

	void Spin()
	{
		transform.Rotate(0, 0, 720* Time.deltaTime ); //rotates 7200 degrees per second around z axis

	}

	void StopSpinning()
	{
		transform.Rotate(0, 0, 0); //rotates 0 degrees per second around z axis

	}

	void ShootAtPlayer()
	{
		if (!isFiring)
			StartCoroutine(FireGun());
	}

	IEnumerator FireGun()
	{
		GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
		if (bullet != null)
		{
			bullet.transform.position = spawnBulletPosition.position;
			bullet.transform.rotation = Quaternion.LookRotation(aimVector, Vector3.up);
			bullet.GetComponent<BulletProjectile>().InstantiateFromPool();
		}
		isFiring = true;
		yield return new WaitForSeconds(.1f);
		isFiring = false;
	}


	public void TakeDamage(int dmg)
    {

    }
}
