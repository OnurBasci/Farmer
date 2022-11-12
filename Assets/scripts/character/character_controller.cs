using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character_controller : MonoBehaviour
{
    public float m_speed = 5;
    public Transform cam;
    public float jump_force = 5;
    public Transform look_object; //for roling towards look direction object
    public Collider weapon_collider;

    private Rigidbody rb;

    private character_input ci;

    private camera_script cam_script;

    private Animator animator;

    private static bool is_rolling = false;
    private static bool is_left_attacking1 = false;
    private static bool is_left_attacking2 = false;
    private float current_jump_force;
    private int attack_click_number = 0;
    private int number_of_attacks = 0;
    private bool left_mouse_clicked = false;

    //for velocity
    Vector3 PrevPos;
    Vector3 NewPos;
    //public static Vector3 character_velocity; delete later

    public void Start()
    {
        //get objects rigidbody
        rb = gameObject.GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        //get instance of the getcomponent script
        cam_script = camera_script.instance;
        PrevPos = transform.position;
        NewPos = PrevPos;
    }

    private void Awake()
    {
        ci = new character_input();
        current_jump_force = jump_force;
        //ci.player.move.started += Move => move();
        Vector2 first = new Vector2(5, 7);
        Vector2 second = new Vector2(3, 4);
        float[] matrix = get_transformaiton_matrix(first, second);
        Vector3 result = mat_mult(matrix, new float[] { 0, 1 });
        Vector3 result2 = mat_mult(matrix, new float[] { 1, 0 });
    }

    public void FixedUpdate()
    {
        move();
        //Debug.Log(attack_click_number);
    }

    public void Update()
    {
        //Debug.Log(number_of_attacks);
        //attack if not rolling (in update because of getmousebuttondown method)
        if (!is_rolling)
        {
            attack();
        }
    }

    public void move()
    {
        //if the character is not rolling
        if(!is_rolling)
        {
            //get move direction
            Vector3 move_dir;
            if((!is_left_attacking1 && !is_left_attacking2))
            {
                if(camera_mode.on_normal_mode)
                {
                    move_dir = get_movedir(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
                    run_normal_mode(move_dir);
                }
                else if(camera_mode.on_focus_mode)
                {
                    move_dir = get_movedir(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
                    run_focus_mode(move_dir);
                }
            }
            else if(is_left_attacking2)
            {
                walk_while_attacknig();
            }
        }
        roll(get_look_dir());
    }

    public void run_normal_mode(Vector3 move_dir)
    {
        // rb.MovePosition(transform.position + move_dir * Time.fixedDeltaTime * m_speed);
        //move
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(move_dir.x * m_speed * Time.fixedDeltaTime, rb.velocity.y, move_dir.z * m_speed * Time.fixedDeltaTime), 0.1f);

        //look
        if (Vector3.Distance(move_dir, Vector3.zero) > 0.1f)
        {
            Vector3 target_look_rotation = new Vector3(move_dir.x, 0, move_dir.z);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target_look_rotation, Vector3.up), 0.2f);
        }

        //animation
        //normalize speed
        float f_speed = rb.velocity.magnitude / m_speed;
        //run animation
        animator.SetFloat("f_w", f_speed);
    }

    public void run_focus_mode(Vector3 move_dir)
    {
        //move
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(move_dir.x * m_speed * Time.fixedDeltaTime, rb.velocity.y, move_dir.z * m_speed * Time.fixedDeltaTime), 0.1f);

        //look
        Vector3 target_look_rotation = camera_mode.surrounding_ennemies[camera_mode.enemy_index].transform.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target_look_rotation, Vector3.up), 0.2f);

        //animation
        //normalize speed
        float f_speed = rb.velocity.magnitude / m_speed;
        //run animation
        animator.SetFloat("f_w", f_speed);
    }

    private bool attacking = false;
    public void walk_while_attacknig()
    {
        //if left_attack2 is playing walk a bit
        Vector3 target = look_object.position - transform.position;
        if(attacking)
        {
            rb.AddForce(target * 20000);
            attacking = false;
        }
    }

    public void roll(Vector3 movedir)
    {
        //adjust the jump force
        if (rb.velocity.magnitude < 0.01f)
            current_jump_force = jump_force * 10;
        else
            current_jump_force = jump_force;
        //roll if not rolling and jump button is down
        if (Input.GetAxisRaw("Jump") == 1 && !is_rolling)
        {
            //roll
            rb.AddForce(movedir * 1000 * current_jump_force * Time.fixedDeltaTime);
            //start animation
            animator.SetBool("is_rolling", true);
        }
        else
        {
            animator.SetBool("is_rolling", false);
        }
        //get the roll animation state
        is_rolling = animator.GetCurrentAnimatorStateInfo(0).IsName("roll_animation");
    }
    private bool attacked = false;
    public void attack()
    {
        //if left click is clicked then light attack
        //if light attack is clicked third consecutively then 3th attack is activated

        if(Input.GetMouseButtonDown(0))
        {
            left_mouse_clicked = true;
            //for the second attack transition
            increase_attack_number();
            attacked = true;
        }
        //while the mouse is pressing
        if (Input.GetMouseButton(0))
        {
            //start the animation
            animator.SetBool("left_attack1", true);
            //left_mouse_clicked = true;
        }
        else
        {
            //end the animation condition
            animator.SetBool("left_attack1", false);
        }

        //get the left attack 1 state
        is_left_attacking1 = animator.GetCurrentAnimatorStateInfo(0).IsName("attack1");

        //if the attack animation is not playing
        if (!is_left_attacking1 && !is_left_attacking2)
        {
            //deactivate the weapon collider
            weapon_collider.isTrigger = true;
            //attack condition
            if (attacked)
            {
                number_of_attacks++;
                attacked = false;
            }
        }
        else if(is_left_attacking1)
        {
            //activate the weapon collider
            weapon_collider.isTrigger = false;

            //left_mouse_clicked = false;
            //don't move if attacking
            rb.velocity = Vector3.zero;


            if(attack_click_number >= 3)
            {
                animator.SetBool("left_attack2", true);
                attack_click_number = 0;
                left_mouse_clicked = false;
                attacking = true;
                number_of_attacks = 0;
            }
        }
        else if(is_left_attacking2)
        {
            weapon_collider.isTrigger = false;
        }

        //get the left attack 2 state
        is_left_attacking2 = animator.GetCurrentAnimatorStateInfo(0).IsName("attack2");
        if(is_left_attacking2)
        {
            animator.SetBool("left_attack2", false);
        }
    }

    public void increase_attack_number()
    {
        //if he attack did not started
        if(attack_click_number == 0 && left_mouse_clicked && !is_left_attacking1)
        {
            attack_click_number = 1;
            left_mouse_clicked = false;
            //start initialization of attack number
            StartCoroutine(reinitialize_attack_state(2.0f));
        }
        if(3 > attack_click_number && attack_click_number >= 1 && left_mouse_clicked && number_of_attacks >= 2)
        {
            attack_click_number += 1;
        }
    }

    private IEnumerator reinitialize_attack_state(float wait_time)
    {
        yield return new WaitForSeconds(wait_time);
        attack_click_number = 0;
        number_of_attacks = 0;
    }

    public Vector3 get_movedir(Vector2 move_value)
    {
        //this function transforms world moving direction to characters look direction
        //float hor_input = Input.GetAxisRaw("Horizontal");
        //float ver_input = Input.GetAxisRaw("Vertical");

        //Vector3 move_dir = new Vector3(hor_input, 0, ver_input);
        //get move directions and create the matrix
        Vector2[] vects = get_camera_look_dir();
        float[] matrix = get_transformaiton_matrix(vects[0], vects[1]);

        //get the move direction
        Vector3 move_dir = mat_mult(matrix, new float[] { move_value.y, move_value.x });
        return move_dir.normalized;
    }

    public float[] get_transformaiton_matrix(Vector2 result_vector_base1, Vector2 result_vector_base2)
    {
        return new float[4] { result_vector_base1.x, result_vector_base2.x, result_vector_base1.y, result_vector_base2.y };
    }

    public Vector3 mat_mult(float[] matrix, float[] input_vector)
    {
        Vector3 r_val;
        r_val.x = matrix[0] * input_vector[0] + matrix[1] * input_vector[1];
        r_val.z = matrix[2] * input_vector[0] + matrix[3] * input_vector[1];
        r_val.y = 0;

        return r_val;
    }

    public Vector2[] get_camera_look_dir()
    {

        //Vector2 forward_vect = new Vector2(camera.forward.x, camera.forward.z);
        Vector2 forward_vect = get_forward_dir();
        forward_vect.Normalize();
        //rotate the forwrd vector by 90 degrees
        Vector2 right_vect = new Vector2(forward_vect.y, -forward_vect.x);
        return new Vector2[] {forward_vect, right_vect };
    }

    private Vector2 get_forward_dir()
    {
        //this function gives cameras forward position from the base circle of the camera script
        //float radius = cam.GetComponent<camera_script>().radius;
        float radius = cam_script.radius;
        //Vector3 cam_pos = cam.GetComponent<camera_script>().get_cam_pos(radius);
        Vector3 cam_pos = cam_script.get_cam_pos(radius);
        return new Vector2(-cam_pos.x, -cam_pos.z);
    }

    private Vector3 get_look_dir()
    {
        Vector3 lookdir = look_object.position - transform.position;
        return new Vector3(lookdir.x, 0, lookdir.z);
    }

    //not usefull delete later
    /*private void update_velocity()
    {
        //update velocity in characters space
        NewPos = transform.position;
        Vector3 word_velocity = NewPos - PrevPos;
        PrevPos = NewPos;
        //transformation of world velocity to character space
        Vector2[] vects = get_camera_look_dir();
        float[] matrix = get_transformaiton_matrix(vects[0], vects[1]);
        character_velocity = mat_mult(matrix, new float[] { word_velocity.z, word_velocity.y });
    }*/
}
