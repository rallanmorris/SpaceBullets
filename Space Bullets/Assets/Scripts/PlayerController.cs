using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform spawnBulletPosition;
    [SerializeField] Transform bulletPF;
    [SerializeField] StarterAssetsInputs input;

    private bool fireGun = false;
    private bool isFiring = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //Firing Gun
        fireGun = input.fire;
        if(fireGun && !isFiring)
        {
            StartCoroutine(FireGun());
        }

    }

    IEnumerator FireGun()
    {
        Vector3 aimDir = (target.position - spawnBulletPosition.position).normalized;
        Instantiate(bulletPF, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        isFiring = true;
        fireGun = false;
        yield return new WaitForSeconds(.1f);
        isFiring = false;
    }
}
