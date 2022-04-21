using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidBody;
    //[SerializeField] private Transform vfxHitEnemy;
    //[SerializeField] private Transform vfxHitOther;
    [SerializeField] float speed = 5f;
    [SerializeField] int damage = 1;
	private bool isEnemyBullet = false;

	private EnemyAI enemy;
	private PlayerController player;

    private void Awake()
    {
        bulletRigidBody = GetComponent<Rigidbody>();

    }

    private void Start()
    {

    }

	public void EnemyShot(bool isEnemyShot)
	{
		isEnemyBullet = isEnemyShot;
	}

	private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletTarget>() != null)
        {
			//Hit enemy
			Debug.Log("hit an enemy");

			
            enemy = other.gameObject.GetComponent<EnemyAI>();
            //var hitEffect = Instantiate(vfxHitEnemy, transform.position, Quaternion.identity);
            //hitEffect.transform.parent = enemy.transform;
            //Destroy(hitEffect.gameObject, 0.2f);

            if (enemy != null && !isEnemyBullet)
			{
                enemy.TakeDamage(damage);
				gameObject.SetActive(false);
			}

		}
		else if(other.GetComponent<BulletProjectile>() != null)
		{
			//Hit another bullet
			Debug.Log("hitting another bullet");
		}
		else if(other.GetComponent<PlayerController>() != null)
		{
			Debug.Log("hit player");

			player = other.gameObject.GetComponent<PlayerController>();
			if (player != null && isEnemyBullet)
			{
				player.TakeDamage(damage);
				gameObject.SetActive(false);
			}
		}
        else
        {
			//Hit something else
			Debug.Log("hitting " + other.tag);
			gameObject.SetActive(false);
			/*
            
            var missEffect = Instantiate(vfxHitOther, transform.position, Quaternion.identity);
            Destroy(missEffect.gameObject, 0.5f);
			*/
		}

		//Debug.Log("Hit Something");
        //gameObject.SetActive(false);
	}

	public void InstantiateFromPool()
	{
		Vector3 bulletTrajectory = new Vector3(transform.forward.x, transform.forward.y, 0f);
		bulletRigidBody.velocity = bulletTrajectory * speed;
		gameObject.SetActive(true);
		StartCoroutine(WaitThenDestroy());
	}

    IEnumerator WaitThenDestroy()
    {
        yield return new WaitForSeconds(5f);
        if(gameObject.activeSelf == true)
		{
			gameObject.SetActive(false);
		}

	}
}
