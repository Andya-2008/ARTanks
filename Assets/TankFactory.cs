using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TankFactory : MonoBehaviour
{
    public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
    public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.

    public float m_CurrentHealth;                      // How much health the tank currently has.
    private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?
    [SerializeField] float timedHealthReduction = 0.01f;
    float startTime;
    float botStartTime;
    [SerializeField] float botInterval = .5f;
    [SerializeField] GameObject realWallParent;
    [SerializeField] float timeToDeath;
    float deathTimer;
    TextMeshProUGUI debugText;
    private void Start()
    {
    }

    private void Awake()
    {

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
        GetComponent<TankFactory>().m_CurrentHealth -= damage;

        // Change the UI elements appropriately.
        SetHealthUI();

        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (GetComponent<TankFactory>().m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }

    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        m_Slider.value = m_CurrentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {

        // Set the flag so that this function is only called once.
        m_Dead = true;
        /*
        Debug.Log("OnDeath: " + this.gameObject.name);
        Debug.Log("OnDeath: " + this.GetComponent<TankMovement>().m_PlayerNumber);
        GameObject.Find("GameManager").GetComponent<GameManager>().PlayerDead(this.gameObject);
        // Move the instantiated explosion prefab to the tank's position and turn it on.
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        // Play the particle system of the tank exploding.
        m_ExplosionParticles.Play();

        // Play the tank explosion sound effect.
        m_ExplosionAudio.Play();

        // Turn the tank off.
        gameObject.SetActive(false);
        */
        Debug.Log("Wall dead");
        realWallParent.SetActive(false);
        deathTimer = Time.time;
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

    public void FixedUpdate()
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
        if (Time.time - botStartTime > botInterval)
        {
            botStartTime = Time.time;
            GetComponent<NetworkObjectSpawner>().AddNewObject("MiniTank");
        }
    }
}
