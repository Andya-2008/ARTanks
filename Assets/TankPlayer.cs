using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class TankPlayer : NetworkBehaviour
{
    Vector3 origLocalPos;
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
            this.gameObject.layer = 6;
        }
        this.gameObject.transform.parent = GameObject.Find("Battlefield1").transform;
        origLocalPos = GameObject.Find("XR Origin").GetComponent<ImageTracking>().localpos;
        this.gameObject.transform.localPosition = origLocalPos;
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
    }
}
