using UnityEngine;

public class DropBulletTest : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletSpawnPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropBullet();
        }
    }

    public void DropBullet()
    {
        Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity);
    }
}
