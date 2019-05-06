using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceflight : MonoBehaviour {


	public Transform hull; 
	public float maxTilt = 20f; 
	public float MaxSpeed = 40f; 
	public float MaxAngularAcceleration = 30f; 
	public float MaxAcceleration = 4f; 
	public float TurnFactor = 0f; 
	internal float ControlHorizontal;
	internal float ControlVertical; 
	internal float ControlThrust = 1f; 
	internal float stunned = 0f;
	private Rigidbody rb;
 

	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
	}

    void FixedUpdate () {

		if (stunned > 1f)
			stunned = 1f; 
		else if (stunned > 0.85f)
			stunned -= Time.fixedDeltaTime * 0.05f; 
		else if (stunned > 0f)
			stunned -= Time.fixedDeltaTime * 0.5f; 
		else
			stunned = 0f; 

        
		ControlHorizontal = Input.GetAxis ("Horizontal")*-1;
		ControlVertical = Input.GetAxis ("Vertical");

        GameObject player = GameObject.Find("Camera");
        Quaternion currRotatation = player.transform.localRotation;
        if (ControlHorizontal>0)
        {
            currRotatation.z += 0.0005f;
            if(currRotatation.z > 0.05f)
            {
                currRotatation.z = 0.05f;
            }
        }
        else if(ControlHorizontal<0)
        {
            currRotatation.z -= 0.0005f;
            if (currRotatation.z < -0.05f)
            {
                currRotatation.z = -0.05f;
            }
        }
        else
        {
            if (currRotatation.z > 0)
            {
                currRotatation.z -= 0.0009f;
                if (currRotatation.z < 0f)
                {
                    currRotatation.z = 0f;
                }
            }
            else if (currRotatation.z < 0)
            {
                currRotatation.z += 0.0009f;
                if (currRotatation.z > 0f)
                {
                    currRotatation.z = 0f;
                }
            }
        }



        player.transform.localRotation = currRotatation;

        Vector3 vDiff = transform.forward * MaxSpeed * ControlThrust - rb.velocity; 
		if (vDiff.magnitude > MaxAcceleration * (1f - stunned))
			vDiff *= MaxAcceleration * (1f - stunned) / vDiff.magnitude;
		rb.AddForce (vDiff , ForceMode.VelocityChange);

		Vector3 avdiff = -1 * (TurnFactor * (transform.up * ControlHorizontal + transform.right * ControlVertical) + rb.angularVelocity); 
        float mag = avdiff.magnitude;
		avdiff.Normalize (); 
		rb.AddTorque (avdiff * Mathf.Clamp (mag, 0, MaxAngularAcceleration * Time.fixedDeltaTime  * (1f - stunned)), ForceMode.VelocityChange);

		hull.localRotation = Quaternion.Euler (0f, ControlHorizontal * -1f, 0f);
	}

	void OnCollisionEnter()
	{
		stunned = 1f;
	}
}
