using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Complete
{
    public class ShellExplosion : MonoBehaviour
    {
        public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
        public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
        public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
        public float m_damage = 30f;                    // The amount of damage done if the explosion is centred on a tank.
        public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
        public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
        public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.
        [SerializeField] float maxBulletDist = 2f;
        Transform originTrans;

        public bool explosive;
        public bool vampire;
        public bool molotov;
        public bool shock;

        public float shockForce = 10f;

        TextMeshProUGUI debugText;

        public ulong myClientID;

        public GameObject myTank;
        public List<GameObject> tankList = new List<GameObject>();

        private void Start()
        {
            tankList.Add(GameObject.FindGameObjectWithTag("MyTank"));
            foreach(GameObject tank in GameObject.FindGameObjectsWithTag("Tank"))
            {
                tankList.Add(tank);
            }
            //debugText = GameObject.Find("DebugText").GetComponent<TextMeshProUGUI>();
            // If it isn't destroyed by then, destroy the shell after it's lifetime.
            Destroy(gameObject, m_MaxLifeTime);
            originTrans = this.transform;
            foreach(GameObject tank in tankList)
            {
                if(tank.GetComponent<NetworkObject>().OwnerClientId == myClientID)
                {
                    myTank = tank;
                }
            }
        }

        private void Update()
        {
            if (BulletDistance(originTrans) >= maxBulletDist)
            {
                Debug.Log("Exploding bullet");
                if (!explosive)
                {
                    ExplodeBullet();
                }
                else
                {
                    ExplosiveBulletTrigger();
                }
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter Called: " + other.name.ToString());
            Debug.Log("OnTriggerEnter tag: " + other.gameObject.tag);
            if (explosive)
            {
                ExplosiveBulletTrigger();
            }
            else
            {
                if(other.gameObject.tag == "Tank" || other.gameObject.tag == "MyTank")
                {
                    if (shock)
                    {
                        Debug.Log("Shock");
                        Vector3 shockVector = new Vector3(transform.forward.x, 1, transform.forward.z);
                        other.GetComponent<Rigidbody>().AddForce(shockVector * shockForce);
                    }
                }

                if (NetworkManager.Singleton.IsServer && other.gameObject.tag == "Shield")
                {
                    Debug.Log("Added damage to shield");
                    other.GetComponent<ShieldController>().TakeShieldDamage(m_damage);
                }
                else if (NetworkManager.Singleton.IsServer && (other.gameObject.tag == "Tank" || other.gameObject.tag == "MyTank"))
                {
                    other.GetComponent<TankHealth>().TakeDamage(m_damage);
                    if (vampire)
                    {
                        myTank.GetComponent<TankHealth>().AddHealth(m_damage);
                    }
                    if (molotov)
                    {
                        other.GetComponent<FireDamage>().startTime = Time.time;
                        other.GetComponent<FireDamage>().takingFire = true;
                    }
                }
                else if(NetworkManager.Singleton.IsServer)
                {
                    if (other.gameObject.tag == "Wall")
                    {
                        Debug.Log("Dealt damage to" + other.gameObject.name);
                        other.GetComponent<Wall_Health>().TakeDamage(m_damage);
                    }
                    if (other.gameObject.tag == "Repair")
                    {
                        Debug.Log("Dealt damage to" + other.gameObject.name);
                        other.GetComponent<RepairFactory>().TakeDamage(m_damage);
                    }
                    if (other.gameObject.tag == "TankFactory")
                    {
                        Debug.Log("Dealt damage to" + other.gameObject.name);
                        other.GetComponent<TankFactory>().TakeDamage(m_damage);
                    }
                }
                if (other.gameObject.tag == "Allowall")
                {
                    Debug.Log("Bullet Client ID: " + myClientID + " : Wall Owner ID: " + other.GetComponent<NetworkObject>().OwnerClientId);
                    debugText.text = "Bullet Owner ID: " + GetComponent<NetworkObject>().OwnerClientId + " : Wall Owner ID: " + other.GetComponent<NetworkObject>().OwnerClientId;
                    if (myClientID != other.GetComponent<NetworkObject>().OwnerClientId)
                    {
                        ExplodeBullet();
                        if (NetworkManager.Singleton.IsServer)
                        {
                            other.GetComponent<Wall_Health>().TakeDamage(m_damage);
                        }
                        
                    }
                    return;
                }
                ExplodeBullet();
            }
        }

        public void ExplosiveBulletTrigger()
        {
            Debug.Log("ExplosiveBulletTrigger called");
            // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

            // Go through all the colliders...
            for (int i = 0; i < colliders.Length; i++)
            {
                // ... and find their rigidbody.
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

                Debug.Log("Found colliders" + targetRigidbody.gameObject.name);

                // If they don't have a rigidbody, go on to the next collider.
                if (!targetRigidbody)
                    continue;
                if (targetRigidbody.gameObject.tag == "Wall")
                {
                    Wall_Health targetHealth = targetRigidbody.GetComponent<Wall_Health>();

                    targetHealth.TakeDamage(m_damage);
                    Debug.Log("Added damage to wall: " + targetHealth.gameObject.name);
                }
                else if(targetRigidbody.gameObject.tag == "Shield")
                {
                    targetRigidbody.GetComponent<ShieldController>().TakeShieldDamage(m_damage);
                }
                else
                {
                    // Add an explosion force.
                    targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

                    // Find the TankHealth script associated with the rigidbody.
                    TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

                    // If there is no TankHealth script attached to the gameobject, go on to the next collider.
                    if (!targetHealth)
                        continue;

                    // Deal this damage to the tank.
                    targetHealth.TakeDamage(m_damage);
                    Debug.Log("Added damage to tank: " + targetHealth.gameObject.name);
                }
            }

            // Unparent the particles from the shell.
            m_ExplosionParticles.transform.parent = null;

            // Play the particle system.
            m_ExplosionParticles.Play();

            // Play the explosion sound effect.
            m_ExplosionAudio.Play();

            // Once the particles have finished, destroy the gameobject they are on.
            Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);

            // Destroy the shell.
            Destroy(gameObject);
        }
        public void ExplodeBullet()
        {
            Debug.Log("2");
            // Unparent the particles from the shell.
            m_ExplosionParticles.transform.parent = null;

            // Play the particle system.
            m_ExplosionParticles.Play();

            // Play the explosion sound effect.
            m_ExplosionAudio.Play();

            // Once the particles have finished, destroy the gameobject they are on.
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            Destroy(m_ExplosionParticles.gameObject, mainModule.duration);

            // Destroy the shell.
            Destroy(gameObject);
        }

        public float BulletDistance(Transform bulletOriginPos)
        {
            float bulletDistance;
            bulletDistance = Vector3.Distance(bulletOriginPos.position, this.transform.position);
            return bulletDistance;
        }

    }
    /*
    public float BulletDistance(float timeSinceLaunched, float )
    {
        return bulletDistance;
    }
    */

}