using UnityEngine;

public class DisableParentalCanvas : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Canvas canvas;
    void Start()
    {
        if (PlayerPrefs.GetInt("supervision") == 1)
        {
            canvas.gameObject.SetActive(false);
        }
        DontDestroyOnLoad(canvas);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DisableCanvas()
    {
        Debug.Log("Hi");
        ChangeSupervisionPlayerPrefs();
        canvas.enabled = false;
    }
    public void ChangeSupervisionPlayerPrefs()
    {
        PlayerPrefs.SetInt("supervision", 1);
    }
}
