using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
using Complete;
public class TankMovement : NetworkBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
    public float m_Speed = 12f;                 // How fast the tank moves forward and back.
    public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
    public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
    public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
    public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

    private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
    private string m_TurnAxisName;              // The name of the input axis for turning.
    private Rigidbody m_Rigidbody;              // Reference used to move the tank.
    private float m_VerticalInputValue;         // The current value of the movement input.
    private float m_HorizontalInputValue;             // The current value of the turn input.
    private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
    private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the Tanks
    FixedJoystick joystick;
    TextMeshProUGUI debugText;
    float timeSinceMoved;
    public bool hydraulic;
    public bool canMove = true;
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        debugText = GameObject.Find("DebugText").GetComponent<TextMeshProUGUI>();
        timeSinceMoved = Time.time;
    }


    private void OnEnable()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_VerticalInputValue = 0f;
        m_HorizontalInputValue = 0f;

        // We grab all the Particle systems child of that Tank to be able to Stop/Play them on Deactivate/Activate
        // It is needed because we move the Tank when spawning it, and if the Particle System is playing while we do that
        // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
        m_particleSystems = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < m_particleSystems.Length; ++i)
        {
            m_particleSystems[i].Play();
        }

    }


    private void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;

        // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
        for (int i = 0; i < m_particleSystems.Length; ++i)
        {
            m_particleSystems[i].Stop();
        }
    }


    private void Start()
    {
        // The axes names are based on player number.
        m_MovementAxisName = "Vertical";
        m_TurnAxisName = "Horizontal";

        // Store the original pitch of the audio source.
        m_OriginalPitch = m_MovementAudio.pitch;

        if (NetworkObject.IsOwner)
        {
            GameObject.Find("PlayerText").GetComponent<TextMeshProUGUI>().enabled = true;
            GameObject.Find("PlayerText").GetComponent<TextMeshProUGUI>().text = "Player " + m_PlayerNumber.ToString();
        }
    }


    private void EngineAudio()
    {
        // If there is no input (the tank is stationary)...
        if (Mathf.Abs(m_VerticalInputValue) < 0.1f && Mathf.Abs(m_HorizontalInputValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                // ... change the clip to idling and play it.
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = UnityEngine.Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                // ... change the clip to driving and play.
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = UnityEngine.Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }


    private void Update()
    {
        if (NetworkObject.IsOwner)
        {
            // Store the value of both input axes.
            m_VerticalInputValue = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>().Vertical;
            m_HorizontalInputValue = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>().Horizontal;

            EngineAudio();
        }
        if (Mathf.Abs(m_VerticalInputValue) >= 0.001f && Mathf.Abs(m_HorizontalInputValue) >= 0.001f)
        {
            if (NetworkObject.IsOwner)
            {
                /*
                if (Time.time - timeSinceMoved > .01f)
                {
                    timeSinceMoved = Time.time;*/
                if (canMove)
                {
                    Move();
                }
                    //}
                Turn();
            }
        }
    }


    private void Move()
    {
        this.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        float stickSpeed = Mathf.Sqrt(Mathf.Pow(m_HorizontalInputValue,2) + Mathf.Pow(m_VerticalInputValue,2));
        float movementSpeed = stickSpeed * m_Speed * Time.deltaTime;

        //debugText.text = "Transform.forward: " + transform.forward + " : m_Speed: " + m_Speed + " : fixeddeltaTime: " + Math.Round(Time.fixedDeltaTime, 4) + "distance: " + stickSpeed * m_Speed * Time.deltaTime / Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        //m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        transform.position += transform.forward * movementSpeed;
        //debugText.text = "movement: " + movement + "Movement magnitude: " + movement.magnitude + " : currentPos: " + m_Rigidbody.position + " : newPos: " + (m_Rigidbody.position + movement).ToString();
    }


    private void Turn()
    {
        /*
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        */
        // Make this into a rotation in the y axis.
        //tan theta = y1-y2/x1-x2
        float CameraAngle = 180 / Mathf.PI * Mathf.Atan2(Camera.main.transform.position.z - transform.position.z, Camera.main.transform.position.x - transform.position.x) + 90;
        float stickAngle = -(180 / Mathf.PI * Mathf.Atan2(m_VerticalInputValue, m_HorizontalInputValue) - 90);
        //Debug.Log("CameraAngle: " + CameraAngle + " : stickAngle: " + stickAngle);
        Quaternion turnRotation = Quaternion.Euler(0f, -CameraAngle + stickAngle, 0f);

        // Apply this rotation to the rigidbody's rotation.
        this.transform.rotation = turnRotation;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if(IsServer)
        {
            if (collision.gameObject.tag == "MyTank" || collision.gameObject.tag == "Tank")
            {
                if (m_PlayerNumber == 4)
                {
                    collision.gameObject.GetComponent<FireDamage>().startTime = Time.time;
                    collision.gameObject.GetComponent<FireDamage>().takingFire = true;
                }
                if (hydraulic)
                {
                    collision.gameObject.GetComponent<TankHealth>().TakeDamage(40);
                }
            }
        }
    }
}