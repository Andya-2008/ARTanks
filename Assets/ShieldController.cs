using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Netcode;
using System;
using Complete;

public class ShieldController : MonoBehaviour
{
    public float ShieldHealth = 200;
    [SerializeField] GameObject damageSphere;

    [SerializeField] GameObject shieldParent;
    public GameObject myTank;
    [SerializeField] bool reg;
    [SerializeField] bool golden;
    [SerializeField] bool absorption;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myTank = shieldParent.GetComponent<ShieldParent>().myTank;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeShieldDamage(float damage)
    {
        ShieldSpecialAction();
        damageSphere.GetComponent<PlayableDirector>().Play();
        ShieldHealth -= damage;
        Debug.Log(ShieldHealth);
        if (ShieldHealth <= 0)
        {
            Debug.Log("ShieldDeath");
            ShieldDeath();
        }
    }

    private void ShieldSpecialAction()
    {
        if(reg)
        {

        }
        if(golden)
        {
            myTank.GetComponent<CoinWallet>().UpdateCoinsServerRPC(40);
            Debug.Log("Coin wallet updated!");
        }
        if (absorption)
        {
            Debug.Log("Adding health");
            myTank.GetComponent<TankHealth>().AddHealth(35);
        }
    }

    public void ShieldDeath()
    {
        ShieldHealth = 100;
        //Play Shield breaking sound effect
        gameObject.SetActive(false);
    }
}
