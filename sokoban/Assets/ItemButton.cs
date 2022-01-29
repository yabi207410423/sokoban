using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public Sprite itemSprite;


    private int level = 0;
    private Button button;
    private Image image;

    void OnEnable()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }
    public void Setup(int level,bool isShow)
    {
        this.level = level;
        if (isShow)
        {
            image.sprite = itemSprite;
            button.enabled = true;
        }
        else
        {
            image.sprite = null;
            button.gameObject.SetActive(false);
        }
    }
    public void OnClick()
    {
        
    }
}
