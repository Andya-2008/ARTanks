using Complete;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurretShoot : NetworkBehaviour
{
    //SHOOTING RPC SHOULD ONLY BE CALLED ON THE OWNER

    public Rigidbody m_Shell;                   
    public Transform m_FireTransform;
    [SerializeField] float ShootInterval = 3f;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (Time.time - startTime > ShootInterval)
            {
                ShootRPC();
                startTime = Time.time;
            }
        }
    }
    [Rpc(SendTo.Everyone)]
    public void ShootRPC()
    {
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation, GameObject.Find("Battlefield1").transform) as Rigidbody;
    }
}
