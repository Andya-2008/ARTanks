using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Unity.Netcode;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DebugText;
    [SerializeField] private Dictionary<string, GameObject> placePrefabs = new Dictionary<string, GameObject>();
    [SerializeField] private GameObject[] placeablePrefabs;
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private ARTrackedImageManager trackedImageManager;

    float startTime;
    bool started;
    GameObject prefab;
    private string strDebug = "";

    ARTrackedImage trackImage;
    private GameObject battleField;
    private void Awake()
    {
        trackedImageManager = FindFirstObjectByType<ARTrackedImageManager>();
        /*
        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
        */
    }
    private void Update()
    {
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
            UpdateImage(trackedImage);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        trackImage = trackedImage;
        string name = trackedImage.referenceImage.name;
        if (!spawnedPrefabs.ContainsKey(name))
        {
            Vector3 position = trackedImage.transform.position;
            
            GameObject pf = new GameObject();
            foreach (GameObject go in placeablePrefabs)
            {
                if (go.name == name)
                {
                    pf = go;
                }
            }
            if (pf.tag != "Battlefield")
            {

                Vector3 pos = trackedImage.transform.position - battleField.transform.position;
                prefab = Instantiate(pf, pos, Quaternion.identity, battleField.transform);
                prefab.GetComponent<NetworkObject>().Spawn();
                strDebug += "\n" + name + ":" + position.ToString() + "\n" + "pos:" + pos.ToString() + "\nTrackedImage:" + trackedImage.transform.position.ToString() + "\nBattlefield: " + battleField.transform.position.ToString();

            }
            if (pf.tag == "Battlefield")
            {
                
                prefab = Instantiate(pf, trackedImage.transform.position, Quaternion.identity);
                battleField = prefab;
                strDebug += "\n" + name + ":" + position.ToString() + "\nTrackedImage:" + trackedImage.transform.position.ToString() + "\nBattlefield: " + battleField.transform.position.ToString();

            }

            prefab.name = pf.name;
            spawnedPrefabs.Add(pf.name, prefab);
            prefab.SetActive(true);
            //prefab.transform.position = trackedImage.transform.position;
            DebugTxt(strDebug);
            
        }
        
    }
    public void DebugTxt(string txt)
    {
        DebugText.text += txt;
    }
}