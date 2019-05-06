using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CSharp;

public class Spaceflight : MonoBehaviour {

    public bool isplayer; //Boolean for control on the manager
    public bool shooter; //Boolean for the movement of the bots, if truee ship will just shoot to the chargers or spacefrigates
    public Transform hull;
    public float maxTilt = 20f;
    public float MaxSpeed = 60f;
    public float MaxAngularAcceleration = 30f;
    public float MaxAcceleration = 4f;
    public float TurnFactor = 0f;
    internal float ControlHorizontal;
    internal float ControlVertical;
    internal float ControlThrust = 1f;
    internal float stunned = 0f;
    private Rigidbody rb;
    public Rigidbody[] enemies;
    public int index = 0;
    private float toSetIndex = 0.0f;
    private float currentRandom = 0;
    private int lastIndex = 0;


    void Start() {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate() {

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
            ControlHorizontal = Input.GetAxis("Horizontal") * -1;
            ControlVertical = Input.GetAxis("Vertical");

            GameObject player = GameObject.Find("Camera");
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

            Vector3 vDiff = transform.forward * MaxSpeed * ControlThrust - rb.velocity;
            if (vDiff.magnitude > MaxAcceleration * (1f - stunned))
                vDiff *= MaxAcceleration * (1f - stunned) / vDiff.magnitude;
            rb.AddForce(vDiff, ForceMode.VelocityChange);

            Vector3 avdiff = -1 * (TurnFactor * (transform.up * ControlHorizontal + transform.right * ControlVertical) + rb.angularVelocity);
            float mag = avdiff.magnitude;
            avdiff.Normalize();
            rb.AddTorque(avdiff * Mathf.Clamp(mag, 0, MaxAngularAcceleration * Time.fixedDeltaTime * (1f - stunned)), ForceMode.VelocityChange);

            hull.localRotation = Quaternion.Euler(0f, ControlHorizontal * -1f, 0f);
            if (Input.GetButtonDown("Jump"))
            {
                MaxSpeed = 150f;
                Vector3 newPos = player.transform.localPosition;
                newPos.y += 50;
            }
            else if (Input.GetButtonUp("Jump"))
            {
                MaxSpeed = 60f;
                Vector3 newPos = player.transform.localPosition;
                newPos.y -= 50;
            }
        }

        //if shooter is true
        else if (shooter)
        {
            changeindex();
            Vector3 position = enemies[index].transform.position;
        }
        else
        {
            ControlHorizontal = generateRandom();
            ControlVertical = generateRandom();

            GameObject player = GameObject.Find("Camera");
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

            Vector3 vDiff = transform.forward * MaxSpeed * ControlThrust - rb.velocity;
            if (vDiff.magnitude > MaxAcceleration * (1f - stunned))
                vDiff *= MaxAcceleration * (1f - stunned) / vDiff.magnitude;
            rb.AddForce(vDiff, ForceMode.VelocityChange);

            Vector3 avdiff = -1 * (TurnFactor * (transform.up * ControlHorizontal + transform.right * ControlVertical) + rb.angularVelocity);
            float mag = avdiff.magnitude;
            avdiff.Normalize();
            rb.AddTorque(avdiff * Mathf.Clamp(mag, 0, MaxAngularAcceleration * Time.fixedDeltaTime * (1f - stunned)), ForceMode.VelocityChange);

            hull.localRotation = Quaternion.Euler(0f, ControlHorizontal * -1f, 0f);
            MaxSpeed = 15;
        }
    }

    float generateRandom()
    {
        changeindex();
        if (index == 0)
        {
            return 0;
        }
        else
        {
            if (lastIndex != index)
            {
                currentRandom = Random.Range(-1f, 1f);
            }                   
        }
        return currentRandom;
    }

    void changeindex()
    {
        toSetIndex = toSetIndex + 0.001f;
        if (toSetIndex >= enemies.Length)
        {
            toSetIndex = 0.0f;
        }
        int newIndex = (int)toSetIndex;
        if(newIndex != index)
        {
            lastIndex = index;
        }
        index = newIndex;
    }

    void OnCollisionEnter()
	{
		stunned = 1f;
	}
}
