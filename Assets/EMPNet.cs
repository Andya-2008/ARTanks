using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class EMPNet : NetworkBehaviour
{
    public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
    public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.


    public float m_CurrentHealth;                      // How much health the tank currently has.
    private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?
    [SerializeField] float timedHealthReduction = 0.01f;
    float powerStartTime;
    [SerializeField] float timeToDeath;
    float deathTimer;

    [SerializeField] ParticleSystem turretAnimation;
    [SerializeField] float shootInterval = 15;
    [SerializeField] float freezeTime = 3;
    float startTime;
    float freezeStartTime;
    public List<Transform> enemyTanks = new List<Transform>();
    [SerializeField] float damage;
    bool freeze = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
    }


    private void OnEnable()
    {
        // When the tank is enabled, reset the tank's health and whether or not it's dead.
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        // Update the health slider's value and color.
        SetHealthUI();
    }


    public void TakeDamage(float damage)
    {
        TakeDamageRPC(this.GetComponent<NetworkObject>().NetworkObjectId, damage);
    }
    [Rpc(SendTo.Everyone)] //server owns this object but client can request a spawn
    public void TakeDamageRPC(ulong objectId, float damage)
    {
        // Reduce current health by the amount of damage done.
        GetComponent<Turret>().m_CurrentHealth -= damage;

        // Change the UI elements appropriately.
        SetHealthUI();

        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (GetComponent<Turret>().m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        Destroy(this.gameObject);
    }
    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        m_Slider.value = m_CurrentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    private void FixedUpdate()
    {
        if (m_Dead && Time.time - deathTimer > timeToDeath)
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(this.gameObject);
        }
        SetHealthUI();
        if (Time.time - startTime > .01f)
        {
            m_CurrentHealth -= timedHealthReduction;
            if (m_CurrentHealth < 0f && !m_Dead)
            {
                OnDeath();
            }
            startTime = Time.time;
        }

        if (GetComponent<NetworkObject>().IsOwner)
        {
            enemyTanks.Clear();
            foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Tank"))
            {
                Debug.Log("Added tank: " + tank.name);
                enemyTanks.Add(tank.transform);
            }
            string t1 = enemyTanks[0].name;
            string t2 = "";
            string t3 = "";
            if (enemyTanks.Count > 1)
                t2 = enemyTanks[1].name;
            if (enemyTanks.Count > 2)
                t3 = enemyTanks[2].name;
            if (Time.time - powerStartTime > shootInterval)
            {
                Debug.Log("Shot pulse: " + t1 + ", " + t2 + ", and " + t3);
                FreezeMovementRPC(t1, t2, t3);
                FireAnimationRPC();

                freezeStartTime = Time.time;
                freeze = true;
                powerStartTime = Time.time;
            }
            if(Time.time - freezeStartTime > freezeTime && freeze)
            {
                freeze = false;
                UnfreezeMovementRPC(t1, t2, t3);
            }
        }
    }
    [Rpc(SendTo.Everyone)]
    public void FireAnimationRPC()
    {
        turretAnimation.Play();
    }
    [Rpc(SendTo.Everyone)]
    public void FreezeMovementRPC(string tank1, string tank2, string tank3)
    {
        if(tank1 != "")
        {
            GameObject.Find(tank1).GetComponent<TankMovement>().enabled = false;
        }
        if (tank2 != "")
        {
            GameObject.Find(tank2).GetComponent<TankMovement>().enabled = false;
        }
        if (tank3 != "")
        {
            GameObject.Find(tank3).GetComponent<TankMovement>().enabled = false;
        }
    }
    [Rpc(SendTo.Everyone)]
    public void UnfreezeMovementRPC(string tank1, string tank2, string tank3)
    {
        if (tank1 != "")
        {
            GameObject.Find(tank1).GetComponent<TankMovement>().enabled = true;
        }
        if (tank2 != "")
        {
            GameObject.Find(tank2).GetComponent<TankMovement>().enabled = true;
        }
        if (tank3 != "")
        {
            GameObject.Find(tank3).GetComponent<TankMovement>().enabled = true;
        }
    }
}
