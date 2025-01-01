using UnityEngine;
using UnityEngine.Playables;

public class OilSpillManager : MonoBehaviour
{
    PlayableDirector playableDir;
    [SerializeField] PlayableDirector oilStopper;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playableDir = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartOilSpill()
    {
        Debug.Log("3");
        playableDir.Play();
    }
    public void StopOilSpill()
    {
        Debug.Log("4");
        oilStopper.Play();
    }
}
