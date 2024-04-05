using System.Collections.Generic;
using UnityEngine;
using Complete;
using Unity.Netcode;
public class ShootButton : MonoBehaviour
{
    public GameObject myTank;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TankShootButton()
    {
        TankShooting TankShoot = GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankShooting>();
        if (Time.time - TankShoot.startTime >= TankShoot.m_ReloadTime)
        {
            TankShoot.Fire();
        }
    }
}
