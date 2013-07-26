using UnityEngine;
using System.Collections;

/* Player to NPC interation under development by wzh.
 * Find nearest NPC4 and debug.log if its distance is within 5.
 * 
 * July 25th, 2013
 */

public class P2NPC_Interaction : MonoBehaviour {
	
	private Transform targetNPC;
	private Transform parent;
	
	int step=0;
	
	public Transform FindNearestNPC4()
	{
		Transform nearestNPC4 = null;
		float mindist = 10000;
		float dist = 0;
		foreach (Transform t in parent)
		{
			Transform temp = t.Find("npc4");
			dist = Vector3.Distance(temp.position,transform.position);
			if (dist<mindist)
			{
				mindist=dist;
				nearestNPC4=temp;
			}
		}
		return nearestNPC4;
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		step++;
		if (step==60)
		{
			parent = GameObject.Find("/Asteroid").transform;
			Transform nearestNPC4 = FindNearestNPC4();
			Debug.Log("Nearest is " + Vector3.Distance(nearestNPC4.position,transform.position));
			if (Vector3.Distance(nearestNPC4.position,transform.position)<5)
			{
				nearestNPC4.gameObject.GetComponent<NPC4_Control>().showGUIText();
			}
			else
			{
				
			}
			step=0;
		}
	}
	
	void OnGUI()
	{
	}
}
