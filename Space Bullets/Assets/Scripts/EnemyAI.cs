using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	[SerializeField] GameObject player;
	[SerializeField] PlayerController playerController;
	[SerializeField] Transform spawnBulletPosition;
	[SerializeField] Transform bulletPF;
	private Vector3 aimVector;
	private bool isFiring = false;

	public float distanceToPlayer;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (GetDistanceToPlayer() < 10)
		{
			UpdateAimVector();
			LookAtPlayer();
			ShootAtPlayer();
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
	}

	void LookAtPlayer()
	{
		transform.LookAt(player.transform.position);
	}

	void ShootAtPlayer()
	{
		if (!isFiring)
			StartCoroutine(FireGun());
	}

	IEnumerator FireGun()
	{
		Instantiate(bulletPF, spawnBulletPosition.position, Quaternion.LookRotation(aimVector, Vector3.up));
		isFiring = true;
		yield return new WaitForSeconds(.1f);
		isFiring = false;
	}

	public void TakeDamage(int dmg)
    {

    }
}
