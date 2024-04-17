using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class TankPlayer : NetworkBehaviour
{
    Vector3 origLocalPos;
    public ulong TankID;
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
        if(NetworkObject.IsOwner)
        {
            this.gameObject.tag = "MyTank";
            SendTankIDRPC(NetworkManager.Singleton.LocalClientId);
            this.gameObject.layer = 6;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SendTankIDRPC(ulong ID)
    {
        TankID = ID;
    }
}
