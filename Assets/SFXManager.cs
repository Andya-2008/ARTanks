using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Netcode;

public class SFXManager : MonoBehaviour
{
    [SerializeField] List<AudioSource> SFX = new List<AudioSource>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Rpc(SendTo.Everyone)]
    public void PlaySFXRPC(string SFXName)
    {
        foreach(AudioSource SF in SFX)
        {
            if(SF.name == SFXName)
            {
                SF.Play();
            }
        }
    }
}
