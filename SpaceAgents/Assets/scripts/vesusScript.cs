using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vesusScript : MonoBehaviour {

    public int life = 100;
    public int power = 10;
    public bool isplayer; //Boolean for control on the manager
    public Transform hull;
    public float maxTilt = 20f;
    public float MaxSpeed = 100f;
    public float MaxAngularAcceleration = 30f;
    public float MaxAcceleration = 4f;
    public float TurnFactor = 0f;
    internal float ControlHorizontal;
    internal float ControlVertical;
    internal float ControlThrust = 1f;
    internal float stunned = 0f;
    private Rigidbody rb;
    public GameObject explosion;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        if (stunned > 1f)
            stunned = 1f;
        else if (stunned > 0.85f)
            stunned -= Time.fixedDeltaTime * 0.05f;
        else if (stunned > 0f)
            stunned -= Time.fixedDeltaTime * 0.5f;
        else
            stunned = 0f;


        //if player is true
        if (isplayer)
        {
            ControlHorizontal = Input.GetAxis("HorizontalPlayer1") * -1;//get controls
            ControlVertical = Input.GetAxis("VerticalPlayer1");
            

            GameObject player = GameObject.Find("Camera");//This will be helpful to move a little the camara when the user moves to the left or right
            if (player)
            {
                Quaternion currRotatation = player.transform.localRotation;
                if (ControlHorizontal > 0)
                {
                    currRotatation.z += 0.0005f;
                    if (currRotatation.z > 0.05f)
                    {
                        currRotatation.z = 0.05f;
                    }
                }
                else if (ControlHorizontal < 0)
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
            }
            Vector3 vDiff = transform.forward * MaxSpeed * ControlThrust - rb.velocity;//Add a force for the ship to move
            if (vDiff.magnitude > MaxAcceleration * (1f - stunned))
                vDiff *= MaxAcceleration * (1f - stunned) / vDiff.magnitude;
            rb.AddForce(vDiff, ForceMode.VelocityChange);

            Vector3 avdiff = -1 * (TurnFactor * (transform.up * ControlHorizontal + transform.right * ControlVertical) + rb.angularVelocity);
            float mag = avdiff.magnitude;
            avdiff.Normalize();
            rb.AddTorque(avdiff * Mathf.Clamp(mag, 0, MaxAngularAcceleration * Time.fixedDeltaTime * (1f - stunned)), ForceMode.VelocityChange);//Add a torque in the direction that the user is setting with the inputs

            if (hull)
            {
                hull.localRotation = Quaternion.Euler(0f, ControlHorizontal * -1f, 0f);
            }
            if (Input.GetButtonDown("Submit"))//If the user press space then the speed will be highter
            {
                MaxSpeed = 400f;
            }
            else if (Input.GetButtonUp("Submit"))
            {
                MaxSpeed = 100f;
            }
        }
    }

    void OnCollisionEnter()
    {
        stunned = 1f;
    }
}
