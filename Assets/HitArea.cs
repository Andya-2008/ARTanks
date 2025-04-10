using Complete;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    bool strikeZone = true;
    float startTime;
    public float damage;
    public List<GameObject> hitTanks = new List<GameObject>();
    bool hasDealtDamage;
    // Starts called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime > 2 && !hasDealtDamage)
        {
            hasDealtDamage = true;
            if (strikeZone)
            {
                foreach (GameObject tank in hitTanks)
                {
                    tank.GetComponent<TankHealth>().TakeDamage(damage);
                }
            }
        }
        if (Time.time - startTime > 2.5)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit: " + other.name);
        if (NetworkManager.Singleton.IsServer)
        {
            if (other.gameObject.tag == "MyTank" || other.gameObject.tag == "Tank")
            {
                if(hitTanks.Contains(other.gameObject))
                {
                    hitTanks.Remove(other.gameObject);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.name);
        if (NetworkManager.Singleton.IsServer)
        {
            if (other.gameObject.tag == "MyTank" || other.gameObject.tag == "Tank")
            {
                if (!hitTanks.Contains(other.gameObject))
                {
                    hitTanks.Add(other.gameObject);
                }
            }
        }

    }
}
