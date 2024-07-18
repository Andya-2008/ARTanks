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
        else if (powerup.Contains("BulletSpeed"))
        {
            Debug.Log("2");
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().BulletSpeedRPC();
        }
        else if (powerup.Contains("BulletPower"))
        {
            Debug.Log("2");
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().BulletPowerRPC();
        }
        else if (powerup.Contains("TankSpeed"))
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
        Debug.Log("AddingPowerup");
        Transform powerupSlot = uiPowerupSlots[powerupsInitialized].transform;
        GameObject newPowerupUI = Instantiate(Resources.Load("UIPowerups/" + powerup), new Vector3(0,0,0), Quaternion.identity) as GameObject;
        Debug.Log("2");
        newPowerupUI.transform.SetParent(powerupSlot, false);
        powerupsInitialized++;
    } 

    public void ClosePowerupCanvas()
    {
        GameObject.Find("StartCanvas").GetComponent<Canvas>().enabled = false;
    }
}
