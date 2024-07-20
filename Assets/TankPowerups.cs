using Complete;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TankPowerups : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    [Rpc(SendTo.Everyone)]
    public void ActivateTankPowerupRPC(string poweruptype)
    {
        if (poweruptype.Contains("BulletReload"))
        {
            this.GetComponent<TankShooting>().m_ReloadTime -= .75f;
        }
        else if (poweruptype.Contains("BulletSpeed"))
        {
            this.GetComponent<TankShooting>().m_BulletSpeed += .005f;
        }
        else if (poweruptype.Contains("BulletPower"))
        {
            this.GetComponent<TankShooting>().m_BulletPower += 50;
        }
        else if (poweruptype.Contains("TankSpeed"))
        {
            this.GetComponent<TankMovement>().m_Speed += 6f;
        }
    }
    public void DeactivateTankPowerupRPC(string poweruptype)
    {
        if (poweruptype.Contains("BulletReload"))
        {
            this.GetComponent<TankShooting>().m_ReloadTime += .75f;
        }
        else if (poweruptype.Contains("BulletSpeed"))
        {
            this.GetComponent<TankShooting>().m_BulletSpeed -= .005f;
        }
        else if (poweruptype.Contains("BulletPower"))
        {
            this.GetComponent<TankShooting>().m_BulletPower -= 50;
        }
        else if (poweruptype.Contains("TankSpeed"))
        {
            this.GetComponent<TankMovement>().m_Speed -= 6f;
        }
    }
}

