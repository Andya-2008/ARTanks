using System;
using UnityEngine;

public class RespawningCoin : Coin
{

    public event Action<RespawningCoin> OnCollected;
    private Vector3 previousPosition;
    private CoinSpawner cs;
    private float lastMove = 0f;
    [SerializeField] int MoveAfterSeconds = 30;
    [SerializeField] GameObject Coin;
    [SerializeField] GameObject Stack;
    public int coinValue;

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
        Debug.Log("Moved Coin");
        //return coinValue;
        return coinValue;
    }

    public override void moveCoin()
    { 
        Vector3 newpos = cs.GetRandomPoint();
        int randomRoll = UnityEngine.Random.Range(0,7);
        if(randomRoll == 0)
        {
            SetStack();
        }
        else
        {
            SetCoin();
        }
        this.transform.position = newpos;
        alreadyCollected = false;
    }
    public void SetStack()
    {
        Stack.SetActive(true);
        Coin.SetActive(false);
        coinValue = 25;
    }
    public void SetCoin()
    {
        Stack.SetActive(false);
        Coin.SetActive(true);
        coinValue = 10;
    }
}