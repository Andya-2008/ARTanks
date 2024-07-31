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
    TankMovement tankMovement;
    TankHealth tankHealth;
    TankShooting tankShooting;
    // Start is called before the first frame update
    void Start()
    {
        tankMovement = this.GetComponent<TankMovement>();
        tankShooting = this.GetComponent<TankShooting>();
        tankHealth = this.GetComponent<TankHealth>();
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
                tankShooting.m_ReloadTime -= .75f;
            }
            else
            {
                tankShooting.m_ReloadTime += .75f;
            }
        }
        else if (poweruptype.Contains("BulletSpeed"))
        {
            if (activate)
            {
                tankShooting.m_BulletSpeed += .005f;
            }
            else
            {
                tankShooting.m_BulletSpeed -= .005f;
            }
        }
        else if (poweruptype.Contains("BulletPower"))
        {
            if (activate)
            {
                tankShooting.m_BulletPower += 50;
            }
            else
            {
                tankShooting.m_BulletPower -= 50;
            }
        }
        else if (poweruptype.Contains("TankSpeed"))
        {
            if(activate)
            {
                tankMovement.m_Speed += .3f;
            }
            else
            {
                tankMovement.m_Speed -= .3f;
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
        else if (poweruptype.Contains("Overdrive"))
        {
            if (activate)
            {
                if (tankHealth.m_CurrentHealth - tankHealth.m_StartingHealth / 5 > 0)
                {
                    tankHealth.m_CurrentHealth -= tankHealth.m_StartingHealth / 5;
                }
                else if(tankHealth.m_CurrentHealth >= 5)
                {
                    tankHealth.m_CurrentHealth = 5;
                }
                tankMovement.m_Speed *= 1.4f;
                tankShooting.m_BulletSpeed *= 1.3f;
                tankShooting.m_BulletPower *= 1.3f;
                tankShooting.m_ReloadTime /= 1.5f;
            }
            else
            {
                tankMovement.m_Speed /= 1.4f;
                tankShooting.m_BulletSpeed /= 1.3f;
                tankShooting.m_BulletPower /= 1.3f;
                tankShooting.m_ReloadTime *= 1.5f;
            }
        }
        else if (poweruptype.Contains("Homing"))
        {
            if (activate)
            {
                tankShooting.homing = true;
            }
            else
            {
                tankShooting.homing = false;
            }
        }
    }

    public void PassiveAddHealth()
    {
        tankHealth.m_CurrentHealth += healthAdditionTime * tankHealth.m_StartingHealth / 12;
        if(tankHealth.m_CurrentHealth >= tankHealth.m_StartingHealth)
        {
            tankHealth.m_CurrentHealth = tankHealth.m_StartingHealth;
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

