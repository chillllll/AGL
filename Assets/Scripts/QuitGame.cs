using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public bool isQuit;
    
    void Update()
    {
        if(isQuit==true)
            Application.Quit();
    }

    public void quitGame()
    {
        isQuit = true;
    }
}
