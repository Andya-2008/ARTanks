using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurretShoot : NetworkBehaviour
{
    Vector3 origLocalPos;
    [SerializeField] public List<Transform> Enemies = new List<Transform>();
    [SerializeField] float firingSpeed;
    Transform EnemyTarget;
    [SerializeField] GameObject PivotPoint;

    private NetworkVariable<Vector3> localpos = new NetworkVariable<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public override void OnNetworkSpawn()
    {
        if (NetworkObject.IsOwner)
        {
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Tank"))
            {
                Enemies.Add(enemy.transform);
            }
        }
        if(IsHost)
        {
            localpos.Value = this.transform.localPosition;
        }
        else
        {
            
            Debug.Log("1");
            this.gameObject.transform.parent = GameObject.Find("Battlefield1").transform;
            //Debug.Log("2");
            //origLocalPos = GameObject.Find("XR Origin").GetComponent<ImageTracking>().localpos;
            Debug.Log("3");
            this.gameObject.transform.localPosition = localpos.Value;
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (NetworkObject.IsOwner)
        {
            EnemyTarget = FindNearestEnemy();
            Debug.Log(EnemyTarget.name);
            if (FindNearestEnemy() != EnemyTarget)
            {
                ChangeNearestEnemyRPC(EnemyTarget.position);
            }
        }
        PivotPoint.transform.rotation = new Quaternion(Mathf.Atan2(EnemyTarget.transform.position.z - this.transform.position.z, EnemyTarget.transform.position.x - this.transform.position.x), PivotPoint.transform.rotation.y, PivotPoint.transform.rotation.z, 0);
    }

    public Transform FindNearestEnemy()
    {
        float minEnemyDistance = 100000000f;
        Transform nearestEnemy = Enemies[0];
        foreach (Transform enemy in Enemies)
        {
            float enemyDistance = Mathf.Pow(enemy.position.x, 2) + Mathf.Pow(enemy.position.z, 2);
            if (enemyDistance <= minEnemyDistance)
            {
                minEnemyDistance = enemyDistance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeNearestEnemyRPC(Vector3 newTargetPos)
    {
        Transform newTarget = this.transform;
        newTarget.position = newTargetPos;
        EnemyTarget = newTarget;
    }

}
