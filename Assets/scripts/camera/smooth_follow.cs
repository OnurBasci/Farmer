using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smooth_follow : MonoBehaviour
{
    public GameObject follow_object;
    public float follow_time = 0.1f;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {

    }
    private void FixedUpdate()
    {
        move();
    }
    private void Update()
    {
        update_follow_time();
    }
    public void move()
    {
        //transform.position = Vector3.SmoothDamp(transform.position, follow_object.transform.position, ref velocity, follow_time);
        Vector3 dir = follow_object.transform.position - transform.position;
        transform.position = Vector3.Lerp(transform.position, transform.position + dir, follow_time);
    }

    public void update_follow_time()
    {
        // Debug.Log(follow_time);

        follow_time = get_follow_time(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }

    private double epsilon = 0.001f;
    public float get_follow_time(Vector2 char_vel)
    {
        //if there is no movment then high follow_time
        //if the movment is up or down then low follow time
        //if the movment is lef or right then half of follow time
        //left right
        if (Mathf.Abs(char_vel.x) > 0 && Mathf.Abs(char_vel.y) < epsilon)
            return 0.05f;
        //up down
        else if (Mathf.Abs(char_vel.y) > 0 && Mathf.Abs(char_vel.x) < epsilon)
            return 0.1f;
        //stable
        else if (Mathf.Abs(char_vel.y) < epsilon && Mathf.Abs(char_vel.x) < epsilon)
            return 0.05f;
        //diagonal
        else
            return 0.15f;
    }
}
