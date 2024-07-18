using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [SerializeField] List<GameObject> uiPowerupSlots = new List<GameObject>();
    int powerupsInitialized;
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
        if (powerup.Contains("BulletReload"))
        {
            Debug.Log("2");
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().BulletReloadRPC();
        }
        if (powerup.Contains("BulletSpeed"))
        {
            Debug.Log("2");
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().BulletSpeedRPC();
        }
        if (powerup.Contains("BulletPower"))
        {
            Debug.Log("2");
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().BulletPowerRPC();
        }
        if (powerup.Contains("TankSpeed"))
        {
            Debug.Log("2");
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().TankSpeedRPC();
        }
    }
    public void AddPowerupSlot()
    {
        Debug.Log("Added powerup");
    }

    public void AddPowerup(string powerup)
    {
        Transform powerupSlot = uiPowerupSlots[powerupsInitialized].transform;
        GameObject newPowerupUI = Instantiate(Resources.Load("UIPowerups/" + powerup), new Vector3(0,0,0), Quaternion.identity) as GameObject;
        newPowerupUI.transform.parent = powerupSlot;
        powerupsInitialized++; 
    }
}
