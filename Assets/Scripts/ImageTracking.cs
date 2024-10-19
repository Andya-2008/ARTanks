using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Unity.Netcode;
using UnityEngine.InputSystem.LowLevel;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : NetworkBehaviour

{
    [SerializeField] TextMeshProUGUI DebugText;
    [SerializeField] private Dictionary<string, GameObject> placePrefabs = new Dictionary<string, GameObject>();
    [SerializeField] private List<GameObject> placeablePrefabs = new List<GameObject>();
    
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    //private List<ARTrackedImage> newImages = new List<ARTrackedImage>();
    private ARTrackedImageManager trackedImageManager;
    private bool hasSpawnedTank = false;

    float startTime;
    bool started;
    GameObject prefab;
    private string strDebug = "";
    private string tiDebug = "";
    //public NetworkVariable<Vector3> localpos = new NetworkVariable<Vector3>();
    //public Vector3 localpos;

    ARTrackedImage trackImage;
    public GameObject battleField;
    private void Awake()
    {
        trackedImageManager = FindFirstObjectByType<ARTrackedImageManager>();
    }
    private void Update()
    {
        /*
        string strDebug = "";
        if ( battleField!=null)
        {
            strDebug += "battlefield:" + battleField.transform.position + "\n";
        }

        GameObject mytank = GameObject.FindGameObjectWithTag("MyTank");
        if ( mytank!=null)
        {
            strDebug += "MyTank:" + mytank.transform.localPosition + "\n";
        }

        GameObject[] gotanks = GameObject.FindGameObjectsWithTag("Tank");
        foreach (GameObject go in gotanks) {
            strDebug += "Tank:" + go.transform.localPosition + "\n";
        }

        GameObject[] goturrets = GameObject.FindGameObjectsWithTag("Turret");
        foreach (GameObject go in goturrets)
        {
            strDebug += "Turret:" + go.transform.localPosition + "\n";
        }

        DebugTxt(strDebug);

        */
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B");
            ARTrackedImagePlus artip = new ARTrackedImagePlus();
            artip.name = "Battlefield1";
            GameObject go = new GameObject();
            artip.transform = go.transform;
            artip.transform.position = new Vector3(0, 0, 0);
            artip.transform.rotation = Quaternion.identity;
            StartCoroutine(CreateObject(artip));
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T");
            ARTrackedImagePlus artip = new ARTrackedImagePlus();
            artip.name = "Tank1";
            GameObject go = new GameObject();
            artip.transform = go.transform;
            artip.transform.position = new Vector3(0.2f, 0, 0);
            artip.transform.rotation = Quaternion.identity;
            StartCoroutine(CreateObject(artip));
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Y");
            ARTrackedImagePlus artip = new ARTrackedImagePlus();
            artip.name = "Tank2";
            GameObject go = new GameObject();
            artip.transform = go.transform;
            artip.transform.position = new Vector3(-0.2f, 0, 0);
            artip.transform.rotation = Quaternion.identity;
            StartCoroutine(CreateObject(artip));
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("U");
            ARTrackedImagePlus artip = new ARTrackedImagePlus();
            artip.name = "Turret";
            GameObject go = new GameObject();
            artip.transform = go.transform;
            artip.transform.position = new Vector3(-0.2f, 0, -0.2f);
            artip.transform.rotation = Quaternion.identity;
            StartCoroutine(CreateObject(artip));
        }




    }
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
        
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
       
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (spawnedPrefabs.ContainsKey(trackedImage.referenceImage.name)) { return; }
            ARTrackedImagePlus artip = new ARTrackedImagePlus();
            artip.trackedImage = trackedImage;
            StartCoroutine(CreateObject(artip));
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.referenceImage.name.Contains("Battlefield"))
            {
                updateObject(trackedImage, "Battlefield");
            }
        }
    }
    
    public void DebugTxt(string txt)
    {
        DebugText.text = txt;
    }

    private void updateObject(ARTrackedImage trackedImage, string updatedImg) {
        if (updatedImg == "Battlefield")
        {
            if (GameObject.FindGameObjectWithTag("Battlefield"))
            {
                battleField = GameObject.FindGameObjectWithTag("Battlefield");
                battleField.transform.position = trackedImage.transform.position;
                battleField.transform.rotation = trackedImage.transform.rotation;

            }
        }
        }
    IEnumerator CreateObject(ARTrackedImagePlus trackedImage)
    {
        string name = "";

        if (trackedImage.trackedImage != null)
        {
            while (trackedImage.trackedImage.trackingState != TrackingState.Tracking)
            {
                yield return null;
            }


            name = trackedImage.trackedImage.referenceImage.name;
            trackedImage.transform = trackedImage.trackedImage.transform;
        }
        else
        {
            // For desktop testing purposes
            name = trackedImage.name;

        }

        GameObject pf = new GameObject();

        int prefabId = 0;
        foreach (GameObject go in placeablePrefabs)
        {

            if (go.name == name)
            {
                pf = go;
                prefabId = placeablePrefabs.IndexOf(go);
            }
        }
        if (pf.tag == "Battlefield")
        {
            if (!GameObject.FindGameObjectWithTag("Battlefield"))
            {
                Quaternion worldRotation = new Quaternion(trackedImage.transform.localRotation.x, trackedImage.transform.localRotation.y, trackedImage.transform.localRotation.z, trackedImage.transform.localRotation.w);
                //worldRotation *= Quaternion.Euler(90, 0, 0);
                prefab = Instantiate(pf, trackedImage.transform.position, worldRotation);
                battleField = prefab;

                prefab.name = pf.name;
                spawnedPrefabs.Add(name, prefab);
                prefab.SetActive(true);

                NetworkObject netObj = prefab.GetComponent<NetworkObject>();
                netObj.Spawn();
            }
        }
        if (pf.tag == "Tank" || pf.tag == "MyTank")
        {
            if (!hasSpawnedTank) {
                hasSpawnedTank = true;
                Vector3 localpos = worldToLocal(trackedImage.transform.position, battleField.transform);
                Debug.Log(localpos);
                SpawnPlayerServerRpc(name, localpos, prefabId);
            }
        }
        if(pf.tag == "Powerup")
        {
            Debug.Log("0");
            GameObject.Find("PowerupManager").GetComponent<PowerupManager>().AddPowerup(pf.name);
            /*
            if (battleField != null)
            {

                Debug.Log("1");
                Quaternion worldRotation = new Quaternion(trackedImage.transform.localRotation.x, trackedImage.transform.localRotation.y, trackedImage.transform.localRotation.z, trackedImage.transform.localRotation.w);
                //worldRotation *= Quaternion.Euler(90, 0, 0);
                prefab = Instantiate(pf, trackedImage.transform.position, worldRotation);
                if(pf.name == "BulletSpeed_Powerup")
                {
                    BSP = prefab;
                }
                if(pf.name == "BulletPower_Powerup")
                {
                    BPP = prefab;
                }
                if(pf.name == "BulletReload_Powerup")
                {
                    BRP = prefab;
                }
                //GameObject.Find("PowerupManager").GetComponent<PowerupManager>().SpawnPowerup(name);
                
        }*/
    }
        

        yield return null;
        //prefab.transform.position = trackedImage.transform.position;

    }

/*
[ServerRpc(RequireOwnership = false)]
public void SetLocalPosServerRPC(Vector3 p_LocalPos)
{
    localpos = p_LocalPos;
}*/


[ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(string name, Vector3 localpos, int prefabId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) { return; }
        if (battleField == null) { return; }
        var clientId = serverRpcParams.Receive.SenderClientId;
        GameObject pf = placeablePrefabs[prefabId];
        Debug.Log("Localpos2: " + localpos);
        Vector3 newPos = new Vector3(localpos.x, 0, localpos.z);
        prefab = Instantiate(pf, newPos, Quaternion.identity);
        
        //Vector3 prefabLocalPos = trackedImagePos - battleField.transform.position;
        prefab.name = pf.name;
        

        NetworkObject netObj = prefab.GetComponent<NetworkObject>();
        prefab.SetActive(true);
        
        // netObj.SpawnAsPlayerObject(clientId, true);
        
        spawnedPrefabs.Add(name, prefab);
        prefab.transform.parent = battleField.transform;
        prefab.transform.localPosition = newPos;
        Debug.Log("NewPosition: " + newPos);
        netObj.SpawnWithOwnership(clientId, false);
        //prefab.transform.localPosition = prefabLocalPos;
        //strDebug = "Prefab Local Pos: " + prefab.transform.localPosition.ToString();
        //Vector3 pos = trackedImagePos - battleField.transform.position;
    }

    private Vector3 worldToLocal(Vector3 worldpos, Transform battlefield) {
        return battleField.transform.InverseTransformPoint(worldpos);
    }

    private Vector3 localToWorld(Vector3 localpos, Transform battlefield)
    {
        return battleField.transform.TransformPoint(localpos);
    }

    [Rpc(SendTo.Everyone)]
    public void RestartSceneRpc()
    {
        SceneManager.LoadScene("Game");
    }

}


public class ARTrackedImagePlus
{
    public ARTrackedImage trackedImage { get; set; }
    public TrackingState trackingState { get; set; }
    public string name { get; set; }
    public Transform transform { get; set; }

}