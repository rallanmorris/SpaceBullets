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

	// Start is called before the first frame update
	void Start()
    {
		isAttackingPlayer = false;
	}

    // Update is called once per frame
    void Update()
    {
		if (GetDistanceToPlayer() < 10 && !isAttackingPlayer)
		{
			isAttackingPlayer = true;
			UpdateAimVector();
			StartCoroutine(AttackPlayer());
		}
		
    }

	public float GetDistanceToPlayer()
	{
		distanceToPlayer = Vector3.Distance(this.gameObject.transform.position, player.transform.position);
		return distanceToPlayer;
	}

	IEnumerator AttackPlayer()
	{
		LookAtPlayer();
		yield return new WaitForSeconds(1f);

		//if(!isStoppedSpinning)
		//{
			StartSpinning();
			ShootAtPlayer();
		//}
			
		yield return new WaitForSeconds(2f);

		StopSpinning();
		yield return new WaitForSeconds(1f);


		//isAttackingPlayer = false;
		
	}

	void UpdateAimVector()
	{
		aimVector = (player.transform.position - spawnBulletPosition.position).normalized;

	}

	void LookAtPlayer()
	{
		//transform.LookAt(player.transform.position);
		Vector3 lookDir = player.transform.position - gameObject.transform.position;
		lookDir.z = 0;
		Quaternion lookRotation = Quaternion.LookRotation(lookDir);
		transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, lookRotation, lookSpeed * Time.deltaTime);
	}

	void StartSpinning()
	{
		transform.Rotate(0, 0, 720* Time.deltaTime ); //rotates 7200 degrees per second around z axis
		isSpinning = true;
	}

	void StopSpinning()
	{
		transform.Rotate(0, 0, 0); //rotates 0 degrees per second around z axis
		isStoppedSpinning = true;
	}

	void ShootAtPlayer()
	{
		if (!isFiring)
			StartCoroutine(FireGun());
	}

	IEnumerator FireGun()
	{
		//Instantiate(bulletPF, spawnBulletPosition.position, Quaternion.LookRotation(aimVector, Vector3.up));
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
