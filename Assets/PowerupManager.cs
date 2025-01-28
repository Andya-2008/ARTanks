using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerupManager : MonoBehaviour
{
    [SerializeField] List<GameObject> addedPowerupSlots = new List<GameObject>();
    [SerializeField] List<GameObject> activePowerupSlots = new List<GameObject>();
    int powerupsInitialized;
    [SerializeField] PowerupPopup popup;
    [SerializeField] GameObject powerupAdding;
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
        GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().ActivateOrDeactivateTankPowerup(powerup, true);

    }

    public void DisablePowerup(string powerup)
    {
        GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPowerups>().ActivateOrDeactivateTankPowerup(powerup, false);
    }
    public void AddPowerupSlot()
    {
        //Adding poweurp slot
        if(GameObject.FindGameObjectWithTag("MyTank"))
        if (GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().TotalCoins.Value >= 100)
        {
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().UpdateCoinsServerRPC(-100);
            if (addedPowerupSlots.Count > 0)
            {
                activePowerupSlots.Add(addedPowerupSlots[0]);
                addedPowerupSlots[0].GetComponent<RawImage>().enabled = true;
                if (addedPowerupSlots.Count == 1)
                {
                    powerupAdding.SetActive(false);
                }
                addedPowerupSlots.RemoveAt(0);
                popup.GetComponent<PowerupPopup>().extendRadius = .46f;
                popup.GetComponent<PowerupPopup>().closing = false;
                popup.GetComponent<PowerupPopup>().CloseMenu();
            }
        }
    }

    public void AddPowerup(string powerup)
    {
        Debug.Log("AddingPowerup");
        Transform powerupSlot = activePowerupSlots[powerupsInitialized].transform;
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
