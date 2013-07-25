using UnityEngine;
using System.Collections;

public class switchView : MonoBehaviour {

	private Transform cameraT;
	
	void Start()
	{
		cameraT = transform.Find("myCamera");
		if(!cameraT)
		{
			print("player does not have a camera named 'myCamera'. ");
		}
		cameraT.localPosition = new Vector3(0f, 1f, -3f); // initialize: 3rd person view
	}
	
	void Update () {
	
		if (Input.GetKeyDown ("3")) // 3rd person view
		{
			cameraT.localPosition = new Vector3(0f, 1f, -3f);
		}
		
		if (Input.GetKeyDown ("1")) // 1st person view
		{
			cameraT.localPosition = new Vector3(0f, 0.4f, 0f);
		}
		
	}
}
