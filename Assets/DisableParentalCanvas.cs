using UnityEngine;

public class DisableParentalCanvas : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Canvas canvas;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DisableCanvas()
    {
        Debug.Log("Hi");    
        canvas.enabled = false;
    }
}
