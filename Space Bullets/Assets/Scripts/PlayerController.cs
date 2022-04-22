using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using StarterAssets;

public class PlayerController : MonoBehaviour
{
	[SerializeField] Transform aimParent;
	[SerializeField] Transform aimSon;
	[SerializeField] Transform targetParent;
	[SerializeField] Transform target;
	[SerializeField] Transform spawnBulletTransform;
	[SerializeField] Transform bulletPF;
	[SerializeField] StarterAssetsInputs input;
	[SerializeField] ImpactReceiver impactReceiver;

	[SerializeField] float mainThrust = 4f;
	[SerializeField] float gunRecoil = 40f;
	[SerializeField] float health = 40;
	[SerializeField] Transform jetPack;

	[SerializeField] ParticleSystem mainEngineParticles;
	[SerializeField] ParticleSystem deathParticles;
	[SerializeField] ParticleSystem bigFireParticles;
	[SerializeField] ParticleSystem smallFireParticles;
	[SerializeField] ParticleSystem sparkParticles;

	[SerializeField] GameObject playerMesh;
	[SerializeField] GameObject gun;
	[SerializeField] GameObject playerCamRoot;
	[SerializeField] GameObject damageIndicatorSprite;
	private Vector3 deathPos;

	[SerializeField] AudioClip thrustAudio;
	[SerializeField] AudioClip gunAudio;
	[SerializeField] AudioSource audioSourceJet;
	[SerializeField] AudioSource audioSourceGun;

	//private Transform playerTransform;

	private bool fireGun = false;
	private bool isFiring = false;

	private bool fireJetPack = false;
	private bool isFiringJetPack = false;

	public bool isDead;
	public bool isImmortal;
	private bool isDying;
	private float startHealth;

	public Vector2 lookInput;
	public Vector3 recoilDir;

	// Start is called before the first frame update
	void Start()
	{
		isDead = false;
		isDying = false;
		startHealth = health;
		damageIndicatorSprite.SetActive(false);
		//playerTransform = gameObject.transform;
		//audioSourceJet = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!isDead)
		{
			//Aiming Gun
			lookInput = input.look;
			if (lookInput != Vector2.zero)
			{
				bool lookingLeft = (gameObject.transform.eulerAngles.y > 269);
				//Debug.Log("lookingleft: " + lookingLeft);
				bool lookingRight = (gameObject.transform.eulerAngles.y < 91);
				//Debug.Log("eulerangles.y: " + gameObject.transform.eulerAngles.y);
				//Debug.Log("lookingRight: " + lookingRight);

				if (lookingLeft && lookInput.x > 0)
				{
					//Debug.Log("Rotate right");
					Vector3 newEulerAngles = new Vector3(0f, 90f, 0f);
					gameObject.transform.eulerAngles = newEulerAngles;
					//aimParent.rotation = Quaternion.Euler(0f, -90f, 0f);
				}

				if (lookingRight && lookInput.x < 0)
				{
					//Debug.Log("Rotate left");
					Vector3 newEulerAngles = new Vector3(0f, 270f, 0f);
					gameObject.transform.eulerAngles = newEulerAngles;
				}

				float inputAngle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg;
				aimParent.rotation = Quaternion.Euler(0, 0, inputAngle);
				Vector3 targetParentPos = new Vector3(aimSon.position.x, aimSon.position.y, targetParent.position.z);
				targetParent.position = targetParentPos;

			}

			//Firing Gun
			if (input.fire || fireJetPack)
			{
				impactReceiver.StartImpact();
			}
			else
			{
				impactReceiver.StopImpact();
			}

			fireGun = input.fire;
			if (fireGun)
			{
				if (!isFiring)
					StartCoroutine(FireGun());

				if (!audioSourceGun.isPlaying)
				{
					audioSourceGun.PlayOneShot(gunAudio);
				}
			}
			else
			{
				audioSourceGun.Stop();
			}

			//Firing jetpack
			fireJetPack = input.jet;
			if (fireJetPack)
			{
				ApplyThrust();
			}
			else
			{
				audioSourceJet.Stop();
				mainEngineParticles.Stop();
			}

			//Setting on fire if dying
			if (isDying)
			{
				if (!bigFireParticles.isPlaying)
					bigFireParticles.Play();
			}
			else
			{
				if (!bigFireParticles.isStopped)
					bigFireParticles.Stop();
			}

		}

		else
		{
			playerCamRoot.transform.position = deathPos;
		}

	}

	IEnumerator FireGun()
	{
		Vector3 aimDir = (target.position - spawnBulletTransform.position).normalized;
		//Instantiate(bulletPF, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));

		GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
		if (bullet != null)
		{
			bullet.GetComponent<BulletProjectile>().EnemyShot(false);
			bullet.transform.position = new Vector3(spawnBulletTransform.position.x, spawnBulletTransform.position.y, 0f);
			bullet.transform.rotation = Quaternion.LookRotation(aimDir, Vector3.up);
			bullet.GetComponent<BulletProjectile>().InstantiateFromPool();
		}

		recoilDir = new Vector3(-aimDir.x, -aimDir.y, 0f);
		impactReceiver.AddImpact(recoilDir, gunRecoil);
		isFiring = true;
		fireGun = false;
		yield return new WaitForSeconds(.1f);
		isFiring = false;
	}

	private void ApplyThrust()
	{
		Vector3 aimDir = (target.position - spawnBulletTransform.position).normalized;
		Vector3 jetVector = new Vector3(aimDir.x, aimDir.y, 0f);
		impactReceiver.AddImpact(jetVector, mainThrust * .1f);

		if (!audioSourceJet.isPlaying)
		{
			audioSourceJet.PlayOneShot(thrustAudio);
		}
		if (!mainEngineParticles.isPlaying)
		{
			mainEngineParticles.Play();
		}

	}

	public void TakeDamage(int dmg)
	{
		health -= dmg;
		Debug.Log("Player Health: " + health);

		Debug.Log("Player Health Ratio: " + health / startHealth);

		if ((health / startHealth) < 1f && (health / startHealth) > 0.5f)
		{
			//Start being mildly on fire
			if (!sparkParticles.isPlaying)
				sparkParticles.Play();
			Debug.Log("Player is mildly hurt");
		}
		else if ((health / startHealth) < 0.5f && (health / startHealth) > 0.2f)
		{
			if (!sparkParticles.isStopped)
				sparkParticles.Stop();

			//Start being medium on fire
			if (!smallFireParticles.isPlaying)
				smallFireParticles.Play();

			Debug.Log("Player is not doing so good");
		}
		else if ((health / startHealth) < 0.2f)
		{
			if (!sparkParticles.isStopped)
				sparkParticles.Stop();
			if (!smallFireParticles.isStopped)
				smallFireParticles.Stop();

			//Start being majorly on fire
			isDying = true;
			Debug.Log("Player is about to die");
		}

		StartCoroutine(IndicateDamage());

		if (health <= 0 && !isImmortal)
			Die();
	}

	IEnumerator IndicateDamage()
	{
		damageIndicatorSprite.SetActive(true);
		yield return new WaitForSeconds(.1f);
		damageIndicatorSprite.SetActive(false);
	}

	private void Die()
	{
		if (!deathParticles.isPlaying && !isDead)
			deathParticles.Play();

		isDead = true;
		deathPos = gameObject.transform.position;

		playerMesh.SetActive(false);
		gun.SetActive(false);
		jetPack.gameObject.SetActive(false);

		if (!sparkParticles.isStopped && sparkParticles != null)
			sparkParticles.Stop();
		if (!smallFireParticles.isStopped && smallFireParticles != null)
			smallFireParticles.Stop();
		if (!bigFireParticles.isStopped && bigFireParticles != null)
			bigFireParticles.Stop();
	}

}
