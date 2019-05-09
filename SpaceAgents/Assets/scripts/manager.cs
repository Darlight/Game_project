using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manager : MonoBehaviour
{

    public static int Objectlife = 100;//Life of the enemy that the raycast is pointing to
    public static int Objectpower = 100;//power of the enemy that the raycas is pointing to
    public static int life = 1000;
    public static int power = 0;
    public GameObject explosion;
    public int enemies = 50;
    public Texture TargetTexture;
    public int range;
    public int multiplier;
    public GameObject[] pointers;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Camera.main)
        {
            Ray myRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
            RaycastHit hitInfo;
            GameObject player = GameObject.Find("PlayerShip");
            power = player.GetComponent<Spaceflight>().power;
            life = player.GetComponent<Spaceflight>().life;
            transform.Find("ownLife").GetComponent<UnityEngine.UI.Text>().text = "LIFE: " + (int)(player.GetComponent<Spaceflight>().life / 1000);//get user life
            transform.Find("ownPower").GetComponent<UnityEngine.UI.Text>().text = "POWER: " + player.GetComponent<Spaceflight>().power;//get user power

            if (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0))//OnClick
            {
                if (Physics.Raycast(myRay, out hitInfo))
                {
                    if (hitInfo.collider.tag == "destructor")//Just if it is an enemy
                    {
                        Objectlife = hitInfo.transform.parent.GetComponent<Spaceflight>().life;
                        Objectpower = hitInfo.transform.parent.GetComponent<Spaceflight>().power;
                        hitInfo.transform.parent.GetComponent<Spaceflight>().life = Objectlife - player.GetComponent<Spaceflight>().power;
                        if (Objectlife <= 0)//if the object does not have more life
                        {
                            Objectlife = 0;
                            player.GetComponent<Spaceflight>().power += 10;//Increment power when user kill someone
                            Instantiate(explosion, hitInfo.transform.parent.position, hitInfo.transform.parent.rotation);
                            Destroy(hitInfo.transform.parent.gameObject);
                            enemies--;
                        }
                        transform.Find("life").GetComponent<UnityEngine.UI.Text>().text = "|" + Objectlife + "|";//show on screen the life of the enemy that the user is poing to
                    }
                    else
                    {
                        transform.Find("life").GetComponent<UnityEngine.UI.Text>().text = "";
                    }
                }
            }
            if (player.GetComponent<Spaceflight>().life <= 0)
            {
                transform.Find("life").GetComponent<UnityEngine.UI.Text>().text = "GAME OVER";
            }
            else if (enemies == 0)
            {
                transform.Find("life").GetComponent<UnityEngine.UI.Text>().text = "VICTORY\n ENEMIES HAVE DECLARED WITHDRAWAL";
            }
        }


    }

    public static void DownLife(int power)
    {
        GameObject player = GameObject.Find("PlayerShip");
        player.GetComponent<Spaceflight>().life -= power;
        if (player.GetComponent<Spaceflight>().life <= 0)
        {
            player.GetComponent<Spaceflight>().life = 0;
        }
    }




    private void OnGUI()
    {
        pointers = GameObject.FindGameObjectsWithTag("destructor");

        for (int i = 0; i < pointers.Length; i++)
        {
            var dist = Vector3.Distance(transform.position, pointers[i].transform.position);
            if (pointers[i].GetComponent<Renderer>().isVisible && Camera.main)
            {
                Vector3 rect = Camera.main.WorldToScreenPoint(pointers[i].transform.position);
                GUI.DrawTexture(new Rect(rect.x - 15 / 2, Screen.height - rect.y - 15 / 2, 15, 15), TargetTexture, ScaleMode.StretchToFill, true, 10.0f);
   
            }

        }
    }
}

