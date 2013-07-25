using UnityEngine;
using System.Collections;

public class fishrotate : MonoBehaviour {

	public static float speed = -90f;
	void Update () {
		transform.Rotate(new Vector3(0,speed,0)*Time.deltaTime);
	}
}
