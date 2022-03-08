using UnityEngine;

using System.Collections;

public class ImpactReceiver : MonoBehaviour
{
	float mass = 3.0F; // defines the character mass
	public Vector3 impact = Vector3.zero;
	private CharacterController character;


	public float zPos;

	// Use this for initialization
	void Start()
	{
		character = GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
	{
		//Debug
		zPos = this.gameObject.transform.position.z;

		// apply the impact force:
		if (impact.magnitude > 0.2F) character.Move(impact * Time.deltaTime);
		// consumes the impact energy each cycle:
		impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

		//Resets z axis
		if (zPos > 1f || zPos < -1f)
		{
			Debug.Log("reset z");
			Vector3 resetZ = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0f);
			this.gameObject.transform.position = resetZ;
		}

		Debug.Log("impact: " + impact);
	}
	// call this function to add an impact force:
	public void AddImpact(Vector3 dir, float force)
	{
		dir.Normalize();
		//if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
		impact += dir.normalized * force / mass;

		
	}
}