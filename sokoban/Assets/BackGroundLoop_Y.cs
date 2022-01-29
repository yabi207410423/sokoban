using UnityEngine;
using System.Collections;
public class BackGroundLoop_Y : MonoBehaviour
{
    public float speed = 0.2f;
    void Update()
    {
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0,Time.time * speed);
    }
}
