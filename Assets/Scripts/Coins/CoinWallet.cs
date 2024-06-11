using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private TMP_Text coinText;
    private void Start()
    {
        GameObject goCoinText = GameObject.Find("CoinValueText");
        coinText = goCoinText.GetComponent<TMP_Text>();
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Coin Trigger Enter");
        if (!col.TryGetComponent<Coin>(out Coin coin)) { return; }
        if (!IsOwner) { return; }

        int coinValue = coin.Collect();
        Debug.Log("Collect:" + col.name + ":" + coinValue.ToString());
        UpdateCoinsServerRPC(coinValue);
        //if (!IsServer) { return; }
        /*
        TotalCoins.Value += coinValue;
        Debug.Log("Total Coin Value:" + TotalCoins.Value);
        
        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds = new ulong[this.GetComponent<NetworkObject>().OwnerClientId];
        UpdateWalletClientRPC(TotalCoins.Value, rpcParams);
        */

    }


    [ServerRpc(RequireOwnership = false)]
    public void UpdateCoinsServerRPC(int coinValue, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        ulong[] singleTarget = new ulong[1];
        singleTarget[0] = clientId;

        TotalCoins.Value += coinValue;
        Debug.Log("Total Coin Value:" + TotalCoins.Value);

        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds = singleTarget;

        UpdateWalletClientRPC(TotalCoins.Value, rpcParams);

    }


    [ClientRpc]
    public void UpdateWalletClientRPC(int totalCoinValue, ClientRpcParams rpcParams = default)
    {
        coinText.text = totalCoinValue.ToString();
    }

}