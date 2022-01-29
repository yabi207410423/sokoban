using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelSelectMenu : MonoBehaviour
{
    public int totalLevel = 0;
    public int unlockerLevel;
    private LevelButton[] levelButtons;
    private int totalPage = 0;
    private int page = 0;
    private int pageItem = 25;

    public GameObject backButton;
    public GameObject nextButton;
    void OnEnable()
    {
        levelButtons = GetComponentsInChildren<LevelButton>();
    }
    // Start is called before the first frame update
    void Start()
    {
        // backButton.SetActive(false);
        FileStream fs = new FileStream(Application.dataPath + "/Save.txt", FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        unlockerLevel = int.Parse(sr.ReadLine());
        sr.Close();
        Refresh();
    }
    public void ClickNext()
    {
        page += 1;
        Refresh();
    }
    public void ClickBack()
    {
        page -= 1;
        Refresh();
    }
    public void Refresh()
    {
        totalPage = totalLevel / pageItem;
        int index = page * pageItem;
        for(int i = 0; i < levelButtons.Length; i++)
        {
            int level = index + i + 1;
            if (level <= totalLevel)
            {
                levelButtons[i].gameObject.SetActive(true);
                levelButtons[i].Setup(level, level <= unlockerLevel);
            }
            else{
                levelButtons[i].gameObject.SetActive(false);
            }
        }
        CheckButton();
    }
    private void CheckButton()
    {
        backButton.SetActive(page > 0);
        nextButton.SetActive(page < totalPage);
    }
}
