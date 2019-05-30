using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        //Exits the game
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
