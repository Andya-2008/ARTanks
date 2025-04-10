using UnityEngine;

public class FireTurretHitZone : MonoBehaviour
{
    [SerializeField] Turret turret;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Tank")
        {
            turret.fireRange = true;
            turret.hitTanks.Add(other.gameObject.name);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Tank")
        {
            turret.fireRange = false;
            turret.hitTanks.Remove(other.gameObject.name);
        }

    }
}
