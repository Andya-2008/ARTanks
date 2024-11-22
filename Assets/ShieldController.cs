using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Netcode;

public class ShieldController : MonoBehaviour
{
    [SerializeField] float ShieldHealth = 200;
    [SerializeField] GameObject damageSphere;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeShieldDamage(float damage)
    {
        damageSphere.GetComponent<PlayableDirector>().Play();
        ShieldHealth -= damage;
        Debug.Log(ShieldHealth);
        if (ShieldHealth <= 0)
        {
            Debug.Log("ShieldDeath");
            ShieldDeath();
        }
    }

    public void ShieldDeath()
    {
        ShieldHealth = 100;
        //Play Shield breaking sound effect
        gameObject.SetActive(false);
    }
}
