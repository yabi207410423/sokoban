using UnityEngine;
using System.Collections;
public class BackGroundLoop_X : MonoBehaviour
{
    public float speed = 0.2f;
    void Update()
    {
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(Time.time * speed,0);
    }
}
