using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPowerup(string powerup)
    {
        if (powerup == "BulletReload_Powerup")
        {
            Debug.Log("2");
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().BulletReloadRPC();
        }
        if (powerup == "BulletSpeed_Powerup")
        {
            Debug.Log("2");
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().BulletSpeedRPC();
        }
        if (powerup == "BulletPower_Powerup")
        {
            Debug.Log("2");
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().BulletPowerRPC();
        }
    }
}
