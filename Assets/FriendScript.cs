using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System;
using System.Collections.Generic;

public class FriendScript : MonoBehaviour
{

    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_Text txtError;
    [SerializeField] private GameObject friendItem;
    [SerializeField] private Transform contentContainer;
    

    public List<FriendInfo> friendlist = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetFriends();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public async void AddFriend()
    {

        if (inputUsername.text != "" && inputUsername.text != null)
        {
            var request = new AddFriendRequest();
            request.FriendUsername = inputUsername.text;
            PlayFabClientAPI.AddFriend(request, result =>
            {
                Debug.Log("Friend added successfully!");
                GetFriends();
            }, DisplayPlayFabError);
        }
        else
        {
            txtError.text = "You must enter a valid friend username.";
        }

    }

    

    void DisplayPlayFabError(PlayFabError error)
    {
        txtError.text = error.ErrorMessage;
    }

    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
        }, result => {
            friendlist = result.Friends;
            DisplayFriends(friendlist); // triggers your UI
        }, DisplayPlayFabError);
    }

    void DisplayFriends(List<FriendInfo> friends)
    {
        ClearFriendDisplay();
        foreach (FriendInfo fi in friends) {
            Debug.Log("friend:" + fi.Username);
            GameObject goFriend = Instantiate(friendItem, contentContainer);
            goFriend.GetComponentInChildren<TMP_Text>().text = fi.Username;
            goFriend.GetComponentInChildren<RemoveFriendScript>().PlayFabId = fi.FriendPlayFabId;
        }

    }

    void ClearFriendDisplay() {

        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

    }



}
