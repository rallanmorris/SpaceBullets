using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	[SerializeField] GameObject player;
	[SerializeField] PlayerController playerController;
	[SerializeField] MeshDestroy meshDestroyer;

	[SerializeField] ParticleSystem deathParticles;
	[SerializeField] ParticleSystem fireParticles;

	[SerializeField] Transform spawnBulletPosition;
	[SerializeField] Transform spawnBulletPosition2;
	[SerializeField] Transform bulletPF;
	[SerializeField] int health = 1;

	private Vector3 aimVector;
	private Vector3 aimVector2;
	private Vector3 playerPosition;
	private bool isFiring = false;
	private bool isSpinning;
	private bool isStoppedSpinning;
	private bool isAttackingPlayer;
	private bool isAlreadyDead;

	[SerializeField] float lookSpeed = 2f;

	public float distanceToPlayer;

	public enum State
	{
		Patrol,
		FollowPlayer,
		Combat,
		Death
	}

	public enum Combat
	{
		Aim,
		WarmUp,
		Shoot,
		CoolDown,
	}

	[SerializeField] float followSpeed = 1f;
	[SerializeField] float stopSpeed = 1f;
	[SerializeField] float aimTime = 1f;
	[SerializeField] float warmUpTime = 2f;
	[SerializeField] float shootTime = 3f;
	[SerializeField] float coolDownTime = 2f;
	[SerializeField] float followRange = 7f;
	[SerializeField] float deathTime = 5f;

	public State _state;
	public Combat combatState;
	public float timer = 0f;
	public float stopTimer = 0f;
	private Rigidbody enemyRigidbody;

	// Start is called before the first frame update
	void Start()
    {
		isAlreadyDead = false;

		enemyRigidbody = GetComponent<Rigidbody>();
		_state = State.Patrol;
		combatState = Combat.Aim;
	}

    // Update is called once per frame
    void Update()
    {
		//Check if dead
		if (health < 1)
			_state = State.Death;

		if (!playerController.isDead)
		{
			//Check if player is within range
			if (GetDistanceToPlayer() < followRange && _state != State.Combat && _state != State.Death)
			{
				timer = 0f;
				stopTimer = 0f;
				_state = State.Combat;
				combatState = Combat.Aim;
				//isAttackingPlayer = true;
				//UpdateAimVector();
				//StartCoroutine(AttackPlayer());
			}

			else if (GetDistanceToPlayer() >= followRange && GetDistanceToPlayer() < 50 && _state != State.Death)
			{
				if (combatState == Combat.Aim)
					_state = State.FollowPlayer;
			}

			if (GetDistanceToPlayer() >= 50 && _state != State.Death)
			{
				_state = State.Patrol;
			}

			fireParticles.transform.position = gameObject.transform.position;
			deathParticles.transform.position = gameObject.transform.position;
		}

		else
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
			case State.Death:
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
		//Vector3.MoveTowards(gameObject.transform.position, player.transform.position, followSpeed * Time.deltaTime);

		Vector3 enemyTrajectory = new Vector3(gameObject.transform.forward.x, gameObject.transform.forward.y, 0f);
		enemyRigidbody.velocity = enemyTrajectory * followSpeed;
	}

	void StopMoving()
	{
		stopTimer += Time.deltaTime * stopSpeed;
		if (stopTimer > 1f)
			stopTimer = 1f;
		enemyRigidbody.velocity = Vector3.Lerp(transform.forward * followSpeed, Vector3.zero, stopTimer);
		
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
			StopMoving();
			timer += Time.deltaTime;
			if (timer > coolDownTime)
			{
				timer = 0f;
				stopTimer = 0f;
				combatState = Combat.Aim;
			}
		}
	}

	void Die()
	{
		if(!isAlreadyDead)
		{
			if (!fireParticles.isStopped)
				fireParticles.Stop();
			if (!deathParticles.isPlaying)
				deathParticles.Play();

			meshDestroyer.DestroyMesh();
			isAlreadyDead = true;
			timer = 0;
		}
		else
		{
			timer += Time.deltaTime;
			if (timer > deathTime)
				Destroy(gameObject);
		}
		

	}

	public float GetDistanceToPlayer()
	{
		distanceToPlayer = Vector3.Distance(this.gameObject.transform.position, player.transform.position);
		return distanceToPlayer;
	}


	void UpdateAimVector()
	{
		aimVector = (player.transform.position - spawnBulletPosition.position).normalized;
		aimVector2 = (player.transform.position - spawnBulletPosition2.position).normalized;
	}

	void LookAtPlayer()
	{
		Vector3 lookDir = player.transform.position - gameObject.transform.position;
		lookDir.z = 0;
		Quaternion lookRotation = Quaternion.LookRotation(lookDir);
		gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, lookRotation, lookSpeed * Time.deltaTime);
	}

	void Spin()
	{
		gameObject.transform.Rotate(0, 0, 720* Time.deltaTime ); //rotates 7200 degrees per second around z axis

	}

	void StopSpinning()
	{
		gameObject.transform.Rotate(0, 0, 0); //rotates 0 degrees per second around z axis

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
			bullet.GetComponent<BulletProjectile>().EnemyShot(true);
			bullet.transform.position = spawnBulletPosition.position;
			bullet.transform.rotation = Quaternion.LookRotation(aimVector, Vector3.up);
			bullet.GetComponent<BulletProjectile>().InstantiateFromPool();
		}

		GameObject bullet2 = ObjectPool.SharedInstance.GetPooledObject();
		if (bullet2 != null)
		{
			bullet2.GetComponent<BulletProjectile>().EnemyShot(true);
			bullet2.transform.position = spawnBulletPosition2.position;
			bullet2.transform.rotation = Quaternion.LookRotation(aimVector2, Vector3.up);
			bullet2.GetComponent<BulletProjectile>().InstantiateFromPool();
		}
		isFiring = true;
		yield return new WaitForSeconds(.1f);
		isFiring = false;
	}


	public void TakeDamage(int dmg)
    {
		health -= dmg;
		Debug.Log("Enemy Health:" + health);

		if (!fireParticles.isPlaying)
			fireParticles.Play();
    }
}
