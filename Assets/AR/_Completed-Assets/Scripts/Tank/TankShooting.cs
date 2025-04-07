using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace Complete
{
    public class TankShooting : NetworkBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify the different players.
        public Rigidbody m_Shell;                   // Prefab of the shell.
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
        public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
        public float m_MinLaunchForce = 1f;        // The force given to the shell if the fire button is not held.
        public float m_MaxLaunchForce = 50f;        // The force given to the shell if the fire button is held for the max charge time.
        public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.


        private string m_FireButton;                // The input axis that is used for launching shells.
        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
        private bool m_Fired = true;                       // Whether or not the shell has been launched with this button press.
        [SerializeField] public float m_ReloadTime = 1.5f; // Time to reload
        [SerializeField] public float m_BulletPower = 50f; // Bullet damage
        [SerializeField] public float m_BulletSpeed = .005f; // Bullet speed
        [SerializeField] public float m_BulletRange = 2f; // Bullet range time
        public float startTime;
        public bool homing;
        public bool explosive;
        public bool vampire;
        public bool phantom;
        public bool molotov;
        public bool shock;
        public bool canShoot = true;
        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }


        private void Start()
        {
            // The fire axis is based on the player number.
            m_FireButton = "Fire";

            // The rate that the launch force charges up is the range of possible forces by the max charge time.
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
            startTime = Time.time;

        }


        private void Update()
        {
            // The slider should have a default value of the minimum launch force.
            m_AimSlider.value = m_MinLaunchForce;

            // If the max force has been exceeded and the shell hasn't yet been launched...
            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                // ... use the max force and launch the shell.
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire();
            }
            // Otherwise, if the fire button has just started being pressed...
            else if (Input.GetButtonDown(m_FireButton))
            {
                // ... reset the fired flag and reset the launch force.
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                // Change the clip to the charging clip and start it playing.
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
            else if (Input.GetButton(m_FireButton) && !m_Fired)
            {
                // Increment the launch force and update the slider.
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

                m_AimSlider.value = m_CurrentLaunchForce;
            }
            // Otherwise, if the fire button is released and the shell hasn't been launched yet...
            else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
            {
                // ... launch the shell.
                Fire();
            }
        }


        public void Fire()
        {
            if (canShoot)
            {
                // Set the fired flag so only Fire is only called once.
                m_Fired = true;
                startTime = Time.time;
                ShootRpc(this.GetComponent<NetworkObject>().NetworkObjectId, NetworkObject.OwnerClientId);
                foreach (GameObject powerupSlider in GameObject.FindGameObjectsWithTag("PowerupSlider"))
                {
                    powerupSlider.GetComponent<PowerupSliderController>().BulletFired();
                }
            }
        }


        [Rpc(SendTo.Everyone)] //server owns this object but client can request a spawn
        public void ShootRpc(ulong objectId, ulong shooterClientID)
        {
            
            Rigidbody shellInstance =
                Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation, GameObject.Find("Battlefield1").transform) as Rigidbody;
            shellInstance.GetComponent<ShellExplosion>().m_damage = m_BulletPower;
            shellInstance.GetComponent<ShellExplosion>().myClientID = shooterClientID;
            shellInstance.GetComponent<BulletMove>().myTank = this.gameObject;
            shellInstance.GetComponent<BulletMove>().bulletSpeed = m_BulletSpeed;
            shellInstance.GetComponent<ShellExplosion>().m_MaxLifeTime = m_BulletRange;
            
            shellInstance.GetComponent<BulletMove>().homing = homing;
            shellInstance.GetComponent<ShellExplosion>().explosive = explosive;
            shellInstance.GetComponent<ShellExplosion>().vampire = vampire;
            shellInstance.GetComponent<BulletMove>().phantom = phantom;
            shellInstance.GetComponent<ShellExplosion>().molotov = molotov;
            shellInstance.GetComponent<ShellExplosion>().shock = shock;
            if (FindNetworkObject(objectId).gameObject.tag == "MyTank")
            {
                m_Shell.gameObject.tag = "MyBullet";
            }
            //shellInstance.AddForce(transform.forward * m_MaxLaunchForce * 100 * Time.deltaTime);
        }
        public NetworkObject FindNetworkObject(ulong networkObjectId)
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject networkObject);
                return networkObject;
            }
            return null;
        }
    }

}