using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Complete;
public class TankPlayer : NetworkBehaviour
{
    Vector3 origLocalPos;
    public ulong TankID;
    public bool invisible;
    [SerializeField] List<MeshRenderer> tankParts = new List<MeshRenderer>();

    [SerializeField] Canvas tankCanvas;
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
        GameObject.Find("GameManager").GetComponent<GameManager>().AddTank(this.GetComponent<TankMovement>());
    }

    [Rpc(SendTo.Everyone)]
    public void SendTankIDRPC(ulong ID)
    {

        TankID = ID;
    }

    public void ToggleInvisibility(bool on)
    {
        if(on)
        {
            foreach(MeshRenderer tankPart in tankParts)
            {
                //Make animation here
                tankPart.enabled = false;
            }
            tankCanvas.enabled = false;
        }
        if(!on)
        {
            foreach (MeshRenderer tankPart in tankParts)
            {
                //Make animation here
                tankPart.enabled = true;
            }
            tankCanvas.enabled = true;
        }
    }
}
