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
	
	private int step=0;
	
	public Ray viewRay;
	private Vector3 viewDirection;
	
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
		viewDirection = new Vector3(0.5f,0.5f,0f);
	}
	
	// Update is called once per frame
	void Update () {
		
		viewRay = Camera.main.ViewportPointToRay(viewDirection);
			//transform.Find("myCamera").gameObject.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f,0.5f,0f));
		RaycastHit hit;
		//\player and \tree2\cap and \tree2\npc4\leader are set to ignore raycast layer, for easy testing
		
		//it would be great if capsulecast only hits objects with a collider. And it would also be good 
		//if the actual shape of the cast can be painted...looks like it's one layer lower than the mainscene picture now
				
		//a third try would be to use collider.RayCast written in NPC4_Control...
		//But I failed to find a method for detecting whether an object is being casted by a ray
		
		//to use OnMouseEnter in NPC4_Control and detect distance? The concept of a host of a NPC?
		
		
		//my original idea is to use raycast, but it turns out that capsule casts look like better choices,
		//evaluations are still needed though.
		
		//if (Physics.Raycast(viewRay,out hit,5)==true)
		
		//if (Physics.CapsuleCast(transform.position,transform.forward,1,transform.forward,out hit,5))
		
		if (Physics.CapsuleCast(transform.position,transform.forward,1,transform.forward,out hit,5))
		{
			//debug message : what's hit by capsulecast
			//Debug.Log("Hit "+hit.transform.gameObject.name);	
			if (hit.transform.gameObject.name == "npc4")
			{
				if (hit.transform.gameObject.GetComponent<NPC4_Control>().status == NPC4_Control.state.free)
				{
					hit.transform.gameObject.GetComponent<NPC4_Control>().showGUIText();
				}
			}
		}
		
		//My earlier approach
		/*
		step++;
		if ( step==60 )
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
			step = 0;
		}
		*/
	}
	
	void OnGUI()
	{
	}
}
