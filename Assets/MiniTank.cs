using Complete;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class MiniTank : MonoBehaviour
{
    public GameObject targetTank;
    [SerializeField] float moveSpeed;
    [SerializeField] float damage;
    List<Transform> enemyTanks = new List<Transform>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            if (!targetTank)
                return;
            FollowTank();
        }
    }

    public void FollowTank()
    {
        transform.LookAt(targetTank.transform);
        transform.Translate((targetTank.transform.position - this.transform.position) * moveSpeed);
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name.ToString());
        if (GetComponent<NetworkObject>().IsOwner)
        {
            foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Tank"))
            {
                Debug.Log("Added tank: " + tank.name);
                enemyTanks.Add(tank.transform);
            }
            targetTank = FindEnemyTank().gameObject;
        }
        if (other.gameObject.name == targetTank.name)
        {
            targetTank.GetComponent<TankHealth>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
        if (other.gameObject.tag == "MyBullet")
        {
            Destroy(this.gameObject);
        }
    }
    public Transform FindEnemyTank()
    {
        float minEnemyDistance = 100000000f;
        Transform nearestEnemy = enemyTanks[0];
        foreach (Transform enemy in enemyTanks)
        {
            float enemyDistance = Mathf.Pow(this.transform.position.x - enemy.position.x, 2) + Mathf.Pow(this.transform.position.z - enemy.position.z, 2);
            if (enemyDistance <= minEnemyDistance)
            {
                minEnemyDistance = enemyDistance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }
}