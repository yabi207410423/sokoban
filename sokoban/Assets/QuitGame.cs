using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{

    GameObject ConfirmQuit;
    // Start is called before the first frame update
    void Start()
    {
        ConfirmQuit = GameObject.Find("ConfirmQuit");
        ConfirmQuit.SetActive(false);
    }
    public void QG(){//QG = QuitGame
        Application.Quit();
    }
    public void ConfirmQuitON(){
        ConfirmQuit.SetActive(true);
    }
    public void ConfirmQuitOFF(){
        ConfirmQuit.SetActive(false);
    }
}
