using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;

public class STGFunction : MonoBehaviour
{
    GameObject cc;
    GameObject fc;
    GameObject MainCharacter;
    GameObject AllSide;

    public GameObject Meteor; 
    public GameObject Mars; 
    public float TotalTime; 
    public float time; 
    public static bool isClear;
    public static STGFunction Instance;
    public int LEVEL;
    
    // Start is called before the first frame update
    void Start()
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
        Instance = this;
        AllSide = GameObject.Find("AllSide");
        MainCharacter = GameObject.Find("MC");
        cc = GameObject.Find("CC");        
        fc = GameObject.Find("FC");
        cc.SetActive(false);        
        fc.SetActive(false);
        AllSide.SetActive(false);
        isClear= false;
    }

    // Update is called once per frame
    void Update()
    {
        if(MainCharacter==null)
            fc.SetActive(true);        
        
        time += Time.deltaTime;
        TotalTime += Time.deltaTime;

        if(isClear==false){
            if(time>0.5f) //如果時間大於0.5(秒)
            {
                Vector3 pos = new Vector3(Random.Range(-20f,20f),32f,0); //宣告位置pos，Random.Range(-2.5f,2.5f)代表X是2.5到-2.5之間隨機
                Instantiate(Meteor,pos,transform.rotation);//產生敵人
                time = 0f; //時間歸零
            }
            if(TotalTime>10.5f){
                Vector3 pos = new Vector3(Random.Range(-20f,20f),32f,0); //宣告位置pos，Random.Range(-2.5f,2.5f)代表X是2.5到-2.5之間隨機
                Instantiate(Mars,pos,transform.rotation);//產生敵人 
                TotalTime = 0f;
            }
        }
    }
    public void GameClear(){
        cc.SetActive(true);
        isClear=true;
        AllSide.SetActive(true);
        StreamReader sr = new StreamReader(Application.dataPath + "/Save.txt");
            int tem = int.Parse(sr.ReadLine());
            sr.Close();
            if (tem < LEVEL+1)
            {
                SaveTheGame();
            }
    }
    public void SaveTheGame()
    {
        FileStream fs = new FileStream(Application.dataPath + "/Save.txt", FileMode.Create);
        using (StreamWriter sw = new StreamWriter(fs))
        {
            sw.Write(LEVEL+1);
        }
        fs.Close();
    }
}