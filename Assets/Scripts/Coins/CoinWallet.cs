using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Coin Trigger Enter");
        if (!col.TryGetComponent<Coin>(out Coin coin)) { return; }

        Debug.Log("Collect!");
        int coinValue = coin.Collect();

        if (!IsServer) { return; }

        TotalCoins.Value += coinValue;
        Debug.Log("Total Coin Value:" + TotalCoins.Value);
    }
}
