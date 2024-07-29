using Complete;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TankPowerups : NetworkBehaviour
{
    public bool addHealth;
    float timeSinceLastAddHealth;
    public float healthAdditionTime = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - timeSinceLastAddHealth >= healthAdditionTime && addHealth)
        {
            timeSinceLastAddHealth = Time.time;
            PassiveAddHealth();
        }
    }


    [Rpc(SendTo.Everyone)]
    public void ActivateOrDeactivateTankPowerupRPC(string poweruptype, bool activate)
    {
        
        if (poweruptype.Contains("BulletReload"))
        {
            if (activate)
            {
                this.GetComponent<TankShooting>().m_ReloadTime -= .75f;
            }
            else
            {
                this.GetComponent<TankShooting>().m_ReloadTime += .75f;
            }
        }
        else if (poweruptype.Contains("BulletSpeed"))
        {
            if (activate)
            {
                this.GetComponent<TankShooting>().m_BulletSpeed += .005f;
            }
            else
            {
                this.GetComponent<TankShooting>().m_BulletSpeed -= .005f;
            }
        }
        else if (poweruptype.Contains("BulletPower"))
        {
            if (activate)
            {
                this.GetComponent<TankShooting>().m_BulletPower += 50;
            }
            else
            {
                this.GetComponent<TankShooting>().m_BulletPower -= 50;
            }
        }
        else if (poweruptype.Contains("TankSpeed"))
        {
            if(activate)
            {
                this.GetComponent<TankMovement>().m_Speed += .3f;
            }
            else
            {
                this.GetComponent<TankMovement>().m_Speed -= .3f;
            }
        }
        else if (poweruptype.Contains("Health"))
        {
            if(activate)
            {
                addHealth = true;
            }
            else
            {
                addHealth = false;
            }
        }
    }

    public void PassiveAddHealth()
    {
        this.GetComponent<TankHealth>().m_CurrentHealth += healthAdditionTime * this.GetComponent<TankHealth>().m_StartingHealth / 12;
        if(this.GetComponent<TankHealth>().m_CurrentHealth >= this.GetComponent<TankHealth>().m_StartingHealth)
        {
            this.GetComponent<TankHealth>().m_CurrentHealth = this.GetComponent<TankHealth>().m_StartingHealth;
            EndPowerupEarly();
        }
    }

    public void EndPowerupEarly()
    {
        foreach (GameObject uiPowerup in GameObject.FindGameObjectsWithTag("UIPowerup"))
        {
            if (uiPowerup.name.Contains("Health"))
            {
                uiPowerup.GetComponent<UIPowerup>().PowerupSlider.GetComponent<PowerupSliderController>().EndPowerupEarly();
                break;
            }
        }
    }
}

