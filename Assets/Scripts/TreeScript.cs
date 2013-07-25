using UnityEngine;
using System.Collections;

public class TreeScript : MonoBehaviour {
	
	/* Activate() & DeActivate() are for performance
	*  	When player lands on an asteroid,
	*	that asteroid should call Activate(),
	*	which enables everything on the asteroid:
	*	colliders, scripts, animations ect.
	*	When player leaves an asteroid,
	*	everything except renderer is DeActivate().
	*	This largely saves game frame rate.
	*/
	
	public void Activate()
	{
		//print("Activate " + transform.name);
		transform.Find("cap").collider.enabled = true;
		transform.Find("npc8/body").collider.enabled = true;
	}
	
	public void DeActivate()
	{
		//print("DeActivate " + transform.name);
		transform.Find("cap").collider.enabled = false;
		transform.Find("npc8/body").collider.enabled = false;
	}
}
