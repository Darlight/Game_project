using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Spaceflight : MonoBehaviour {

    public int life = 100;
    public int power = 10;
    public bool isplayer; //Boolean for control on the manager
    public bool isEnemy; //Boolean to make diference between good and evil bots
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
    public Transform[] enemies;
    private int index;
    private float toSetIndex = 0.0f;
    public GameObject explosion;
    public AudioSource colSound;
    public AudioSource shotSound;

    void Start() {
        rb = gameObject.GetComponent<Rigidbody>();
        if (isplayer) {
            AudioSource[] audios = GetComponents<AudioSource>();
            shotSound = audios[0];
            colSound = audios[2];
        }
        index = (int)Random.Range(0f, 9f);
    }

    void FixedUpdate() {

        if (stunned > 1f)
            stunned = 1f;
        else if (stunned > 0.85f)
            stunned -= Time.fixedDeltaTime * 0.05f;
        else if (stunned > 0f)
            stunned -= Time.fixedDeltaTime* 0.5f;
        else
            stunned = 0f;


        //if player is true
        if (isplayer)
        {
            ControlHorizontal = Input.GetAxis("Horizontal") * -1;//get controls
            ControlVertical = Input.GetAxis("Vertical");

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
            if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire3"))//If the user press space then the speed will be highter
            {
                MaxSpeed = 200f;
            }
            else if (Input.GetButtonUp("Jump") || Input.GetButtonDown("Fire3"))
            {
                MaxSpeed = 100f;
            }
            if (Input.GetButtonDown("Fire2"))//If the user press space then the speed will be highter
            {
                MaxSpeed = 25f;
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                MaxSpeed = 100f;
            }
            if (Input.GetButtonUp("Fire1"))
            {
                shotSound.Play();
            }
        }
        //if it is a bot, or it is not a player
        else
        {
            changeindex();//Change index for the bot to go to other place
            if (enemies[index])
            {
                RaycastHit hit;
                transform.LookAt(enemies[index].position);
                rb.AddRelativeForce(0, 0, 0.1f,ForceMode.VelocityChange);
                
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
                {
                    if (hit.collider.tag == "agentship" && isEnemy)//Just if it is an enemy
                    {
                        
                        int Objectlife = manager.life;
                        int Objectpower = manager.power;
                        manager.DownLife(Objectpower);
                        if (Objectlife <= 0)//if the object does not have more life
                        {
                            Objectlife = 0;
                            power += 10;//Increment power when user kill someone
                            if (hit.transform)
                            {
                                transform.Find("SecondCamera").gameObject.SetActive(true);
                                Instantiate(explosion, hit.transform.parent.position, hit.transform.parent.rotation);
                                Destroy(hit.transform.parent.gameObject);
                            }
                        }
                    }
                    
                }
            }
            else
            {
                index++;
                if (index >= enemies.Length)
                {
                    index = 0;
                }
            }
        }
    }

    void changeindex()
    {
        toSetIndex = toSetIndex + 0.01f;
        if (toSetIndex >= 1)
        {
            toSetIndex = 0.0f;
            index = (int) Random.Range(0.0f,9.0f);//We set this way because we want them to follow the 10 leaders or follow the player.
        }
    }

    void OnCollisionEnter()
	{
        colSound.Play();
		stunned = 1f;
	}
}
