using System.Collections;
using UnityEngine;

public class TankLaser : MonoBehaviour
{
    [SerializeField] GameObject laser;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Shoot()
    {
        GameObject.Find("SFXManager").GetComponent<SFXManager>().PlaySFXRPC("AnnihilaserCharge");
        yield return new WaitForSeconds(3);

        GameObject.Find("SFXManager").GetComponent<SFXManager>().PlaySFXRPC("Annihilaser");
        laser.SetActive(true);
    }

    public void StopShoot()
    {
        laser.SetActive(false);

    }
}
