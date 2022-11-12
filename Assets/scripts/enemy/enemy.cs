using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    /*This script is about basic enemy movments*/

    public Transform character; //character to follow
    public Vector2 speed_borders = new Vector2(2, 4);
    public Vector3 surround_limits = new Vector3(4, 10, 20);

    private Collider[] surrounding_objects_huge;
    private Collider[] surrounding_objects_large;
    private Collider[] surrounding_object_small;

    private float current_speed;

    private Animator animator;

    public void Awake()
    {
        current_speed = speed_borders.x;
        animator = GetComponent<Animator>();
    }
    public void Update()
    {
        move();
        look();
        //Debug.Log(get_angle());
    }

    public void move()
    {
        bool is_seeing = false;
        //check the surrounding objects. If there is character than move
        surrounding_objects_huge = Physics.OverlapSphere(transform.position, surround_limits.z);
        foreach(var s_object in surrounding_objects_huge)
        {
            if(s_object.tag == "Player")
            {
                is_seeing = true;
            }
        }
        if (!is_seeing)
        {
            animator.SetBool("idle", true);
            animator.SetBool("walk", false);
            return;
        }
        bool in_attack_mode = false;
        //if sees the character and close than move in the attacking mode
        surrounding_objects_large = Physics.OverlapSphere(transform.position, surround_limits.y);
        foreach(var s_object in surrounding_objects_large)
        {
            if(s_object.tag == "Player")
            {
                in_attack_mode = true;
            }
        }
        Vector3 target_pos;
        //if the character is not close enough then just follow
        if (!in_attack_mode)
        {
            animator.SetBool("walk", true);
            animator.SetBool("idle", false);
            current_speed = speed_borders.y;
            target_pos = character.position;
        }
        else
        {
            //if it is in the inner circle move differently
            bool is_close = false;
            surrounding_object_small = Physics.OverlapSphere(transform.position, surround_limits.x);
            foreach (var s_object in surrounding_object_small)
            {
                if (s_object.tag == "Player")
                {
                    is_close = true;
                }
            }
            if (is_close)
            {
                //to do
                return;
            }

            current_speed = speed_borders.x;

            float right_angle = get_angle(get_difference_vector(), new Vector2(character.right.x, character.right.z));
            float forward_angle = get_angle(get_difference_vector(), new Vector2(character.forward.x, character.forward.z));
            //if the enemy is looking to character
            if (forward_angle <= 90)
            {
                if (right_angle >= 0 && right_angle <= 90)
                {
                    if (right_angle > 60)
                    {
                        //Debug.Log("forward");
                        target_pos = transform.position - transform.right;
                        animator.SetBool("walk", false);
                        animator.SetBool("walk_left", false);
                        animator.SetBool("walk_right", true);
                    }
                    else
                    {
                        //Debug.Log("to right");
                        target_pos = character.position + (character.right * 2);
                        animator.SetBool("walk", true);
                        animator.SetBool("walk_right", false);
                    }
                }
                else
                {
                    if (right_angle < 120)
                    {
                        //Debug.Log("forward");
                        target_pos = transform.position + transform.right;
                        animator.SetBool("walk", false);
                        animator.SetBool("walk_right", false);
                        animator.SetBool("walk_left", true);
                    }
                    else
                    {
                        //Debug.Log("to left");
                        target_pos = character.position - (character.right * 2);
                        animator.SetBool("walk", true);
                        animator.SetBool("walk_left", false);
                    }
                }
            }
            else
            {
                target_pos = character.position - character.forward * 2;
            }
        }
        
        transform.position = Vector3.MoveTowards(transform.position, target_pos, current_speed * Time.deltaTime);
    }

    public void look()
    {
        Vector3 look_dir = character.position - transform.position;
        Vector3 target_look_rotation = new Vector3(look_dir.x, 0, look_dir.z);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target_look_rotation, Vector3.up), 0.2f);
    }

    private float get_angle(Vector2 A, Vector2 B)
    {
        //gets angle from two vectors angle = acos((A.B)/(|A|.|B|))
         return Mathf.Acos(Vector2.Dot(A, B)/Vector3.Magnitude(A)* Vector3.Magnitude(B)) * Mathf.Rad2Deg;
    }

    private Vector2 get_difference_vector()
    {
        return -1 * new Vector2(character.position.x - transform.position.x, character.position.z - transform.position.z);
    }
}
