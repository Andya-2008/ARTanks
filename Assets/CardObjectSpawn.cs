using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;

public class CardObjectSpawn : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnNetworkSpawn()
    {
        this.gameObject.transform.parent = GameObject.Find("Battlefield1").transform;
        if (!IsServer)
        {
            GetLocalPosServerRPC();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetLocalPosServerRPC(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        ulong[] singleTarget = new ulong[1];
        singleTarget[0] = clientId;

        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds = singleTarget;
        
        SetLocalPosClientRPC(transform.localPosition, rpcParams);
        if (GetComponent<ClientNetworkTransform>() != null)
        {
            GetComponent<ClientNetworkTransform>().enabled = true;
        }
    }
    [ClientRpc]
    public void SetLocalPosClientRPC(Vector3 localpos, ClientRpcParams rpcParams = default)
    {
        this.transform.localPosition = localpos;
        if (GetComponent<ClientNetworkTransform>() != null)
        {
            GetComponent<ClientNetworkTransform>().enabled = true;
        }
    }
}
