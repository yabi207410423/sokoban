using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPUI : MonoBehaviour
{
    GameObject BP;
    // Start is called before the first frame update
    void Start()
    {
        BP=GameObject.Find("BackPack");
        BP.SetActive(false);
    }
    public void CheckBP(){
        if(BP.activeSelf==true){
            CloseBP();
        }
        else{
            OpenBP();
        }
    }
    public void OpenBP(){
        BP.SetActive(true);
    }
    public void CloseBP(){
        BP.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
