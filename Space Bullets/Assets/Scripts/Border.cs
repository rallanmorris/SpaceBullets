using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Border : MonoBehaviour
{
	[SerializeField] Border leftBorder;
	[SerializeField] Border rightBorder;
	[SerializeField] Border topBorderLowerSky;
	[SerializeField] Border topBorderUpperSky;
	[SerializeField] Border topBorderSpace;
	[SerializeField] GameController gameController;

	public bool isLeftBorder;
	public bool isRightBorder;
	public bool isTopBorderLowerSky;
	public bool isTopBorderUpperSky;
	public bool isTopBorderSpace;

	public float leftX;
	public float rightX;
	public float lowerY;
	public float upperY;
	public float spaceY;

	private bool borderHit;
	private bool enableCharacterControllerFlag;
	private Transform playerTransform;
	private CharacterController characterController;
	private PlayerController playerController;

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

		else if (isTopBorderLowerSky)
		{
			lowerY = gameObject.transform.position.y;
			upperY = topBorderUpperSky.gameObject.transform.position.y;
		}

		else if (isTopBorderUpperSky)
		{
			upperY = gameObject.transform.position.y;
			lowerY = topBorderLowerSky.gameObject.transform.position.y;
		}

		else if (isTopBorderSpace)
		{
			spaceY = gameObject.transform.position.y;
			upperY = topBorderUpperSky.gameObject.transform.position.x;
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
				playerTransform.position = rightPos;
				Debug.Log("moving player to right border" + rightPos);
			}

			else if (isRightBorder)
			{
				Vector3 leftPos = new Vector3(leftX + 10f, playerTransform.position.y, playerTransform.position.z);
				playerTransform.position = leftPos;
				Debug.Log("moving player to left border" + leftPos);
			}

			else if (isTopBorderLowerSky)
			{
				if(playerController.GetZone() == PlayerController.Zone.LowerSky)
				{
					playerController.SetZone(PlayerController.Zone.UpperSky);
				}
				else if(playerController.GetZone() == PlayerController.Zone.UpperSky)
				{
					if (gameController.playerHeight < upperY)
						playerController.SetZone(PlayerController.Zone.LowerSky);
					else if (gameController.playerHeight > upperY)
					{
						Vector3 topPos = new Vector3(playerTransform.position.x, upperY - 10f, playerTransform.position.z);
						playerTransform.position = topPos;
						gameController.yOffset -= 990;
						Debug.Log("moving player to upper sky border" + topPos);
					}
				}
			}

			else if (isTopBorderUpperSky)
			{
				if (playerController.GetZone() == PlayerController.Zone.UpperSky)
				{
					if(gameController.playerHeight >= gameController.spaceHeight)
						playerController.SetZone(PlayerController.Zone.Space);
					else if(gameController.playerHeight < gameController.spaceHeight)
					{
						Vector3 topPos = new Vector3(playerTransform.position.x, lowerY + 10f, playerTransform.position.z);
						playerTransform.position = topPos;
						gameController.yOffset += 990;
						Debug.Log("moving player to lower sky border" + topPos);
					}
				}
				else if (playerController.GetZone() == PlayerController.Zone.Space)
				{
					if (gameController.playerHeight <= gameController.skyHeight)
						playerController.SetZone(PlayerController.Zone.UpperSky);
					else if (gameController.playerHeight > gameController.skyHeight)
					{
						Vector3 topPos = new Vector3(playerTransform.position.x, spaceY - 10f, playerTransform.position.z);
						playerTransform.position = topPos;
						gameController.yOffset -= 990;
						Debug.Log("moving player to space border" + topPos);
					}
				}
			}

			else if (isTopBorderSpace)
			{
				if (playerController.GetZone() == PlayerController.Zone.Space)
				{
					if (gameController.playerHeight < gameController.planetHeight)
					{
						Vector3 topPos = new Vector3(playerTransform.position.x, upperY + 10f, playerTransform.position.z);
						playerTransform.position = topPos;
						gameController.yOffset += 990;
						Debug.Log("moving player to upper sky border" + topPos);
					}
				}
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
			playerController = other.GetComponent<PlayerController>();
			characterController = other.GetComponent<CharacterController>();
			playerTransform = characterController.gameObject.transform;
			
			characterController.enabled = false;
		}
	}
}
