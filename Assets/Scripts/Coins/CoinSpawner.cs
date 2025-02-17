using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class CoinSpawner : NetworkBehaviour
{
    //[SerializeField] private RespawningCoin coinPrefab;
    [SerializeField] private GameObject goCoin;

    [SerializeField] private int maxCoins = 50;
    [SerializeField] private int coinValue = 10;
    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 ySpawnRange;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float spawningInterval = 10;

    private Collider[] coinBuffer = new Collider[1];
    private float coinRadius;
    private float lastSpawn = 0;
    private bool spawnOn = false;
    private GameObject battleField;
    private int coinCount = 0;

    /*
    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        coinRadius = coinPrefab.GetComponent<CapsuleCollider>().radius;

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }
    */

    private void Start()
    {
        Debug.Log("CoinSpawner:NetworkSpawn");
        lastSpawn = Time.time;
        battleField = this.gameObject;

    }

    private void Update()
    {

        if (!IsServer) { return; }
        if (coinCount < maxCoins)
        {
            if (Time.time - lastSpawn > spawningInterval)
            {
                SpawnCoin();
                coinCount += 1;
                lastSpawn = Time.time;
            }
        }


    }
    public void SpawnCoins()
    {

        Debug.Log("Spawn Coins (IsClient):" + IsClient);
        Debug.Log("Spawn Coins (IsServer):" + IsServer);
        Debug.Log("Spawn Coins (IsHost):" + IsHost);
        Debug.Log("Spawn Coins (IsOwner):" + IsOwner);
        Debug.Log("Spawn Coins (IsLocalPlayer):" + IsLocalPlayer);


        if (!IsServer) { return; }
        spawnOn = true;

        coinRadius = goCoin.GetComponent<CapsuleCollider>().radius;

        for (int i = 0; i < maxCoins; i++)
        {
            Debug.Log("Spawn Coin");
            SpawnCoin();
        }


    }




    private void SpawnCoin()
    {
        int stackChance = Random.Range(0, 7);
        GameObject coinInstance;
        coinInstance = Instantiate(goCoin, GetRandomPoint(), Quaternion.identity, transform.parent);
        if (stackChance == 0)
        {
            coinInstance.GetComponent<RespawningCoin>().SetStack();
        }
        else
        {
            coinInstance.GetComponent<RespawningCoin>().SetCoin();
        }

        coinInstance.GetComponent<NetworkObject>().Spawn();
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        //coin.Reset();
    }

    public Vector3 GetRandomPoint()
    {
        float x = 0;
        float y = 0;
        x = Random.Range(xSpawnRange.x, xSpawnRange.y);
        y = Random.Range(ySpawnRange.x, ySpawnRange.y);
        Vector3 spawnPoint = new Vector3(transform.position.x + x, this.gameObject.transform.position.y, transform.position.z + y);
        return spawnPoint;
    }

    public Vector3 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;
        while (true)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector3 spawnPoint = new Vector3(x, 1, y);
            int numColliders = Physics.OverlapSphereNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}