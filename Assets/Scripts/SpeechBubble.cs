using UnityEngine;
using System.Collections;

public class SpeechBubble : MonoBehaviour {
	
	public Transform target;  
	public Vector3 offset = new Vector3(0f, 1.5f, 0f);  
	public Camera cameraToUse;
	
	// Update is called once per frame
	void Update () {
	
		transform.position = cameraToUse.camera.WorldToViewportPoint(target.position + offset);
	}
} 