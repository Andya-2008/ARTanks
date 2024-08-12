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

        private void Start()
        {
            // If it isn't destroyed by then, destroy the shell after it's lifetime.
            Destroy(gameObject, m_MaxLifeTime);
            originTrans = this.transform;

        }

        private void Update()
        {
            if (BulletDistance(originTrans) >= maxBulletDist)
            {
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
            if (explosive)
            {
                ExplosiveBulletTrigger();
            }
            else
            {
                if (NetworkManager.Singleton.IsServer && (other.gameObject.tag == "Tank" || other.gameObject.tag == "MyTank"))
                {
                    other.GetComponent<TankHealth>().TakeDamage(m_damage);
                }
                else if(NetworkManager.Singleton.IsServer && other.gameObject.tag == "Wall")
                {
                    other.GetComponent<Wall_Health>().TakeDamage(m_damage);
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