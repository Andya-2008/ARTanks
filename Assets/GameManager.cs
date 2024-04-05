using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject Player1UI;
    [SerializeField] GameObject Player2UI;
    [SerializeField] Canvas GameOverCanvas;
    // Start is called before the first frame update
    void Start()
    {
        Player1UI.SetActive(false);
        Player2UI.SetActive(false);
        GameOverCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GameOverInitiate(int loser)
    {
        GameOverCanvas.enabled = true;
        if (loser == 1)
        {
            Debug.Log("Player 2 won");
            Player2UI.SetActive(true);
            Player1UI.SetActive(false);
        }
        else if(loser == 2)
        {
            Debug.Log("Player 1 won");
            Player1UI.SetActive(true);
            Player2UI.SetActive(false);
        }
        else
        {
            Debug.LogError("GameOverInitiate called without clear winner.");
        }
    }
}
