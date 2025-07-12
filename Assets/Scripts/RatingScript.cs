using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class RatingScript : NetworkBehaviour
{
    public int Rating = 0;
    public List<int> lstRatings = new List<int>();
    private int k = 20; // dev factor between 10-40 for elo calculation
    private List<TankMovement> listTanks = new List<TankMovement>();
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SelfRatingRPC(GetComponent<NetworkObject>().NetworkObjectId, PlayerPrefs.GetInt("Rating"));
        }
    }


    public void recordStats(bool win)
    {
        if (IsOwner)
        {
            int opponentRating = 0;

            int players = 0;
            Dictionary<ulong, int> ratings = GameObject.Find("GameManager").GetComponent<GameManager>().playerRatings;
            foreach (KeyValuePair<ulong, int> pair in ratings)
            {
                if (GetComponent<NetworkObject>().NetworkObjectId != pair.Key)
                {
                  
                    opponentRating += ratings[pair.Key];
                    players++;
                }
            }
            opponentRating = (int)(opponentRating / players);
            Debug.Log("OppRating:" + opponentRating);

            /* ELO formula 
             *  R' = R + K × (S − E) 
             *  k=20 (dev factor) [between 10-40]
             *  S = 0 for lose, .5 draw, 1 for win
             *  E = Expected Score
             *  E = 1 / (1 + 10^((opponent_rating - your_rating)/400))
             */

            int scoreDiff = opponentRating - Rating;
            Debug.Log("scoreDiff:" + scoreDiff);
            double exp = (double)scoreDiff / 400.0;
            Debug.Log("exp:" + exp);
            double den = 1 + 1.0 + Math.Pow(10, exp);
            Debug.Log("den:" + den);
            double expectedScore = 1.0 / den;
            Debug.Log("MyRating:" + Rating);
            Debug.Log("ExpectedScore:" + expectedScore);
            int newRating = 0;
            int score = 0;
            if (win)
            {
                score = 1;
            }
            newRating = Rating + (int)(k * (score - expectedScore));
            Debug.Log("NewRating:" + newRating);
            if (newRating < 400)
            {
                newRating = 400;
            }
            setRating("Rating", newRating);
        }
    }

    private void setRating(string statName, int newRating)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate
            {
                StatisticName = statName,
                Value = newRating
            }
        }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => UnityEngine.Debug.Log("Stat updated"),
            error => UnityEngine.Debug.LogError(error.GenerateErrorReport()));

        PlayerPrefs.SetInt("Rating", newRating);
        PlayerPrefs.Save();

    }
    [Rpc(SendTo.Everyone)]
    public void SelfRatingRPC(ulong networkId, int myRating)
    {
        Rating = myRating;
        GameObject.Find("GameManager").GetComponent<GameManager>().AddPlayerRating(networkId, myRating);
    }
}
