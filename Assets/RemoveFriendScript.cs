using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class RemoveFriendScript : MonoBehaviour
{

    [SerializeField] private TMP_Text txtError;
    public string PlayFabId;
    public async void RemoveFriend()
    {
        PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
        {

            FriendPlayFabId = PlayFabId
        }, result =>
        {
            GameObject.Find("Scroll View").GetComponent<FriendScript>().GetFriends();
        }, DisplayPlayFabError);
        Debug.Log("Remove Friend:" + PlayFabId);
        
    }

    void DisplayPlayFabError(PlayFabError error)
    {
        txtError.text = error.ErrorMessage;
    }
}
