using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

public class CreateSave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnClick()
    {
        if (File.Exists(Application.dataPath + "/Save.txt"))
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/Save.txt");
            string exm = sr.ReadLine();
            sr.Close();
            if (exm == null)
            {
                FileStream fs = new FileStream(Application.dataPath + "/Save.txt", FileMode.Create);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(0);
                }
                fs.Close();
            }
        }
        else
        {
            FileStream fs = new FileStream(Application.dataPath + "/Save.txt", FileMode.Create);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(0);
            }
            fs.Close();
        }
    }
}