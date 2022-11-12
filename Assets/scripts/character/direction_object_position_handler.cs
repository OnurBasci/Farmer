using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class direction_object_position_handler : MonoBehaviour
{
    //this object is a refernce for the player for it's move direction

    public Transform character;
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, character.position.y, transform.position.z);
    }
}
