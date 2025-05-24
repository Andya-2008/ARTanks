using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using Unity.Netcode;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : NetworkBehaviour

{
    [SerializeField] TextMeshProUGUI DebugText;
    [SerializeField] private Dictionary<string, GameObject> placePrefabs = new Dictionary<string, GameObject>();
    [SerializeField] private List<GameObject> placeablePrefabs = new List<GameObject>();
    [SerializeField] private XRReferenceImageLibrary refLib;

    private ARAnchorManager m_AnchorManager;
    private ARAnchor anchor;
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    //private List<ARTrackedImage> newImages = new List<ARTrackedImage>();
    private ARTrackedImageManager trackedImageManager;
    private bool hasSpawnedTank = false;

    float startTime;
    bool started;
    GameObject prefab;
    private string strDebug = "";
    private string tiDebug = "";
    public bool doneWithPowerups;
    //public NetworkVariable<Vector3> localpos = new NetworkVariable<Vector3>();
    //public Vector3 localpos;

    ARTrackedImage trackImage;
    public GameObject battleField;
    public bool canSpawnTank;
    public bool tankApproved;
    private void Awake()
    {
        m_AnchorManager = this.GetComponent<ARAnchorManager>();
        trackedImageManager = FindFirstObjectByType<ARTrackedImageManager>();
        trackedImageManager.referenceLibrary = refLib;
        trackedImageManager.enabled = true;
    }
    private void Update()
    {
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
            hasSpawnedTank = false;
            ARTrackedImagePlus artip = new ARTrackedImagePlus();
            artip.name = "Firestorm";
            GameObject go = new GameObject();
            artip.transform = go.transform;
            artip.transform.position = new Vector3(0.2f, 0, 0);
            artip.transform.rotation = Quaternion.identity;
            Debug.Log("1");
            StartCoroutine(CreateObject(artip));
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Y");
            hasSpawnedTank = false;
            ARTrackedImagePlus artip = new ARTrackedImagePlus();
            artip.name = "Cheetah";
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
            else if (!hasSpawnedTank && trackedImage.trackingState == TrackingState.Tracking && tankApproved)
            {
                ARTrackedImagePlus artip = new ARTrackedImagePlus();
                artip.trackedImage = trackedImage;
                StartCoroutine(CreateObject(artip));
            }
        }
    }
    
    public void DebugTxt(string txt)
    {
        string oldDebug;
        if (DebugText != null)
        {
            if (DebugText.text.Length > 200)
            {
                oldDebug = DebugText.text.Substring(0, 200);
            }
            else
            {
                oldDebug = DebugText.text;
            }
            DebugText.text = txt + "\n" + oldDebug;
        }
    }

    private void updateObject(ARTrackedImage trackedImage, string updatedImg)
    {
        if (updatedImg == "Battlefield")
        {
            if (GameObject.FindGameObjectWithTag("Battlefield"))
            {
                battleField = GameObject.FindGameObjectWithTag("Battlefield");
                if (anchor == null)
                {
                    battleField.transform.position = trackedImage.transform.position;
                    battleField.transform.rotation = trackedImage.transform.rotation;
                }
                else
                {
                    Vector3 localpos = worldToLocal(trackedImage.transform.position, anchor.transform);
                    battleField.transform.position = localpos;

                    Quaternion localRotation = Quaternion.Inverse(anchor.transform.rotation) * trackedImage.transform.rotation;

                    battleField.transform.rotation = localRotation;
                }
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
        if (pf.tag == "Battlefield"/* && doneWithPowerups*/)
        {
            if (!GameObject.FindGameObjectWithTag("Battlefield"))
            {
                Quaternion worldRotation = new Quaternion(trackedImage.transform.localRotation.x, trackedImage.transform.localRotation.y, trackedImage.transform.localRotation.z, trackedImage.transform.localRotation.w);
                //worldRotation *= Quaternion.Euler(90, 0, 0);
                
                Pose pose = new Pose(trackedImage.transform.position, trackedImage.transform.rotation);
                Task<ARAnchor> myTask = SetAnchor(pose); // async Task method
                yield return new WaitUntil(() => myTask.IsCompleted);
                anchor = myTask.Result;
                if (anchor != null)
                {
                    prefab = Instantiate(pf, anchor.transform);
                }
                else
                {
                    prefab = Instantiate(pf, trackedImage.transform.position, worldRotation);
                }
                battleField = prefab;

                prefab.name = pf.name;
                spawnedPrefabs.Add(name, prefab);
                prefab.SetActive(true);

                prefab.name = "Battlefield1";
                SendBattleFieldStatRpc();
            }
        }
        if (pf.tag == "Tank" || pf.tag == "MyTank")
        {

            if (!hasSpawnedTank && tankApproved) {
                hasSpawnedTank = true;
                Vector3 localpos = worldToLocal(trackedImage.transform.position, battleField.transform);
                SpawnPlayerServerRpc(name, localpos, prefabId);
            }
        }
        if(pf.tag == "Powerup" || pf.tag == "UIPowerup")
        {
            if (!spawnedPrefabs.ContainsKey(name))
            {
                Debug.Log("0");
                GameObject.Find("PowerupManager").GetComponent<PowerupManager>().AddPowerup(pf.name);
                spawnedPrefabs.Add(name, prefab);
            }
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

    private async Task<ARAnchor> SetAnchor(Pose pose)
    {
        try
        {

            var result = await m_AnchorManager.TryAddAnchorAsync(pose);

            if (result.status.IsSuccess())
            {
                ARAnchor newAnchor = result.value;
                return newAnchor;
            }
            else
            {
                return null;
            }

        }
        catch (Exception ex)
        {
            //DebugTxt("SetAnchor:" + ex.Message);
            return null;
        }
    }

    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(string name, Vector3 localpos, int prefabId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) { return; }
        if (battleField == null) { return; }
        if(!tankApproved) { return; }
        var clientId = serverRpcParams.Receive.SenderClientId;
        GameObject pf = placeablePrefabs[prefabId];
        Vector3 newPos = new Vector3(localpos.x, 0, localpos.z);
        prefab = Instantiate(pf, newPos, Quaternion.identity);
        prefab.name = pf.name;
        

        NetworkObject netObj = prefab.GetComponent<NetworkObject>();
        prefab.SetActive(true);
        
        spawnedPrefabs.Add(name, prefab);
        prefab.transform.parent = battleField.transform;
        prefab.transform.localPosition = newPos;
        netObj.SpawnWithOwnership(clientId, false);

    }
    [Rpc(SendTo.Server)]
    public void SendBattleFieldStatRpc()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().battlefieldReadyNum += 1;
        Debug.Log("Added readynum. Connectedclients.count: " + NetworkManager.Singleton.ConnectedClientsList.Count);

        Debug.Log("Approving tanks: " + GameObject.Find("GameManager").GetComponent<GameManager>().battlefieldReadyNum + " : " + NetworkManager.Singleton.ConnectedClientsList.Count);
        if (GameObject.Find("GameManager").GetComponent<GameManager>().battlefieldReadyNum == NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            TankApprovalRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    public void TankApprovalRpc()
    {
        Debug.Log("Tank approved");
        tankApproved = true;
    }
    private Vector3 worldToLocal(Vector3 worldpos, Transform battlefield) {
        return battleField.transform.InverseTransformPoint(worldpos);
    }

    private Vector3 localToWorld(Vector3 localpos, Transform battlefield)
    {
        return battleField.transform.TransformPoint(localpos);
    }

    public void RestartScene()
    {
        RestartSceneRpc();
    }

    public void ExitScene() {
        ExitSceneRpc();
    }
    [Rpc(SendTo.Everyone)]
    public void ExitSceneRpc()
    {
        ARSession arSession = GameObject.Find("AR Session").GetComponent<ARSession>();
        arSession.Reset();

        //GameObject.Find("AR Session").GetComponent<ResetSessionManager>().ResetARSession();
        /*GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPlayer>().OnDespawnTank();
        foreach(GameObject tank in GameObject.FindGameObjectsWithTag("Tank"))
        {
            tank.GetComponent<TankPlayer>().OnDespawnTank();
        }*/
        hasSpawnedTank = false;
        spawnedPrefabs.Clear();
        Destroy(GameObject.FindGameObjectWithTag("Battlefield"));
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    
    
    [Rpc(SendTo.Everyone)]
    public void RestartSceneRpc()
    {

        ARSession arSession = GameObject.Find("AR Session").GetComponent<ARSession>();
        arSession.Reset();

        //GameObject.Find("AR Session").GetComponent<ResetSessionManager>().ResetARSession();
        /*GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankPlayer>().OnDespawnTank();
        foreach(GameObject tank in GameObject.FindGameObjectsWithTag("Tank"))
        {
            tank.GetComponent<TankPlayer>().OnDespawnTank();
        }*/
        hasSpawnedTank = false;
        spawnedPrefabs.Clear();
        Destroy(GameObject.FindGameObjectWithTag("Battlefield"));
        NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    public void DoneWithPowerups()
    {
        doneWithPowerups = true;
        GameObject.Find("PowerupManager").GetComponent<PowerupManager>().ClosePowerupCanvas();
    }
}

public class ARTrackedImagePlus
{
    public ARTrackedImage trackedImage { get; set; }
    public TrackingState trackingState { get; set; }
    public string name { get; set; }
    public Transform transform { get; set; }

}