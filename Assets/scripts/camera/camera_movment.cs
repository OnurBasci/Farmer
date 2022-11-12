using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_movment : MonoBehaviour
{
    public Transform follow;
    public float follow_time = 0.5f;
    private Vector3 velocity = Vector3.zero;


    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, follow.position, ref velocity ,follow_time);
    }
}
