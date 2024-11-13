using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ResetSessionManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called  once per frame
    void Update()
    {
        
    }

    public void ResetARSession()
    {
        ARSession arSession = GetComponent<ARSession>();
        arSession.Reset();
    }
}
