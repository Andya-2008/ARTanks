using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using TMPro;
[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DebugText;
    [SerializeField] private Dictionary<string, GameObject> placePrefabs = new Dictionary<string, GameObject>();
    [SerializeField] private GameObject[] placeablePrefabs;
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        DebugTxt("S");
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
        /*
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }
        */
        /*
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            spawnedPrefabs[trackedImage.name].SetActive(false);
        }
        */
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        DebugTxt("1");
        string name = trackedImage.referenceImage.name;
        if (!spawnedPrefabs.ContainsKey(name))
        {
            DebugTxt("2");
            Vector3 position = trackedImage.transform.position;
            GameObject pf = new GameObject();
            DebugTxt("3");
            foreach (GameObject go in placeablePrefabs)
            {
                if (go.name == name)
                {
                    DebugTxt("4");
                    pf = go;
                }
            }
                DebugTxt("5");
                GameObject prefab = Instantiate(pf.gameObject, Vector3.zero, Quaternion.identity);
                DebugTxt("6");
                prefab.name = pf.name;
                DebugTxt("7");
                spawnedPrefabs.Add(pf.name, prefab);
                prefab.transform.position = position;
                DebugTxt("8");
                prefab.SetActive(true);
                DebugTxt("9");
            
            /*
            foreach(GameObject go in spawnedPrefabs.Values)
            {
                if (go.name != name)
                {
                    go.SetActive(false);
                }
            }
            */
        }
    }
    public void DebugTxt(string txt)
    {
        DebugText.text += txt;
    }
}