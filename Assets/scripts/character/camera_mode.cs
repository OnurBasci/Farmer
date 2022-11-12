using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_mode : MonoBehaviour
{
    public static int c_mode = 0;
    public float sphere_radius = 30f;
    public LayerMask enemy_layer;
    public GameObject cam;
    public static bool on_focus_mode = false, on_normal_mode = true;

    private camera_script cam_script;
    public static Collider[] surrounding_ennemies;
    public static int enemy_index = 0;
    private float cam_angle;
    

    private void Awake()
    {
        cam_script = cam.GetComponent<camera_script>();
    }

    private void Update()
    {
        get_c_mod();
        if (on_focus_mode)
        {
            cam_mode2();
        }
    }

    private void cam_mode2()
    {
        Vector2 ref_vect = new Vector2(1, 0);
        Vector3 difference = (surrounding_ennemies[enemy_index].transform.position - transform.position);
        Vector2 dif_vect = new Vector2(difference.x, difference.z).normalized;

        //camera angle is equal to the differenece angle + 180 [360]
        //if the angle is between 0 and 180
        cam_angle = (((difference.z > 0) ?get_angle(ref_vect, dif_vect):-get_angle(ref_vect, dif_vect)) + 180) * Mathf.Deg2Rad;

        cam_script.setRotX(cam_angle);
        cam_script.setRotY(0.5f);

        //look to the enemy
        cam.transform.LookAt(surrounding_ennemies[enemy_index].transform);
    }
       
    private void get_surrounding_ennemies()
    {
        surrounding_ennemies = Physics.OverlapSphere(transform.position, sphere_radius, enemy_layer);
    }

    private void get_c_mod()
    {
        get_surrounding_ennemies();
        //if the middle button is clicked
        if(Input.GetMouseButtonDown(2))
        {
            if (surrounding_ennemies.Length > 0 && on_normal_mode)
            {
                on_focus_mode = true;
                on_normal_mode = false;
            }
            else if(on_focus_mode)
            {
                on_normal_mode = true;
                on_focus_mode = false;
            }
        }
        if(surrounding_ennemies.Length == 0)
        {
            on_normal_mode = true;
            on_focus_mode = false;
        }
    }

    private float get_angle(Vector2 A, Vector2 B)
    {
        //gets angle from two vectors angle = acos((A.B)/(|A|.|B|))
        return Mathf.Acos(Vector2.Dot(A, B) / Vector3.Magnitude(A) * Vector3.Magnitude(B)) * Mathf.Rad2Deg;
    }
}
