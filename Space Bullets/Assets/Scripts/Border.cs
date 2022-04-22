using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Border : MonoBehaviour
{
	[SerializeField] Border leftBorder;
	[SerializeField] Border rightBorder;
	[SerializeField] CinemachineVirtualCamera virtCam;

	public bool isLeftBorder;
	public bool isRightBorder;

	public float leftX;
	public float rightX;

	private bool borderHit;
	private bool enableCharacterControllerFlag;
	private Transform playerTransform;
	private CharacterController characterController;

	// Start is called before the first frame update
	void Start()
    {
		if(isLeftBorder)
		{
			leftX = gameObject.transform.position.x;
			rightX = rightBorder.gameObject.transform.position.x;
		}

		else if(isRightBorder)
		{
			rightX = gameObject.transform.position.x;
			leftX = leftBorder.gameObject.transform.position.x;
		}

		borderHit = false;
	}

    void Update()
    {
        if(borderHit)
		{
			if (isLeftBorder)
			{
				Vector3 rightPos = new Vector3(rightX - 10f, playerTransform.position.y, playerTransform.position.z);
				Vector3 delta = rightPos - playerTransform.position;
				playerTransform.position = rightPos;

				virtCam.OnTargetObjectWarped(playerTransform, delta);
				Debug.Log("moving player to right border" + rightPos);
			}

			else if (isRightBorder)
			{
				Vector3 leftPos = new Vector3(leftX + 10f, playerTransform.position.y, playerTransform.position.z);
				playerTransform.position = leftPos;
				Debug.Log("moving player to left border" + leftPos);
			}

			borderHit = false;
			characterController.enabled = true;
			//enableCharacterControllerFlag = true;
		}

		if(enableCharacterControllerFlag && characterController != null)
		{
			characterController.enabled = true;
			enableCharacterControllerFlag = false;
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PlayerController>() != null)
		{
			Debug.Log("hit Border");
			borderHit = true;
			characterController = other.GetComponent<CharacterController>();
			playerTransform = characterController.gameObject.transform;

			characterController.enabled = false;
		}
	}
}
