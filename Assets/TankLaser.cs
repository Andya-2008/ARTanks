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
        yield return new WaitForSeconds(3);
        laser.SetActive(true);
    }

    public void StopShoot()
    {
        laser.SetActive(false);

    }
}
