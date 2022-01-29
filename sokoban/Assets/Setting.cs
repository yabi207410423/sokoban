using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    GameObject SET;
    GameObject Quit;
    GameObject Back;

    void Start()
    {
        SET=GameObject.Find("Setting");
        SET.SetActive(false);
        Back=GameObject.Find("ConfirmBack");
        Back.SetActive(false);
    }
    public void CheckSetting(){
        if(SET.activeSelf==true){
            CloseSetting();
        }
        else{
            OpenSetting();
        }
    }

    public void OpenSetting(){
        SET.SetActive(true);
    }
    public void CloseSetting(){
        SET.SetActive(false);
    }
    public void ConfirmOpen(){
        Back.SetActive(true);
    }
    public void ConfirmClose(){
        Back.SetActive(false);
    }
}
