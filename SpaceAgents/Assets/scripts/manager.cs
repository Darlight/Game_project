using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manager : MonoBehaviour {

    public static int Objectlife = 100;//Life of the enemy that the raycast is pointing to
    public static int Objectpower = 100;//power of the enemy that the raycas is pointing to
    public GameObject explosion;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Ray myRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        RaycastHit hitInfo;
        GameObject player = GameObject.Find("PlayerShip");
        transform.Find("ownLife").GetComponent<UnityEngine.UI.Text>().text = "LIFE: " + player.GetComponent<Spaceflight>().life;//get user life
        transform.Find("ownPower").GetComponent<UnityEngine.UI.Text>().text = "POWER: " + player.GetComponent<Spaceflight>().power;//get user power

        if (Input.GetMouseButtonDown(0))//OnClick
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
    }
}
