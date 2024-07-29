using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public void SpawnPowerup(string powerup, string powerupDisplayName, int numOfBullets, bool bulletbased)
    {
        GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().ActivateOrDeactivateTankPowerupRPC(powerup, true);

    }

    public void DisablePowerup(string powerup)
    {
        GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().ActivateOrDeactivateTankPowerupRPC(powerup, false);
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
        newPowerupUI.GetComponent<UIPowerup>().PowerupSlider.SetActive(false);
        Debug.Log("2");
        newPowerupUI.transform.SetParent(powerupSlot, false);
        powerupsInitialized++;
    } 

    public void ClosePowerupCanvas()
    {
        GameObject.Find("StartCanvas").GetComponent<Canvas>().enabled = false;
    }
}
