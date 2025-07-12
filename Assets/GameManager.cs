using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] GameObject Player1UI;
    [SerializeField] GameObject Player2UI;
    [SerializeField] GameObject Player3UI;
    [SerializeField] GameObject Player4UI;
    [SerializeField] Canvas GameOverCanvas;
    [SerializeField] public List<TankMovement> tanks = new List<TankMovement>();
    [SerializeField] public List<TankMovement> aliveTanks = new List<TankMovement>();
    [SerializeField] public Dictionary<ulong, int> playerRatings = new Dictionary<ulong, int>();


    [SerializeField] GameObject ARCamera;
    [SerializeField] GameObject MainCamera;
    [SerializeField] GameObject WinPlane;
    [SerializeField] bool PlayOnEditor = false;

    public int battlefieldReadyNum;

    [SerializeField] Canvas startCanvas;

    [SerializeField] Canvas deathCanvas;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_STANDALONE_WIN
if (PlayOnEditor){
Debug.Log("Unity Standalone Win");
  ARCamera.SetActive(false);
        MainCamera.SetActive(true);
        WinPlane.SetActive(true);
        Screen.SetResolution(1920, 1080, false);
        //MainCamera.GetComponent<Camera>().aspect = 1;
        }
#endif


#if UNITY_EDITOR_WIN
        if (PlayOnEditor)
        {
            ARCamera.SetActive(false);
            MainCamera.SetActive(true);
            WinPlane.SetActive(true);
            Screen.SetResolution(1920, 1080, false);
            //MainCamera.GetComponent<Camera>().aspect = 1;
        }

#endif


#if UNITY_EDITOR_OSX
        if (PlayOnEditor)
        {
            Debug.Log("Unity Editor Win");
            ARCamera.SetActive(false);
            MainCamera.SetActive(true); 
            WinPlane.SetActive(true);
            Screen.SetResolution(1920, 1080, false);
            //MainCamera.GetComponent<Camera>().aspect = 1;
        }

#endif
        startCanvas.enabled = true;
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

    public void GameOverInitiate(TankMovement aliveTank)
    {
        if(aliveTank.GetComponent<NetworkObject>().IsOwner)
		{
            aliveTank.GetComponent<RatingScript>().recordStats(true);
        }
        int winner = aliveTank.m_PlayerNumber;
        deathCanvas.enabled = false;
        GameOverCanvas.enabled = true;
        if (winner == 1)
        {
            Debug.Log("Player 1 won");
            Player1UI.SetActive(true);
            Player2UI.SetActive(false);
            Player3UI.SetActive(false);
            Player4UI.SetActive(false);
        }
        else if (winner == 2)
        {
            Debug.Log("Player 2 won");
            Player1UI.SetActive(false);
            Player2UI.SetActive(true);
            Player3UI.SetActive(false);
            Player4UI.SetActive(false);
        }
        else if (winner == 3)
        {
            Debug.Log("Player 3 won");
            Player1UI.SetActive(false);
            Player2UI.SetActive(false);
            Player3UI.SetActive(true);
            Player4UI.SetActive(false);
        }
        else if (winner == 4)
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

    public void PlayerDead(GameObject tank)
    {
        Debug.Log("2");

        var tankMovement = tank.GetComponent<TankMovement>();
        if (tankMovement != null)
        {
            Debug.Log("TankMovement component found.");
            if (aliveTanks.Contains(tankMovement))
            {
                aliveTanks.Remove(tankMovement);
                Debug.Log("3");
                Debug.Log("Tank successfully removed from aliveTanks list.");

                if (aliveTanks.Count == 1)
                {
                    Debug.Log("4");
                    Debug.Log("Winner: " + aliveTanks[0].m_PlayerNumber.ToString());
                    GameOverInitiate(aliveTanks[0]);
                }
                else
                {
                    if(tank.GetComponent<NetworkObject>().IsOwner)
                    deathCanvas.enabled = true;
                }
            }
            else
            {
                Debug.LogError("TankMovement component found, but tank not in aliveTanks list.");
            }
        }
        else
        {
            Debug.LogError("TankMovement component missing on the tank GameObject.");
        }
    }
    public void AddTank(TankMovement Tank)
    {
        tanks.Add(Tank);
        aliveTanks.Add(Tank);
    }

    public void AddPlayerRating(ulong networkId, int myRating) {
        playerRatings.Add(networkId, myRating);
    }

    public void SpectateAfterDeath()
    {
        deathCanvas.enabled = false;
    }
}
