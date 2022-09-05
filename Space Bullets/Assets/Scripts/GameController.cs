using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	[SerializeField] GameObject player;

	[SerializeField] GameObject xRightBorder;
	[SerializeField] GameObject xLeftBorder;

	[SerializeField] GameObject yGroundBorder;
	[SerializeField] GameObject ySkyBorder;
	[SerializeField] GameObject ySpaceBorder;
	[SerializeField] GameObject yPlanetBorder;

	public int points;
	public int skyHeight = 1000;
	public int spaceHeight = 3000;
	public int planetHeight = 10000;
	public int playerHeight;
	public int yOffset;
	public Vector3 playerPos;


	public float groundTop;
	public float skyTop;
	public float spaceTop;

	//Border flags
	bool skyStartFlag;
	bool skyEndFlag;

	public enum HeightState
	{
		Ground,
		Sky,
		Space,
		Planet
	}

	public HeightState heightState;

	// Start is called before the first frame update
	void Start()
    {
		points = 0;
		playerHeight = 0;
		yOffset = 0;
		playerPos = player.transform.position;

		//Initialize border flags
		skyStartFlag = false;

		heightState = HeightState.Ground;

		groundTop = yGroundBorder.transform.position.y;
		skyTop = ySkyBorder.transform.position.y;
		spaceTop = ySpaceBorder.transform.position.y;
	}

    // Update is called once per frame
    void Update()
    {
		playerHeight = (int)(player.transform.position.y + yOffset);
		playerPos = player.transform.position;
		CheckIfPlayerHitBorder();
	}

	public void KillEnemy(GameObject enemy)
	{
		Destroy(enemy);
		AwardPoints();
	}

	void AwardPoints()
	{
		points += 100;
		
	}

	public GameObject GetPlayer()
	{
		return player;
	}

	public void CheckIfPlayerHitBorder()
	{
		//Lateral Border detection works
		if (playerPos.x > xRightBorder.transform.position.x)
		{
			Vector3 leftPos = new Vector3(xLeftBorder.transform.position.x + 10f, player.transform.position.y, player.transform.position.z);
			player.transform.position = leftPos;
			Debug.Log("moving player to left border" + leftPos);
		}
		else if (playerPos.x < xLeftBorder.transform.position.x)
		{
			Vector3 rightPos = new Vector3(xRightBorder.transform.position.x - 10f, player.transform.position.y, player.transform.position.z);
			player.transform.position = rightPos;
			Debug.Log("moving player to right border" + rightPos);
		}

		//Border Logic
		//GroundBox
		//When player hits groundbox top border
		if (playerPos.y >= groundTop && playerPos.y <= skyTop && heightState == HeightState.Ground)
		{
			heightState = HeightState.Sky;
		}

		//SkyBox
		//When player hits skybox bottom border FROM SKY
		else if (playerPos.y <= groundTop && heightState == HeightState.Sky)
		{
			if (playerHeight > skyHeight)
			{
				player.transform.position = new Vector3(playerPos.x, skyTop, playerPos.z);
				float diff = skyTop - groundTop;
				yOffset -= (int)diff;
			}
			else
			{
				heightState = HeightState.Ground;
			}
		}
		//When player hits skybox top border FROM SKY
		else if (playerPos.y >= skyTop && playerPos.y <= spaceTop && heightState == HeightState.Sky)
		{
			if(playerHeight < spaceHeight)
			{
				player.transform.position = new Vector3(playerPos.x, groundTop, playerPos.z);
				float diff = skyTop - groundTop;
				yOffset += (int)diff;
			}
			else
			{
				heightState = HeightState.Space;
			}
		}
		
		//SpaceBox
		//When player hits spacebox bottom border FROM SPACE
		else if (playerPos.y <= skyTop && heightState == HeightState.Space)
		{
			if (playerHeight > spaceHeight)
			{
				player.transform.position = new Vector3(playerPos.x, spaceTop, playerPos.z);
				float diff = spaceTop - skyTop;
				yOffset -= (int)diff;
			}
			else
			{
				heightState = HeightState.Sky;
			}
		}
		//When player hits spacebox top border FROM SPACE
		else if (playerPos.y >= spaceTop && heightState == HeightState.Space)
		{
			if (playerHeight < planetHeight)
			{
				player.transform.position = new Vector3(playerPos.x, skyTop, playerPos.z);
				float diff = spaceTop - skyTop;
				yOffset += (int)diff;
			}
			else
			{
				heightState = HeightState.Planet;
			}
		}

		//PlanetBox
		//player hits planetbox bottom border FROM PLANET
		else if(playerPos.y <= spaceTop && heightState == HeightState.Planet)
		{
			heightState = HeightState.Space;
		}
	}
}
