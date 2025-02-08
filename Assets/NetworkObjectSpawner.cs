using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkObjectSpawner : NetworkBehaviour
{
    [SerializeField] GameObject repairObj;
    [SerializeField] GameObject tankObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AddNewObject(string objName)
    {
        Vector3 localpos = worldToLocal(this.transform.position);
        if (GetComponent<NetworkObject>().IsOwner)
        {
            AddObjServerRpc(transform.position, objName);
        }

    }
    [ServerRpc(RequireOwnership = false)]
    private void AddObjServerRpc(Vector3 localpos, string objN, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) { return; }
        if (GameObject.Find("Battlefield1") == null) { return; }
        GameObject instObj;
        if (objN == "RepairBot")
        {
            instObj = repairObj;
        }
        else
        {
            instObj = tankObj;
        }
        var clientId = serverRpcParams.Receive.SenderClientId;
        Vector3 newPos = new Vector3(localpos.x, 0, localpos.z);
        GameObject prefab = Instantiate(instObj, newPos, Quaternion.identity);

        //Vector3 prefabLocalPos = trackedImagePos - battleField.transform.position;


        NetworkObject netObj = prefab.GetComponent<NetworkObject>();
        prefab.SetActive(true);
        prefab.transform.parent = GameObject.Find("Battlefield1").transform;
        prefab.transform.localPosition = newPos;
        netObj.SpawnWithOwnership(clientId, false);

    }
    private Vector3 worldToLocal(Vector3 worldpos)
    {
        return GameObject.Find("Battlefield1").transform.InverseTransformPoint(worldpos);
    }
}
