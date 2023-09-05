using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class TankPlayer : NetworkBehaviour
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
        if(NetworkObject.IsOwner)
        {
            this.gameObject.tag = "MyTank";
        }
    }
}
