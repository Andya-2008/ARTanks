using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();
    public AudioSource m_CoinAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_CoinClip;            // Audio that plays when each shot is charging up.
    private TMP_Text coinText;
    float looterStartTime;
    [SerializeField] float looterInterval;
    [SerializeField] int looterCoinAmt = 5;
    [SerializeField] bool looter;
    private void Start()
    {
        GameObject goCoinText = GameObject.Find("CoinValueText");
        coinText = goCoinText.GetComponent<TMP_Text>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.TryGetComponent<Coin>(out Coin coin)) { return; }
        

        int coinValue = coin.Collect();
        coin.moveCoin();
        
        if (!IsOwner) { return; }
        UpdateCoinsServerRPC(coinValue);

        m_CoinAudio.clip = m_CoinClip;
        m_CoinAudio.Play();
        
        //if (!IsServer) { return; }
        /*
        TotalCoins.Value += coinValue;
        Debug.Log("Total Coin Value:" + TotalCoins.Value);
        
        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds = new ulong[this.GetComponent<NetworkObject>().OwnerClientId];
        UpdateWalletClientRPC(TotalCoins.Value, rpcParams);
        */

    }
    private void FixedUpdate()
    {
        if(looter && Time.time - looterStartTime >= looterInterval)
        {
            looterStartTime = Time.time;
            LooterAdded();
        }
    }
    public void LooterAdded()
    {
        UpdateCoinsServerRPC(looterCoinAmt);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateCoinsServerRPC(int coinValue, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        ulong[] singleTarget = new ulong[1];
        singleTarget[0] = clientId;

        TotalCoins.Value += coinValue;

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