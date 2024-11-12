using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class RoomCodeText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomCodeText;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject rcc = GameObject.Find("RoomCodeCanvas");
        
        if (rcc.scene.buildIndex!=-1)
        {
            DontDestroyOnLoad(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RoomCode(string roomCode)
    {
        roomCodeText.gameObject.SetActive(true);
        roomCodeText.text = roomCode;
    }
}
