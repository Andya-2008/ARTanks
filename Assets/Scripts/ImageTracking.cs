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

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : NetworkBehaviour

{
    [SerializeField] TextMeshProUGUI DebugText;
    [SerializeField] private Dictionary<string, GameObject> placePrefabs = new Dictionary<string, GameObject>();
    [SerializeField] private List<GameObject> placeablePrefabs = new List<GameObject>();

    
    
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    //private List<ARTrackedImage> newImages = new List<ARTrackedImage>();
    private ARTrackedImageManager trackedImageManager;

    float startTime;
    bool started;
    GameObject prefab;
    private string strDebug = "";
    private string tiDebug = "";

    ARTrackedImage trackImage;
    public GameObject battleField;
    private void Awake()
    {
        trackedImageManager = FindFirstObjectByType<ARTrackedImageManager>();

    }
    private void Update()
    {


        /*
        if (newImages.Count > 0) {
            foreach (ARTrackedImage img in newImages) {
                if (img.transform.position != new Vector3(0f, 0f, 0f) && img.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking) {
                    CreateObject(img);
                    newImages.Remove(img);
                }
            
            }
        }
        */

        
        DebugTxt(strDebug);
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
            //Update
            //Debug.Log("referenceImageName:" + trackedImage.referenceImage.name);
            if (spawnedPrefabs.ContainsKey(trackedImage.referenceImage.name)) { return; }
            //newImages.Add(trackedImage);
            StartCoroutine(CreateObject(trackedImage));
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.referenceImage.name.Contains("Battlefield"))
            {
                updateObject(trackedImage);
            }
        }
    }
    
    public void DebugTxt(string txt)
    {
        DebugText.text = txt;
    }

    private void updateObject(ARTrackedImage trackedImage) {
        if (battleField != null) { 
            battleField.transform.position = trackedImage.transform.position;
            battleField.transform.rotation = trackedImage.transform.rotation;
        }
    }
    IEnumerator CreateObject(ARTrackedImage trackedImage) {

        while (trackedImage.trackingState != TrackingState.Tracking) {
            yield return null;
        }


        string name = trackedImage.referenceImage.name;

        
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
        Debug.Log(name + ":" + trackedImage.transform.position);
        if (pf.tag != "Battlefield")
        {
            SpawnPlayerServerRpc(name, trackedImage.transform.position, prefabId);
 
        }
        if (pf.tag == "Battlefield")
        {
            
            Quaternion worldRotation = new Quaternion(trackedImage.transform.localRotation.x, trackedImage.transform.localRotation.y, trackedImage.transform.localRotation.z, trackedImage.transform.localRotation.w);
            //worldRotation *= Quaternion.Euler(90, 0, 0);
            Debug.Log("Instantiate:" + name);
            prefab = Instantiate(pf, trackedImage.transform.position, worldRotation);
            battleField = prefab;

            prefab.name = pf.name;
            Debug.Log("Added to SpawnedPrefabs:" + name);
            spawnedPrefabs.Add(name, prefab);
            prefab.SetActive(true);
        }

        yield return null;
        //prefab.transform.position = trackedImage.transform.position;
        
    }

    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(string name, Vector3 trackedImagePos, int prefabId, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("SpawnPlayerServerRpc");
        var clientId = serverRpcParams.Receive.SenderClientId;
        GameObject pf = placeablePrefabs[prefabId];
        Debug.Log("spsr Instantiate:" + name);
        prefab = Instantiate(pf, trackedImagePos, Quaternion.identity, battleField.transform);
        prefab.name = pf.name;
        prefab.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);


        NetworkObject netObj = prefab.GetComponent<NetworkObject>();
        prefab.SetActive(true);
        // netObj.SpawnWithOwnership(clientId, true);
        // netObj.SpawnAsPlayerObject(clientId, true);

        Debug.Log("Added to SpawnedPrefabs:" + name);
        spawnedPrefabs.Add(name, prefab);

        Vector3 pos = trackedImagePos - battleField.transform.position;
        Debug.Log(name + ":" + trackedImagePos.ToString() + "\n" + "pos:" + pos.ToString() + "\nTrackedImage:" + trackedImagePos.ToString() + "\nBattlefield:" + battleField.transform.position.ToString());

    }
}
