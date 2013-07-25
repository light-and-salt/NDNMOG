using UnityEngine;
using System.Collections;

public class CheckMode : MonoBehaviour {

	public string OnAsteroid = null;
	
	private static Transform asteroidparent;
	private static GameObject Boat;
	
	private static bool start = false;
	
	IEnumerator Start()
	{
		CharacterController controller = GetComponent<CharacterController>();
		Boat =  transform.Find("graphics/boat").gameObject;
		asteroidparent = GameObject.Find("/Asteroid").transform;
		
		while(controller.isGrounded == false)
			yield return new WaitForSeconds(0.1f);
		OnAsteroid = FindNearestAsteroid();
		
		//start = true;
		InvokeRepeating("Check", 0, 0.05F); 
	}
	
//	void Update()
//	{
//		if(start == false)
//			return;
//		
//		Check();
//	}
	
	void Check () {
		
		string newhomeasteroid = FindNearestAsteroid();
		
		if(newhomeasteroid != OnAsteroid)
		{
			print("Change of Mode!");
			ChangeMode(newhomeasteroid, OnAsteroid);
			OnAsteroid = newhomeasteroid;
		}
		
	}
	
	public void ChangeMode(string newhomeast, string oldhomeast)
	{
		
		if(newhomeast == null && oldhomeast != null) // change to fly mode
		{
			Boat.SetActiveRecursively(true);
			move.currentmode = (int)move.Mode.fly;
			asteroidparent.Find(oldhomeast).GetComponent<TreeScript>().DeActivate();
		}
		else if(newhomeast != null && oldhomeast == null) // change to walk mode
		{
			Boat.SetActiveRecursively(false);
			move.currentmode = (int)move.Mode.walk;
			asteroidparent.Find(newhomeast).GetComponent<TreeScript>().Activate();
		}
		else
		{
			print("ChangeMode Error.");
		}
	}
	
	public string FindNearestAsteroid()
	{
		
		string homeasteroid = null;
		float mindistance = 17;
		foreach(Transform a in asteroidparent)
		{
			Transform cap = a.Find("cap");
			float distance = Vector3.Distance(transform.position, cap.position);
			if(distance<=mindistance && transform.position.y > cap.position.y-7)
			{
				mindistance = distance;
				homeasteroid = a.name;
			}
		}
		return homeasteroid;
	}
}
