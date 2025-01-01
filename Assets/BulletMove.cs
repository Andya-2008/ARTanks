using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class BulletMove : MonoBehaviour
{
    [SerializeField] public float bulletSpeed;
    public bool homing;
    public bool phantom;
    [SerializeField] public float homingTurnSpeed = .1f;
    [SerializeField] public GameObject myTank;
    [SerializeField] public List<Transform> Enemies = new List<Transform>();
    Transform EnemyTarget;

    bool firstEnemy = true;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("MyTank") != null)
        {
            foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Tank"))
            {
                Enemies.Add(tank.transform);
            }
            Enemies.Add(GameObject.FindGameObjectWithTag("MyTank").transform);
            Enemies.Remove(myTank.transform);
        }
        if(phantom)
        {
            this.gameObject.layer = 15;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Enemies.Count != 0)
        {
            if (firstEnemy)
            {
                firstEnemy = false;
                EnemyTarget = FindNearestEnemy();
            }
            EnemyTarget = FindNearestEnemy();
            if (EnemyTarget != null)
            {
                if (homing)
                {
                    HomingTurn();
                }
            }
            else
            {
                Debug.Log("There is no enemy target!");
            }
        }
        MoveBullet();
        
    }

    public void MoveBullet()
    {
        this.transform.position += this.transform.forward * bulletSpeed * Time.deltaTime * 100;
    }
    
    public void HomingTurn()
    {
        //Vector3 bulletTankAngle = new Vector3(0, 90 - 180 / Mathf.PI * Mathf.Atan2(EnemyTarget.transform.position.z - this.transform.position.z, EnemyTarget.transform.position.x - this.transform.position.x), 0);
        //transform.eulerAngles = bulletTankAngle;
        Vector3 bulletTankVector = new Vector3(EnemyTarget.transform.position.x - this.transform.position.x, 0, EnemyTarget.transform.position.z - this.transform.position.z);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, bulletTankVector, homingTurnSpeed * Time.deltaTime * 100, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }


    public Transform FindNearestEnemy()
    {
        float minEnemyDistance = 100000000f;
        Transform nearestEnemy = Enemies[0];
        foreach (Transform enemy in Enemies)
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