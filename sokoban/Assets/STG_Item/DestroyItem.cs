using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItem : MonoBehaviour
{
    public AudioSource DestroySound;

    void OnTriggerEnter2D(Collider2D col) //碰撞事件
    {
        if (col.tag == "Meteor") 
        {
            Destroy(col.gameObject);
            DestroySound.Play();
        }
        if (col.tag == "Mars") 
        {
            Destroy(col.gameObject);
            DestroySound.Play();
        }
    }
}
