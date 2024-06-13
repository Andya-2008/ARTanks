using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RespawningCoin : Coin
{

    public event Action<RespawningCoin> OnCollected;
    private Vector3 previousPosition;
    private CoinSpawner cs;
    private float lastMove = 0f;
    [SerializeField] int MoveAfterSeconds = 30;

    private void Start()
    {
        cs = GameObject.Find("Battlefield1").GetComponent<CoinSpawner>();
        alreadyCollected = false;

    }

    public override void OnNetworkSpawn() {

    }
    private void Update()
    {
        if (IsServer)
        {
            if (Time.time - lastMove > MoveAfterSeconds)
            {
                lastMove = Time.time;
                moveCoin();
            }
        }
    }

    public override int Collect()
    {
        Debug.Log("Respawning Coin Collect");

        int coinValue = 10;

        if (!IsServer)
        {
            //Show(false);
            //Debug.Log("Coin COllected Not Server!");
            return coinValue;
        }
        
        if (alreadyCollected)
        {
            Debug.Log("Coin Collected Already COllected");
            moveCoin();
            return 0;
        }

        alreadyCollected = true;
        moveCoin();
        Debug.Log("Moved Coin");
        //return coinValue;
        return coinValue;
    }

    public void moveCoin()
    { 
        Vector3 newpos = cs.GetRandomPoint();
        this.transform.position = newpos;
        alreadyCollected = false;
    }


    

}