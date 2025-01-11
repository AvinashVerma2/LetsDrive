
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class CarController : MonoBehaviour
{
    [Header("Wheel Collider")]
    [SerializeField] WheelCollider ForntRightWheelCollider;
    [SerializeField] WheelCollider ForntLeftWheelCollider;
    [SerializeField] WheelCollider BackRightWheelCollider;
    [SerializeField] WheelCollider BackLeftWheelCollider;

    [Header("Wheel Mesh")]
    [SerializeField] Transform ForntRightWheelMesh;
    [SerializeField] Transform ForntLeftWheelMesh;
    [SerializeField] Transform BackRightWheelMesh;
    [SerializeField] Transform BackLeftWheelMesh;

    [Header("Wheel Power")]
    [SerializeField] float Torque;
    [SerializeField] float BackTorque;
    [SerializeField] float MaxSpeedCar;
    [SerializeField] float Brakeforce;
    [SerializeField] float BostForce;



    private float horizontalInput = 0f;
  

    float VerticalMove;

    // bool breakApply;
    float HorizontalNewInput;
    float VerticalNewInput;

    bool VerticalInputPostive;
    bool VerticalInputNagtive;

    bool BreakNewInput;


    PlayerActions inputActions;


    //Matarial 
    [Header("Car Light")]
    [SerializeField] Material BreakLight;
    [SerializeField] Material BackLight;
    [SerializeField] Material ForntLight;
    bool backlight;



    bool isOn;
    bool ForntLightKey;




    //car Audio 
    [Header("Car Audio")]
    public AudioSource audioSource;
    // public AudioSource gearshift;
    [SerializeField] float targetPitch = 0f;
    [SerializeField] float PMaxPitch = 2.5f;
    [SerializeField] float NMaxPitch = -1.5f;
    [SerializeField] float pitchincrease = 0.2f;


    [Header("Bost Button ")]
    public AudioSource Bost;
    // public AudioSource BackBeep;

    bool Inputstreeing;


    // Speed Boost

    Rigidbody MainRb;
    bool BostSpeed;
    private bool hasPressedKey = false;

    // Bost Particle
    public ParticleSystem bost;
    public ParticleSystem bost2;


    // [SerializeField] float SpeedBoostForce;




    //Crash Audio
    //public Collider bodyCollider;


    // backlightwait
    bool check;


    private Vector3 velocity = Vector3.zero;












    void Awake()
    {
        inputActions = new PlayerActions();
    }


    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }
    void Start()
    {

        //Cursor.lockState = CursorLockMode.Locked;
        targetPitch = 0f;

        MainRb = GetComponent<Rigidbody>();




        check = true;

        ForntLight.DisableKeyword("_EMISSION");

        controlledAntiFlip = gameObject.GetComponent<ControlledAntiFlip>();
        antiRollBar = gameObject.GetComponent<AntiRollBar>();
    }


    void FixedUpdate()
    {

        float Accelt = Input.GetAxisRaw("Vertical");
        float Trun = Input.GetAxisRaw("Horizontal");

        //bool brake = Input.GetKey(KeyCode.Space);
        //brake = breakApply;

        // Accelt = VerticalMove;

        HorizontalNewInput = inputActions.Ground.Horizontal.ReadValue<float>();
        VerticalNewInput = inputActions.Ground.Vertical.ReadValue<float>();
        BreakNewInput = inputActions.Ground.Brakeforce.ReadValue<float>() > 0.1f;
        Inputstreeing = inputActions.Ground.streering.ReadValue<float>() > 0f;
        backlight = inputActions.Ground.BackLight.ReadValue<float>() > 0.1f;



        Trun = horizontalInput;

        Accelt = VerticalNewInput;
        Trun = HorizontalNewInput;
        //brake1 = BreakNewInput;

        float maxspeed = MainRb.velocity.magnitude * 3.6f;

        if (maxspeed < MaxSpeedCar)
        {
            ForntRightWheelCollider.motorTorque = Accelt * Torque * 100 * Time.deltaTime;
            ForntLeftWheelCollider.motorTorque = Accelt * Torque * 100 * Time.deltaTime;

            BackRightWheelCollider.motorTorque = Accelt * BackTorque * 100 * Time.deltaTime;
            BackLeftWheelCollider.motorTorque = Accelt * BackTorque * 100 * Time.deltaTime;
        }
        else
        {
            ForntRightWheelCollider.motorTorque = 0 * Time.deltaTime;
            ForntLeftWheelCollider.motorTorque = 0 * Time.deltaTime;

            BackRightWheelCollider.motorTorque = 0 * Time.deltaTime;
            BackLeftWheelCollider.motorTorque = 0 * Time.deltaTime;
        }

        ForntRightWheelCollider.steerAngle = Trun * 30;
        ForntLeftWheelCollider.steerAngle = Trun * 30;

        if (Inputstreeing)
        {
            float Steering = SimpleInput.GetAxis("Horizontal");
            ForntRightWheelCollider.steerAngle = Steering * 30;
            ForntLeftWheelCollider.steerAngle = Steering * 30;

        }
        WheelUpdate();

        if (BreakNewInput)
        {
            //  BackLight.EnableKeyword("_EMISSION");
            targetPitch = 0f;
            BackLight.EnableKeyword("_EMISSION");
            BackRightWheelCollider.brakeTorque = Brakeforce * 1000 * Time.deltaTime;
            BackLeftWheelCollider.brakeTorque = Brakeforce * 1000 * Time.deltaTime;
            ForntRightWheelCollider.brakeTorque = Brakeforce * 1000 * Time.deltaTime;
            ForntLeftWheelCollider.brakeTorque = Brakeforce * 1000 * Time.deltaTime;


        }

        else
        {
            BackLight.DisableKeyword("_EMISSION");
            BackRightWheelCollider.brakeTorque = 0;
            BackLeftWheelCollider.brakeTorque = 0;
            ForntRightWheelCollider.brakeTorque = 0;
            ForntLeftWheelCollider.brakeTorque = 0;

            // BackLight.DisableKeyword("_EMISSION");
        }




        if (backlight)
        {

            if (check)
            {
                BackLight.EnableKeyword("_EMISSION");
                StartCoroutine(BackLightWait());
            }



        }
        else
        {


            // BackLight.DisableKeyword("_EMISSION");


        }


        //Car Audio 

        if (VerticalNewInput == 1)
        {
            //  BackBeep.Stop();
            targetPitch = PMaxPitch; // Set your desired minimum pitch when "W" is released
        }
        if (VerticalNewInput == -1)
        {
            targetPitch = NMaxPitch; // Set your desired minimum pitch when "W" is released
                                     // BackBeep.Play();
        }
        if (VerticalNewInput == 0)
        {
            //   BackBeep.Stop();
            targetPitch = 0f; // Set your desired minimum pitch when "W" is released


        }


        // Smoothly interpolate pitch
        audioSource.pitch = Mathf.Lerp(audioSource.pitch, targetPitch, pitchincrease * Time.deltaTime * 10);

        // if (Input.GetKey(KeyCode.T))
        // {
        //     gearshift.Play();
        // }
        // else
        // {
        //     gearshift.Stop();
        // }


        if (BostSpeed && !hasPressedKey)
        {
            //audioSource.pitch = 2;
            hasPressedKey = true;
            StartCoroutine(ActivateFunctionAfterDelay());
            // Debug.Log("Bost");
            Bost.Play();
            MainRb.AddForce(transform.forward * BostForce * 14000 * Time.deltaTime);
            bost.Play();
            bost2.Play();

            ForntRightWheelCollider.motorTorque = 500;
            ForntLeftWheelCollider.motorTorque = 500;
            BackRightWheelCollider.motorTorque = 500;
            BackLeftWheelCollider.motorTorque = 500;
        }
        MainRb.AddForce(-transform.up * 20 * MainRb.velocity.magnitude);
        MainRb.centerOfMass = new Vector3(0, -0.5f, 0);

    }

    IEnumerator BackLightWait()
    {
        yield return new WaitForSeconds(0.3f);
        check = false;
        BackLight.DisableKeyword("_EMISSION");
        yield return new WaitForSeconds(0.5f);
        check = true;


    }
    void Update()
    {


        ForntLightKey = inputActions.Ground.ForntLight.ReadValue<float>() > 0.1f;
        BostSpeed = inputActions.Ground.Boost.ReadValue<float>() > 0.1f * Time.deltaTime;

        if (ForntLightKey)
        {
            isOn = !isOn;
            // Perform actions based on the state (e.g., enable/disable a panel or light)
            if (isOn)
            {
                ForntLight.EnableKeyword("_EMISSION");
            }
            else
            {
                ForntLight.DisableKeyword("_EMISSION");
            }
        }

      


        Debug.Log(MainRb.velocity.magnitude * 3.6f);
    }



    private IEnumerator ActivateFunctionAfterDelay()
    {

       
        yield return new WaitForSeconds(3f);
        BackRightWheelCollider.brakeTorque = 10000;
        BackLeftWheelCollider.brakeTorque = 10000;
        ForntRightWheelCollider.brakeTorque = 10000;
        ForntLeftWheelCollider.brakeTorque = 10000;
        bost.Stop();
        bost2.Stop();
        hasPressedKey = false;
        Debug.Log(hasPressedKey);




    }
   

    void WheelUpdate()
    {
        Rotate(ForntRightWheelCollider, ForntRightWheelMesh);
        Rotate(ForntLeftWheelCollider, ForntLeftWheelMesh);
        Rotate(BackRightWheelCollider, BackRightWheelMesh);
        Rotate(BackLeftWheelCollider, BackLeftWheelMesh);
    }

    void Rotate(WheelCollider wheelcollider, Transform WheelTransform)
    {
        Vector3 pos;
        Quaternion Rot;

        wheelcollider.GetWorldPose(out pos, out Rot);
        WheelTransform.position = pos;
        WheelTransform.rotation = Rot;
    }

    // Speed Boost 

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "SpeedBost")
        {
            // VerticalNewInput=1;
            VerticalNewInput = 1;
            // Torque = BostTorque;
            // BackTorque = BostBackTorque;
            MainRb.AddForce(transform.forward * BostForce * 150 * Time.deltaTime);
            bost.Play();
            bost2.Play();

        }


    }

    int sidewaysFrictionValue = 2;
    int FrontwaysFrictionValue = 2;
    int i = 0;
    ControlledAntiFlip controlledAntiFlip;
    AntiRollBar antiRollBar;


    float StuntsidewaysFrictionValue = 1.5f;
    float StuntFrontwaysFrictionValue = 1.5f;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "JampRampBost")
        {
            BackTorque = 700;
            Torque = 1000;

        }
        if (other.gameObject.tag == "SpeedBost")
        {
            VerticalNewInput = 1;
            Bost.Play();
            // VerticalNewInput=1;
            Debug.Log("bostOn");
            // Torque = BostTorque;
            // BackTorque = BostBackTorque;
            //MainRb.AddForce(transform.forward * BostForce *10000* Time.deltaTime);
        }
        // if (other.gameObject.tag == "JumpFix")
        // {

        //     ForntRightWheelCollider.suspensionDistance = 0.1f;
        //     ForntLeftWheelCollider.suspensionDistance = 0.1f;
        //     BackRightWheelCollider.suspensionDistance = 0.1f;
        //     BackLeftWheelCollider.suspensionDistance = 0.1f;

        //     WheelFrictionCurve sidewaysFriction = ForntRightWheelCollider.sidewaysFriction;
        //     sidewaysFriction.stiffness = sidewaysFrictionValue;
        //     ForntRightWheelCollider.sidewaysFriction = sidewaysFriction;

        //     WheelFrictionCurve sidewaysFriction1 = ForntLeftWheelCollider.sidewaysFriction;
        //     sidewaysFriction1.stiffness = sidewaysFrictionValue;
        //     ForntLeftWheelCollider.sidewaysFriction = sidewaysFriction1;

        //     WheelFrictionCurve sidewaysFriction2 = BackRightWheelCollider.sidewaysFriction;
        //     sidewaysFriction2.stiffness = sidewaysFrictionValue;
        //     BackRightWheelCollider.sidewaysFriction = sidewaysFriction2;

        //     WheelFrictionCurve sidewaysFriction3 = BackLeftWheelCollider.sidewaysFriction;
        //     sidewaysFriction3.stiffness = sidewaysFrictionValue;
        //     BackLeftWheelCollider.sidewaysFriction = sidewaysFriction3;




        //     WheelFrictionCurve forwardFriction = ForntRightWheelCollider.forwardFriction;
        //     forwardFriction.stiffness = FrontwaysFrictionValue;
        //     ForntRightWheelCollider.forwardFriction = forwardFriction;

        //     WheelFrictionCurve forwardFriction1 = ForntLeftWheelCollider.forwardFriction;
        //     forwardFriction1.stiffness = FrontwaysFrictionValue;
        //     ForntLeftWheelCollider.forwardFriction = forwardFriction1;

        //     WheelFrictionCurve forwardFriction2 = BackRightWheelCollider.forwardFriction;
        //     forwardFriction2.stiffness = FrontwaysFrictionValue;
        //     BackRightWheelCollider.forwardFriction = forwardFriction2;

        //     WheelFrictionCurve forwardFriction3 = BackLeftWheelCollider.forwardFriction;
        //     forwardFriction3.stiffness = FrontwaysFrictionValue;
        //     BackLeftWheelCollider.forwardFriction = forwardFriction3;

        //     if (i < 1)
        //     {
        //         i = 1;
        //         //gameObject.AddComponent<AntiRollBar>();

        //     }

        //     // controlledAntiFlip.enabled = true;
        //     // antiRollBar.enabled = true;
        //     // DownCollider.SetActive(true);

        // }
        if (other.gameObject.tag == "OpenStunt")
        {



            WheelFrictionCurve sidewaysFriction = ForntRightWheelCollider.sidewaysFriction;
            sidewaysFriction.stiffness = StuntsidewaysFrictionValue;
            ForntRightWheelCollider.sidewaysFriction = sidewaysFriction;

            WheelFrictionCurve sidewaysFriction1 = ForntLeftWheelCollider.sidewaysFriction;
            sidewaysFriction1.stiffness = StuntsidewaysFrictionValue;
            ForntLeftWheelCollider.sidewaysFriction = sidewaysFriction1;

            WheelFrictionCurve sidewaysFriction2 = BackRightWheelCollider.sidewaysFriction;
            sidewaysFriction2.stiffness = StuntsidewaysFrictionValue;
            BackRightWheelCollider.sidewaysFriction = sidewaysFriction2;

            WheelFrictionCurve sidewaysFriction3 = BackLeftWheelCollider.sidewaysFriction;
            sidewaysFriction3.stiffness = StuntsidewaysFrictionValue;
            BackLeftWheelCollider.sidewaysFriction = sidewaysFriction3;




            WheelFrictionCurve forwardFriction = ForntRightWheelCollider.forwardFriction;
            forwardFriction.stiffness = StuntFrontwaysFrictionValue;
            ForntRightWheelCollider.forwardFriction = forwardFriction;

            WheelFrictionCurve forwardFriction1 = ForntLeftWheelCollider.forwardFriction;
            forwardFriction1.stiffness = StuntFrontwaysFrictionValue;
            ForntLeftWheelCollider.forwardFriction = forwardFriction1;

            WheelFrictionCurve forwardFriction2 = BackRightWheelCollider.forwardFriction;
            forwardFriction2.stiffness = StuntFrontwaysFrictionValue;
            BackRightWheelCollider.forwardFriction = forwardFriction2;

            WheelFrictionCurve forwardFriction3 = BackLeftWheelCollider.forwardFriction;
            forwardFriction3.stiffness = StuntFrontwaysFrictionValue;
            BackLeftWheelCollider.forwardFriction = forwardFriction3;


        }
    }





}

