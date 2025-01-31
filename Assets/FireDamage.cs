using Complete;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

public class FireDamage : MonoBehaviour
{
    public bool takingFire;
    public float fireTime;
    public float fireDamage;
    [SerializeField] GameObject FireEffect;
    public float startTime;
    float initScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (takingFire && Time.time - startTime >= fireTime)
        {
            takingFire = false;
            startTime = Time.time;
            FireEffect.SetActive(false);
            fireTime = 20;
            //FireEffect.transform.localScale = new Vector3(initScale * ((fireTime - (Time.time - startTime)) / fireTime), initScale * (Time.time - startTime) / fireTime, initScale * (Time.time - startTime) / fireTime);
        }
        if(takingFire)
        {
            if (!FireEffect.activeSelf)
            {
                Debug.Log("Not active");
                FireEffect.SetActive(true);
                FireEffect.GetComponent<PlayableDirector>().Play();
                FireEffect.GetComponent<ParticleSystem>().Play();
                initScale = FireEffect.transform.localScale.x;
            }
            TakeFireDamage();
        }
    }

    public void TakeFireDamage()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            this.GetComponent<TankHealth>().TakeDamage(Time.fixedDeltaTime * fireDamage / fireTime);
        }

        float m_VerticalInputValue = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>().Vertical;
        float m_HorizontalInputValue = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>().Horizontal;

        if(m_VerticalInputValue != 0 || m_HorizontalInputValue != 0)
        {
            fireTime -= Time.deltaTime;
        }
        //FireEffect.transform.localScale = new Vector3(initScale * ((fireTime - (Time.time - startTime)) / fireTime), initScale * (Time.time - startTime) / fireTime, initScale * (Time.time - startTime) / fireTime);

    }
}
