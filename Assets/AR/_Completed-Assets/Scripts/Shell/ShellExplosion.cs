using Unity.Netcode;
using UnityEngine;

namespace Complete
{
    public class ShellExplosion : MonoBehaviour
    {
        public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
        public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
        public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
        public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
        public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
        public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
        public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.
        [SerializeField] float maxBulletDist = 2f;
        Transform originTrans;

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
                ExplodeBullet();
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (NetworkManager.Singleton.IsServer && other.gameObject.tag == "Tank" || other.gameObject.tag == "MyTank")
            {
                other.GetComponent<TankHealth>().TakeDamage();
            }
            ExplodeBullet();
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