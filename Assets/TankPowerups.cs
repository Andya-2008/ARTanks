using Complete;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TankPowerups : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Rpc(SendTo.Everyone)]
    public void BulletSpeedRPC()
    {
        Debug.Log("3");
        this.GetComponent<TankShooting>().m_ReloadTime -= .75f;
    }
}
