using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Backpack : MonoBehaviour
{
    public int totalItem = 0;
    public int unlockerItem;
    private ItemButton[] itemButtons;
    private int totalPage = 0;
    private int page = 0;
    private int pageItem = 25;

    void OnEnable()
    {
        itemButtons = GetComponentsInChildren<ItemButton>();
    }
    // Start is called before the first frame update
    void Start()
    {
        // backButton.SetActive(false);
        FileStream fs = new FileStream(Application.dataPath + "/Save.txt", FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        unlockerItem = int.Parse(sr.ReadLine());
        sr.Close();
        Refresh();
    }
    public void Refresh()
    {
        totalPage = totalItem / pageItem;
        int index = page * pageItem;
        for(int i = 0; i < itemButtons.Length; i++)
        {            
            int item = index + i + 1;
            if (item <= totalItem)
            {
                itemButtons[i].gameObject.SetActive(true);
                itemButtons[i].Setup(item,item <= unlockerItem);
            }
            else{
                itemButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
