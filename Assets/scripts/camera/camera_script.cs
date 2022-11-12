using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_script : MonoBehaviour
{
    public Transform reference;
    public Transform look_at_object;
    public float radius;
    public float follow_time = 0.5f;
    public Vector2 camera_rot_change_speed;
    [Range(-4* Mathf.PI, 4 * Mathf.PI)]
    public float x_rot;
    [Range(-1, 1)]
    public float y_rot;
    private float current_radius;
    private float temp_x_rot, temp_radius, temp_y_rot;
    private Vector3 cam_pos;
    private Vector3 velocity = Vector3.zero;
    public static camera_script instance { get; private set; }

    public void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        //set the cursor invisable
        Cursor.visible = false;
        temp_radius = radius;
        temp_x_rot = x_rot;
        temp_y_rot = y_rot;
        current_radius = radius;
        cam_pos = get_cam_pos(current_radius);
        transform.position = new Vector3(reference.position.x, reference.position.y, reference.position.z - current_radius);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Update_cam_pos();
        if (camera_mode.on_normal_mode)
        {
            change_rotation();
        }
    }

    /*private float getX()
    {
        float x;
        //nomalize the x value by moduo 2pi
        x_rot = x_rot > 0 ? x_rot % (2 * Mathf.PI) : 2 * Mathf.PI + (x_rot % (2 * Mathf.PI));
        //get the x value
        if (x_rot >= 0 && x_rot <= Mathf.PI) 
            x = (2 * x_rot) / Mathf.PI - 1;
        else
            x = (2 - (2 * (x_rot - Mathf.PI)) / Mathf.PI) - 1;
        return x * current_radius;
    }*/
    private float getX()
    {
        //nomalize the x value by moduo 2pi
        x_rot = x_rot > 0 ? x_rot % (2 * Mathf.PI) : 2 * Mathf.PI + (x_rot % (2 * Mathf.PI));
        return Mathf.Cos(x_rot) * current_radius;   
    }

    /*private float getY(float x, float radius)
    {
        float y;
        if (x_rot >= 0 && x_rot <= Mathf.PI)
            y = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(x, 2));
        else
            y = - Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(x, 2));
        return y;
    }*/
    private float getY()
    {
        return Mathf.Sin(x_rot) * current_radius;
    }


    public Vector3 get_cam_pos(float radius)
    {
        float x = getX();
        float y = getY();
        return new Vector3(x, 0, y);
    }

    public void Update_cam_pos()
    {
        //change camera transform from editor
        if (x_rot != temp_x_rot || temp_radius != radius || temp_y_rot != y_rot)
        {
            cam_pos = get_cam_pos(current_radius);
            temp_x_rot = x_rot;
            temp_radius = radius;
            temp_y_rot = y_rot;
        }

        //fallow the point smoothly
        transform.position = Vector3.SmoothDamp(transform.position, reference.position + cam_pos, ref velocity, follow_time);


        //transform.position = reference.position + cam_pos;
        update_vertical_pos();
        if(camera_mode.on_normal_mode)
        {
            transform.LookAt(look_at_object);
        }
    }
    
    public void update_vertical_pos()
    {
        //get horizontal redius in terms of y_rot
        current_radius = radius * Mathf.Cos(y_rot * (Mathf.PI/2));

        //change y position
        transform.position = new Vector3(transform.position.x, reference.position.y + y_rot * radius , transform.position.z);
    }

    //change the camera rotation by changing the mouse position

    public Vector2 get_cam_pos_change()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public void change_rotation()
    {
        float temp_y = y_rot;
        x_rot -= get_cam_pos_change().x * camera_rot_change_speed.x * Time.fixedDeltaTime;
        temp_y -= get_cam_pos_change().y * camera_rot_change_speed.y * Time.fixedDeltaTime;
        y_rot = Mathf.Clamp(temp_y, -0.9f, 0.9f);
    }
    public void setRotX(float rad)
    {
        x_rot = rad;
    }
    public void setRotY(float rad)
    {
        y_rot = rad;
    }
}
