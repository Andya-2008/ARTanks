using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject Player1UI;
    [SerializeField] GameObject Player2UI;
    [SerializeField] GameObject Player3UI;
    [SerializeField] GameObject Player4UI;
    [SerializeField] Canvas GameOverCanvas;
    [SerializeField] public List<TankPlayer> tanks = new List<TankPlayer>();
    [SerializeField] public List<TankPlayer> aliveTanks = new List<TankPlayer>();
    // Start is called before the first frame update
    void Start()
    {
        Player1UI.SetActive(false);
        Player2UI.SetActive(false);
        Player3UI.SetActive(false);
        Player4UI.SetActive(false);
        GameOverCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GameOverInitiate(int winner)
    {
        GameOverCanvas.enabled = true;
        if (winner == 1)
        {
            Debug.Log("Player 1 won");
            Player1UI.SetActive(true);
            Player2UI.SetActive(false);
            Player3UI.SetActive(false);
            Player4UI.SetActive(false);
        }
        else if(winner == 2)
        {
            Debug.Log("Player 2 won");
            Player1UI.SetActive(false);
            Player2UI.SetActive(true);
            Player3UI.SetActive(false);
            Player4UI.SetActive(false);
        }
        else if(winner == 3)
        {
            Debug.Log("Player 3 won");
            Player1UI.SetActive(false);
            Player2UI.SetActive(false);
            Player3UI.SetActive(true);
            Player4UI.SetActive(false);
        }
        else if(winner == 4)
        {
            Debug.Log("Player 4 won");
            Player1UI.SetActive(false);
            Player2UI.SetActive(false);
            Player3UI.SetActive(false);
            Player4UI.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverInitiate called without clear winner.");
        }
    }

    public void PlayerDead(int playerDead)
    {
        foreach(TankPlayer tank in tanks)
        {
            if(tank.GetComponent<TankMovement>().m_PlayerNumber == playerDead)
            {
                aliveTanks.Remove(tank);
                Debug.Log("Tank killed: " + tank.gameObject.name);
            }
        }
        if (aliveTanks.Count == 1)
        {
            //Initiaes GameOver with the last remaining tank as winner
            GameOverInitiate(aliveTanks[0].GetComponent<TankMovement>().m_PlayerNumber);
        }
    }
    public void AddTank(TankPlayer Tank)
    {
        tanks.Add(Tank);
        aliveTanks.Add(Tank);
    }
}
