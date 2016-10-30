using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

    public class rb_player_controller : MonoBehaviour {

    
    //0 = aim
    //1 = shield
    //2 = openWings
    private state[] characterStates;
    private const int nrOfStates = 3;
    private int currentState = -1;


    //Avatar parts
    private Rigidbody rb_head;
	private GameObject neck;
	private GameObject lwing;

	private GameObject rwing;
    private Vector3 shoot_euler_angles;
    private Vector3 shoot_position;
    private Vector3 relaxed_euler_angles;
    private Vector3 relaxed_position;
    private ParticleSystem rwingParticles;


    //private Rigidbody rb_rwing;
    private GameObject fwing;
	private GameObject bwing;

    private GameObject eyes;

    //weapon variables
    private GameObject certain_weapon;
	private GameObject current_weapon;
    private bool weapon_charged = false;
    private float charge_timer = 0.0f;
    private const float charge_time = 1.5f;
    private float chargeRatio;

    //movement properties
    public float max_speed;
	private float curr_speed;
	private Vector3 planar_velocity;

    private Vector3 curr_mouse_dir;
    private Vector3 curr_mouse_dir_noy;
    private Vector3 curr_mouse_hit;

	//hovering variables
	private double previous_hight_error = 0.0;
	private double hight_error;
	private double hight_integral = 0.0;
	private double hight_derivative;
	private double hight_adjustment;
	private const double max_hight_adjustment = 1000.0;
	private double wanted_hight = 2.4;

    GameObject mountains;

    //controls the rise speed when the wings was recently closed
    private bool wings_closed_recently;
    private float wings_closed_timer = 0.0f;
    private const float WINGS_CLOSED = 0.7f;

    //rotational variables

    private double prev_head_rot_error = 0.0;
	private double head_rot_error;
	private double head_rot_integral = 0.0;
	private double head_rot_derivative;
	private double head_rot_adjustment;

	private double prev_body_rot_error = 0.0;
	private double body_rot_error;
	private double body_rot_integral = 0.0;
	private double body_rot_derivative;
	private double body_rot_adjustment;

	//projectile
	private GameObject wing_projectile_prefab;
	public LayerMask mouse_mask = (1 << 1) | (1 << 10) | (1 << 13) | (1 << 14) | (1 << 18) | (1 << 21) | (1 << 4);
    public LayerMask hover_mask = (1 << 1) | (1 << 4);

    void Start()
	{
        mountains = GameObject.Find("Mountains");

        //Initiating the different action states of the character
        characterStates = new state[nrOfStates];
        characterStates[0] = new aim();
        characterStates[0].playerController = gameObject.GetComponent<rb_player_controller>();
        characterStates[1] = new shield();
        characterStates[1].playerController = gameObject.GetComponent<rb_player_controller>();
        characterStates[2] = new wings();
        characterStates[2].playerController = gameObject.GetComponent<rb_player_controller>();

        //initiating the different parts of the character
        rb_head = GetComponent<Rigidbody> ();
		neck = GameObject.Find("final_prototype_neckjoint");
		
		rwing = GameObject.Find("final_prototype_rwing");
        shoot_euler_angles = new Vector3(-10, 85, -92);
        shoot_position = new Vector3(1.05f, -0.331f, -0.9f);
        relaxed_euler_angles = new Vector3(0, 90, 0);
        relaxed_position = new Vector3(0.66f, -0.131f, 0.0f);
        rwingParticles = rwing.GetComponentInChildren<ParticleSystem>();

        lwing = GameObject.Find("final_prototype_lwing");
        fwing = GameObject.Find("final_prototype_fwing");
		bwing = GameObject.Find("final_prototype_bwing");

        //initiating the projectile prefab of the character
		wing_projectile_prefab = Resources.Load ("final_prototype_wing_projectile") as GameObject;

        curr_speed = max_speed;

        eyes = GameObject.Find("eyes");
        var renderers = eyes.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            Color c = r.material.color;
            c.a = 0.5f;
            r.material.color = c;
        }
    }

	void Update(){

        //charing the weapon
        if (!weapon_charged)
        {
            chargeRatio = (charge_timer) / charge_time;
            charge_timer += Time.deltaTime;
            if (charge_timer > charge_time)
            {
                weapon_charged = true;
            }
        }

        // === Character input handling =====================================================================

        //AIM ===========================================================================================
        if (Input.GetButtonDown("Fire2"))
        {
            // startAim();
            activateState(0);
        }
        else if (Input.GetButton("Fire2") && currentState == 0)
        {           
            var em = rwingParticles.emission;//.rate = chargeRatio * 500;
            em.rate = chargeRatio * 500;
            //vacker - DO NOT BREAK LINE
            rwing.transform.localPosition = shoot_position + new Vector3(Mathf.Sin(Time.time * 150) * 0.03f * chargeRatio * chargeRatio, Mathf.Sin(Time.time * 150) * 0.03f * chargeRatio * chargeRatio, 0.45f * chargeRatio * chargeRatio + Mathf.Sin(Time.time * 150) * 0.03f * chargeRatio * chargeRatio);

            if (weapon_charged)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    shoot();

                    weapon_charged = false;
                    charge_timer = 0.0f;
                    var em2 = rwingParticles.emission;//.rate = chargeRatio * 500;
                    em2.rate = 0;
                    rwingParticles.Clear();
                }
            }
        }
        else if (Input.GetButtonUp("Fire2"))
        {
           // stopAim();
            deActivateState(0);
        }

        //RAISE SHIELD ===========================================================================================
        if (Input.GetButtonDown("Fire1") && currentState != 0)
        {
            //raiseShield();
            activateState(1);

        }
        else if (Input.GetButton("Fire1")) {
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            //lowerShield();
            deActivateState(1);
        }

        //OPEN WINGS ===========================================================================================
        if (Input.GetButtonDown("Fire3"))
        {
            //openWings();
            activateState(2);
        }
        else if (Input.GetButtonUp("Fire3"))
        {
            //closeWings();
            deActivateState(2);
        }
        //Jump ==============================================================================
        //if(Input.GetButton("Jump"))
        if (Input.GetButtonDown("Jump"))
        {
            wanted_hight = 5.0;
			previous_hight_error = 0.0;
        }
        else if (Input.GetButton ("Jump") ) 
		{
		}
        else if (Input.GetButtonUp("Jump"))
        {
            wanted_hight = 2.4;
			previous_hight_error = 0.0;
        }
        
        //Debug actions ====================================================================

		//Hover Hight
		wanted_hight += Input.mouseScrollDelta.x;
		wanted_hight += Input.mouseScrollDelta.y;
	}

	void FixedUpdate()
	{
        // === Turn functions =====================================================================
        //mouse ray tracing =====================================================================
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit mouse_hit;
        //Physics.Raycast (ray, out mouse_hit, 100, mouse_mask);
        Physics.SphereCast(ray, 0.05f, out mouse_hit, 100, mouse_mask);
        curr_mouse_hit = mouse_hit.point;
        curr_mouse_dir = mouse_hit.point - transform.position;
        curr_mouse_dir_noy = curr_mouse_dir;
        curr_mouse_dir_noy.y = 0;
        curr_mouse_dir_noy.Normalize ();

        //head turning =====================================================================
        Vector3 curr_forward = transform.up;
		curr_forward.y = 0;
		curr_forward.Normalize ();

		head_rot_error = -Vector3.Angle (curr_mouse_dir_noy, curr_forward);
		if (Vector3.Cross (curr_mouse_dir_noy, curr_forward).y < 0)
			head_rot_error = -head_rot_error;

		head_rot_integral = head_rot_integral + head_rot_error * Time.deltaTime;
		head_rot_derivative = (head_rot_error - prev_head_rot_error) / Time.deltaTime;

		head_rot_adjustment = 0.07 * head_rot_error + 0.0 * head_rot_integral + 0.04 * head_rot_derivative;
		prev_head_rot_error = head_rot_error;
		rb_head.AddRelativeTorque(Vector3.forward * (float)head_rot_adjustment);

        //Body turning =====================================================================
        curr_forward = neck.transform.forward;
		curr_forward.y = 0;
		curr_forward.Normalize ();

		body_rot_error = -Vector3.Angle (curr_mouse_dir_noy, curr_forward);
		if (Vector3.Cross (curr_mouse_dir_noy, curr_forward).y < 0)
			body_rot_error = -body_rot_error;

		body_rot_integral = body_rot_integral + body_rot_error * Time.deltaTime;
		body_rot_derivative = (body_rot_error - prev_body_rot_error) / Time.deltaTime;

		body_rot_adjustment = 50.0 * body_rot_error + 0.0 * body_rot_integral + 0.5 * body_rot_derivative;
		prev_body_rot_error = body_rot_error;
		neck.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * (float)body_rot_adjustment);

        // Hover function ===================================================================

        //lowers the character if it's trying to pick something up
        if (wings_closed_recently)
        {
            wings_closed_timer += Time.fixedDeltaTime;
            if (wings_closed_timer > WINGS_CLOSED)
            {
                print("go higher..");
                wanted_hight = 2.4;
                wings_closed_timer = 0.0f;
                wings_closed_recently = false;
            }
        }

        RaycastHit hit;
        //Ray tmpRay = new Ray();
        //tmpRay.origin = rb_head.transform.position;
        //tmpRay.direction = Vector3.down;
        //if (mountains.GetComponent<MeshCollider>().Raycast(tmpRay, out hit, 100)) {
        if(Physics.SphereCast(rb_head.transform.position, 0.2f, Vector3.down, out hit, 100.0f, hover_mask)) {
		//if (Physics.Raycast (rb_head.transform.position, Vector3.down, out hit, 100.0f, 1)) {
			hight_error = wanted_hight - hit.distance;
			hight_integral = hight_integral + hight_error * Time.deltaTime;
			hight_derivative = (hight_error - previous_hight_error) / Time.deltaTime;

			hight_adjustment = 100.0 * hight_error + 0.0 * hight_integral + 50.0 * hight_derivative;
			hight_adjustment = Math.Min (Math.Max (0.0, hight_adjustment), max_hight_adjustment);

			previous_hight_error = hight_error;

			rb_head.AddForce (Vector3.up * (float)hight_adjustment);

		} else {
			previous_hight_error = 0.0;
			hight_integral = 0.0;
		}
			
		//Planar movement ===================================================================
		planar_velocity = rb_head.velocity; planar_velocity.y = 0.0f;

		float moveH = Input.GetAxis ("Horizontal");
		float moveV = Input.GetAxis ("Vertical");


		Vector3 wanted_velocity = new Vector3(moveH, 0, moveV).normalized * curr_speed;
		Vector3 velocity_diff = wanted_velocity - planar_velocity;

		float diff_magnitude = velocity_diff.magnitude;
		velocity_diff.Normalize ();

		if (planar_velocity.magnitude < curr_speed) {
			rb_head.AddForce (velocity_diff * diff_magnitude * diff_magnitude * Time.deltaTime * 60);
		}
	}

	void LateUpdate()
	{
		//Points the right wing towards the mouse ray trace hit point
		Vector3 tmpAng;
        if (currentState == 0) {
			tmpAng = rwing.transform.eulerAngles;
            if (curr_mouse_dir.y < 0)
                tmpAng.z = -90 + Vector3.Angle(curr_mouse_dir, curr_mouse_dir_noy);
            else
                tmpAng.z = -90 + -Vector3.Angle(curr_mouse_dir, curr_mouse_dir_noy);

            rwing.transform.eulerAngles = tmpAng;
           // rwing.transform.LookAt(tmp_mouse);
           // rwing.transform.Rotate(-90, 0, 0);
        }
	}

    // Character state handling ==================================================================

    void activateState(int _state)
    {
        if (currentState != -1)
            characterStates[currentState].deActivate();

        currentState = _state;
        characterStates[currentState].activate();

    }

    void deActivateState(int _state)
    {
        if (_state == currentState)
        {
            characterStates[currentState].deActivate();
            currentState = -1;
        }
    }

    public class state
    {
        public bool active;
        public rb_player_controller playerController;

        public virtual void activate()
        {

        }

        public virtual void deActivate()
        {

        }
    }

    public class aim : state
    {
        public override void activate()
        {
            print("aim active");
            playerController.startAim();
            active = true;

        }

        public override void deActivate()
        {
            print("aim inactive");
            playerController.stopAim();
            active = false;
        }
    }

    public class shield : state
    {
        public override void activate()
        {
            print("shield active");
            playerController.raiseShield();
            active = true;
        }

        public override void deActivate()
        {
            print("shield inactive");
            playerController.lowerShield();
            active = false;
        }
    }

    public class wings : state
    {
        public override void activate()
        {
            print("wings active");
            playerController.openWings();
            active = true;
        }

        public override void deActivate()
        {
            print("wings inactive");
            playerController.closeWings();
            active = false;
        }
    }

    // different action functions =========================================================
    void startAim()
    {
        Destroy(rwing.GetComponent<HingeJoint>());
        Destroy(rwing.GetComponent<Rigidbody>());
        rwing.transform.localEulerAngles = shoot_euler_angles;
        rwing.transform.localPosition = shoot_position;
    }
    void stopAim()
    {
        rwing.transform.localEulerAngles = relaxed_euler_angles;
        rwing.transform.localPosition = relaxed_position;
        Rigidbody tmp_rb = rwing.AddComponent<Rigidbody>();
        tmp_rb.angularDrag = 30;
        HingeJoint tmp_hj = rwing.AddComponent<HingeJoint>();
        tmp_hj.connectedBody = neck.GetComponent<Rigidbody>();
        tmp_hj.autoConfigureConnectedAnchor = true;
        tmp_hj.useLimits = true;
        JointLimits tmp_lim = tmp_hj.limits;
        tmp_lim.min = -50;
        tmp_hj.limits = tmp_lim;
        tmp_hj.useSpring = true;
        JointSpring tmp_spring = tmp_hj.spring;
        tmp_spring.spring = 50;
        tmp_hj.spring = tmp_spring;

        var em3 = rwingParticles.emission;//.rate = chargeRatio * 500;
        em3.rate = 0;
        rwingParticles.Clear();
    }
    void shoot()
    {
        GetComponent<Rigidbody>().AddForce(rwing.transform.up * 200.0f);
        GameObject wing_projectile = Instantiate(wing_projectile_prefab) as GameObject;
        wing_projectile.transform.position = rwing.transform.position;
        wing_projectile.transform.rotation = rwing.transform.rotation;
        //Vector3 tmp_vec = neck.transform.forward;
        Vector3 tmp_vec = curr_mouse_hit - rwing.transform.position;
        //tmp_vec.y = 0;
        tmp_vec.Normalize();
        wing_projectile.GetComponent<Rigidbody>().AddForce(tmp_vec * 7000);
    }
    void openWings()
    {
        curr_speed = max_speed * 0.5f;

        rwing.GetComponent<HingeJoint>().useMotor = true;
        JointMotor tmp_mot = rwing.GetComponent<HingeJoint>().motor;
        tmp_mot.targetVelocity = -70;
        tmp_mot.force = 700;
        rwing.GetComponent<HingeJoint>().motor = tmp_mot;

        lwing.GetComponent<HingeJoint>().useMotor = true;
        lwing.GetComponent<HingeJoint>().motor = tmp_mot;

        fwing.GetComponent<HingeJoint>().useMotor = true;
        fwing.GetComponent<HingeJoint>().motor = tmp_mot;

        bwing.GetComponent<HingeJoint>().useMotor = true;
        bwing.GetComponent<HingeJoint>().motor = tmp_mot;
    }
    void closeWings()
    {
        curr_speed = max_speed;
        rwing.GetComponent<HingeJoint>().useMotor = false;
        lwing.GetComponent<HingeJoint>().useMotor = false;
        fwing.GetComponent<HingeJoint>().useMotor = false;
        bwing.GetComponent<HingeJoint>().useMotor = false;

        wings_closed_recently = true;
        if (!Input.GetButton("Jump"))
            wanted_hight = 2.1f;
    }
    void raiseShield()
    {
        Destroy(rwing.GetComponent<HingeJoint>());
        //Destroy (rwing.GetComponent<Rigidbody> ());
        rwing.transform.localEulerAngles = new Vector3(0, 50, 0);
        rwing.transform.localPosition = new Vector3(0.9f, 0.6f, 0.48f);
        // Rigidbody tmp_rb = rwing.AddComponent<Rigidbody>();
        // tmp_rb.angularDrag = 30;
        FixedJoint tmp_hj = rwing.AddComponent<FixedJoint>();
        tmp_hj.connectedBody = neck.GetComponent<Rigidbody>();
        tmp_hj.autoConfigureConnectedAnchor = true;


        Destroy(lwing.GetComponent<HingeJoint>());
        //Destroy (lwing.GetComponent<Rigidbody> ());
        lwing.transform.localEulerAngles = new Vector3(0, -50, 0);
        lwing.transform.localPosition = new Vector3(-0.9f, 0.6f, 0.48f);
        // tmp_rb = rwing.AddComponent<Rigidbody>();
        // tmp_rb.angularDrag = 30;
        tmp_hj = lwing.AddComponent<FixedJoint>();
        tmp_hj.connectedBody = neck.GetComponent<Rigidbody>();
        tmp_hj.autoConfigureConnectedAnchor = true;

        Destroy(fwing.GetComponent<HingeJoint>());
        //Destroy (fwing.GetComponent<Rigidbody> ());
        fwing.transform.localEulerAngles = new Vector3(0, 0, 0);
        fwing.transform.localPosition = new Vector3(0, 0.6f, 0.9f);
        // tmp_rb = rwing.AddComponent<Rigidbody>();
        // tmp_rb.angularDrag = 30;
        tmp_hj = fwing.AddComponent<FixedJoint>();
        tmp_hj.connectedBody = neck.GetComponent<Rigidbody>();
        tmp_hj.autoConfigureConnectedAnchor = true;

        bwing.GetComponent<Rigidbody>().mass = 3;
    }
    void lowerShield()
    {
        Destroy(rwing.GetComponent<FixedJoint>());
        rwing.transform.localEulerAngles = new Vector3(0, 90, 0);
        rwing.transform.localPosition = new Vector3(0.66f, -0.131f, 0.0f);
        //Rigidbody tmp_rb = rwing.AddComponent<Rigidbody> ();
        //tmp_rb.angularDrag = 30;
        HingeJoint tmp_hj = rwing.AddComponent<HingeJoint>();
        tmp_hj.connectedBody = neck.GetComponent<Rigidbody>();
        tmp_hj.autoConfigureConnectedAnchor = true;
        tmp_hj.useLimits = true;
        JointLimits tmp_lim = tmp_hj.limits;
        tmp_lim.min = -50;
        tmp_hj.limits = tmp_lim;
        tmp_hj.useSpring = true;
        JointSpring tmp_spring = tmp_hj.spring;
        tmp_spring.spring = 50;
        tmp_hj.spring = tmp_spring;

        Destroy(lwing.GetComponent<FixedJoint>());
        lwing.transform.localEulerAngles = new Vector3(0, -90, 0);
        lwing.transform.localPosition = new Vector3(-0.66f, -0.131f, 0.0f);
        //tmp_rb = lwing.AddComponent<Rigidbody> ();
        //tmp_rb.angularDrag = 30;
        tmp_hj = lwing.AddComponent<HingeJoint>();
        tmp_hj.connectedBody = neck.GetComponent<Rigidbody>();
        tmp_hj.autoConfigureConnectedAnchor = true;
        tmp_hj.useLimits = true;
        tmp_lim = tmp_hj.limits;
        tmp_lim.min = -50;
        tmp_hj.limits = tmp_lim;
        tmp_hj.useSpring = true;
        tmp_spring = tmp_hj.spring;
        tmp_spring.spring = 50;
        tmp_hj.spring = tmp_spring;

        Destroy(fwing.GetComponent<FixedJoint>());
        fwing.transform.localEulerAngles = new Vector3(0, 0, 0);
        fwing.transform.localPosition = new Vector3(0.0f, -0.131f, 0.66f);
        //tmp_rb = fwing.AddComponent<Rigidbody> ();
        //tmp_rb.angularDrag = 30;
        tmp_hj = fwing.AddComponent<HingeJoint>();
        tmp_hj.connectedBody = neck.GetComponent<Rigidbody>();
        tmp_hj.autoConfigureConnectedAnchor = true;
        tmp_hj.useLimits = true;
        tmp_lim = tmp_hj.limits;
        tmp_lim.min = -50;
        tmp_hj.limits = tmp_lim;
        tmp_hj.useSpring = true;
        tmp_spring = tmp_hj.spring;
        tmp_spring.spring = 50;
        tmp_hj.spring = tmp_spring;

        bwing.GetComponent<Rigidbody>().mass = 1;
    }

   
}
