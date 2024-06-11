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
        Debug.Log("Card Object Spawn: Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log("Card Object Spawn: On Network Spawn");
        this.gameObject.transform.parent = GameObject.Find("Battlefield1").transform;
        if (!IsServer)
        {
            Debug.Log("GetLocalPosServerRPC");
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
        Debug.Log("localpos3:" + transform.localPosition);
        
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
        Debug.Log(this.gameObject.name + " localpos3:" + this.transform.localPosition);
        if (GetComponent<ClientNetworkTransform>() != null)
        {
            GetComponent<ClientNetworkTransform>().enabled = true;
        }
    }
}
