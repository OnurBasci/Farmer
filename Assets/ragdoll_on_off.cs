using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ragdoll_on_off : MonoBehaviour
{
    public Collider body_collider;
    public Rigidbody body_rb;
    public Animator body_animator;

    private List<Collider> ragdoll_colliders;
    private List<Rigidbody> ragdoll_rb;

    private void Awake()
    {
        get_ragdoll();
        deactivate_ragdoll();
    }

    private void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            deactivate_ragdoll();
        }
        if(Input.GetKeyDown("l"))
        {
            activate_ragdoll();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "weapon")
        {
            activate_ragdoll();
        }
    }

    private void get_ragdoll()
    {
        //initiate the ragdoll list
        ragdoll_colliders = new List<Collider>();
        ragdoll_rb = new List<Rigidbody>();

        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        Rigidbody[] rbs = gameObject.GetComponentsInChildren<Rigidbody>();

        foreach(Collider c in colliders)
        {
            //add collider if it's not the gameobjects
            if(c != body_collider)
            {
                ragdoll_colliders.Add(c);
                ragdoll_rb.Add(c.gameObject.GetComponent<Rigidbody>());
            }
        }
    }

    public void activate_ragdoll()
    {
        //disable animator
        body_animator.enabled = false;
        //check if the ragdoll list is not empty
        if (ragdoll_colliders.Count <= 0)
            return;

        foreach(Collider c in ragdoll_colliders)
        {
            c.isTrigger = false;
        }

        body_collider.isTrigger = true;
        //enable all the ragdoll gravities
        set_rb_off();
    }

    public void deactivate_ragdoll()
    {
        //enable animator
        body_animator.enabled = true;
        //check if the ragdoll list is not empty
        if (ragdoll_colliders.Count <= 0)
            return;
        
        //ragdoll of for each of the collider
        foreach(Collider c in ragdoll_colliders)
        {
            c.isTrigger = true;
        }
        //set the game_object collider active
        body_collider.isTrigger = false;

        //disable all the ragdoll gravities
        set_rb_on();
    }

    private void set_rb_on()
    {
        foreach(Rigidbody rb in ragdoll_rb)
        {
            rb.isKinematic = true;
        }
        body_rb.isKinematic = false;
    }
    private void set_rb_off()
    {
        foreach (Rigidbody rb in ragdoll_rb)
        {
            rb.isKinematic = false;
        }
        body_rb.isKinematic = true;
    }
}
