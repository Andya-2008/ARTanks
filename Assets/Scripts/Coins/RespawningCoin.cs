using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Update()
    {
        if (Time.time - lastMove > MoveAfterSeconds)
        {
            lastMove = Time.time;
            moveCoin();
        }
    }

    public override int Collect()
    {
        /*
        if (!IsServer)
        {
            Show(false);
            Debug.Log("Coin COllected Not Server!");
            return 0;
        }
        */
        if (alreadyCollected)
        {
            Debug.Log("Coin Collected Already COllected");
            moveCoin();
            return 0;
        }

        alreadyCollected = true;

        //OnCollected?.Invoke(this);


        moveCoin();
        Debug.Log("Moved Coin");
        //return coinValue;
        return 10;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }

    private void moveCoin()
    {
        Vector3 newpos = cs.GetRandomPoint();
        this.transform.position = newpos;
        alreadyCollected = false;
    }
}