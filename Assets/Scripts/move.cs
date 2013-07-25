using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {

	public static float speed = 4.0F;
    public static float jumpSpeed = 6.0F;
    public static float gravity = 10F;
	public static float autoflyspeed = 0.5F;
	public static float flyspeedstep = 0.05F;
	public static float flyspeedlimit = 5f;
	
	
	public enum Mode {walk, fly};
	public static int currentmode = (int)Mode.walk;
	
    private Vector3 moveDirection = Vector3.zero;
    
	void Update() {
        CharacterController controller = GetComponent<CharacterController>();
		
		if(currentmode == (int)Mode.walk)
		{
			if (controller.isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("LeftRight"), 0, Input.GetAxis("WalkForBack"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
            
        	}
        	moveDirection.y -= gravity * Time.deltaTime;
        	controller.Move(moveDirection * Time.deltaTime);
		}
		else if(currentmode == (int)Mode.fly)
		{
			
			float newspeed = autoflyspeed + flyspeedstep*Input.GetAxis("FlySpeed");
			if(newspeed>=0 && newspeed<=flyspeedlimit)
			{
				autoflyspeed = newspeed;
			}
			moveDirection = new Vector3(Input.GetAxis("LeftRight"), Input.GetAxis("FlyUpDown"), autoflyspeed);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= 12F;
            
        	controller.Move(moveDirection * Time.deltaTime);
		}
        
    }
}
