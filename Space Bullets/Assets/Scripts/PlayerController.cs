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
	[SerializeField] Transform spawnBulletPosition;
	[SerializeField] Transform bulletPF;
	[SerializeField] StarterAssetsInputs input;
	[SerializeField] ImpactReceiver impactReceiver;

	[SerializeField] float mainThrust = 4f;
	[SerializeField] float gunRecoil = 40f;
	[SerializeField] Transform jetPack;
	[SerializeField] ParticleSystem mainEngineParticles;
	[SerializeField] AudioClip thrustAudio;
	[SerializeField] AudioClip gunAudio;

	[SerializeField] AudioSource audioSourceJet;
	[SerializeField] AudioSource audioSourceGun;
	private bool fireGun = false;
	private bool isFiring = false;

	private bool fireJetPack = false;
	private bool isFiringJetPack = false;

	public Vector2 lookInput;
	public Vector3 recoilDir;

	// Start is called before the first frame update
	void Start()
	{
		//audioSourceJet = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		//Aiming Gun
		lookInput = input.look;
		if(lookInput != Vector2.zero)
		{
			bool lookingLeft = (gameObject.transform.eulerAngles.y > 269);
			Debug.Log("lookingleft: " + lookingLeft);
			bool lookingRight = (gameObject.transform.eulerAngles.y < 91);
			//Debug.Log("eulerangles.y: " + gameObject.transform.eulerAngles.y);
			Debug.Log("lookingRight: " + lookingRight);

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
		if(input.fire || fireJetPack)
		{
			impactReceiver.StartImpact();
		}
		else
		{
			impactReceiver.StopImpact();
		}

		fireGun = input.fire;
		if(fireGun)
		{
			if(!isFiring)
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
		if(fireJetPack)
		{
			ApplyThrust();
		}
		else
		{
			audioSourceJet.Stop();
			mainEngineParticles.Stop();
		}

	}

	IEnumerator FireGun()
	{
		Vector3 aimDir = (target.position - spawnBulletPosition.position).normalized;
		//Instantiate(bulletPF, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));

		GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
		if (bullet != null)
		{
			bullet.transform.position = spawnBulletPosition.position;
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
		Vector3 jetVector = new Vector3(jetPack.up.x, jetPack.up.y, 0f);
		impactReceiver.AddImpact(jetVector,mainThrust);

		if (!audioSourceJet.isPlaying)
		{
			audioSourceJet.PlayOneShot(thrustAudio);
		}
		if(!mainEngineParticles.isPlaying)
		{
			mainEngineParticles.Play();
		}
		
	}
}
