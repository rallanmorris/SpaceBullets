using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	[SerializeField] GameObject player;

	public int points;
	public int skyHeight = 2000;
	public int spaceHeight = 5000;
	public int planetHeight = 10000;
	public int playerHeight;
	public int yOffset;

    // Start is called before the first frame update
    void Start()
    {
		points = 0;
		playerHeight = 0;
		yOffset = 0;
    }

    // Update is called once per frame
    void Update()
    {
		playerHeight = (int)(player.transform.position.y + yOffset);
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
}
