using UnityEngine;
using System.Collections;

public class FishSwim : MonoBehaviour {

	
	public Vector3 target = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	
	void Update () {
		if(target == Vector3.zero)
			return;
		
		Vector3 relativePos = target - transform.position;
		relativePos.y = 0;
        transform.rotation = Quaternion.LookRotation(relativePos);
		transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 60f);
		
	}
	
}
