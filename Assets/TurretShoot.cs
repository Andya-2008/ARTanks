using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;

public class TurretShoot : NetworkBehaviour
{
    Vector3 origLocalPos;
    [SerializeField] public List<Transform> Enemies = new List<Transform>();
    [SerializeField] float firingSpeed;
    Transform EnemyTarget;
    [SerializeField] GameObject PivotPoint;

    private NetworkVariable<Vector3> localpos = new NetworkVariable<Vector3>();
    bool firstEnemy = true;
    [SerializeField] TextMeshProUGUI DebugText;
    [SerializeField] ulong OwnerID;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log("Sent Turret ID");
            SendTurretIDRPC(NetworkManager.Singleton.LocalClientId);
        }
        foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Tank"))
        {
            Debug.Log("Tank: " + tank.name);
            Enemies.Add(tank.transform);
        }
        Enemies.Add(GameObject.FindGameObjectWithTag("MyTank").transform);
        foreach(Transform enemy in Enemies)
        {
            Debug.Log("EnemyID: " + enemy.GetComponent<TankPlayer>().TankID);
            Debug.Log("OwnerID: " + OwnerID);
            if(enemy.GetComponent<TankPlayer>().TankID == OwnerID)
            {
                Debug.Log("Removed owner tank: " + enemy.name);
                Enemies.Remove(enemy);
            }
        }
        if(IsHost)
        {
            localpos.Value = this.transform.localPosition;
        }
        else
        {
            this.gameObject.transform.parent = GameObject.Find("Battlefield1").transform;
            this.gameObject.transform.localPosition = localpos.Value;
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (firstEnemy)
        {
            Debug.Log("First Enemy");
            firstEnemy = false;
            EnemyTarget = FindNearestEnemy();
            Debug.Log("EnemyName: " + EnemyTarget.name);
        }
        EnemyTarget = FindNearestEnemy();
        if (EnemyTarget != null)
        {
            Debug.Log(EnemyTarget.name);
        }
        else
        {
            Debug.Log("There is no enemy target!");
        }
            PivotPoint.transform.eulerAngles = new Vector3(0, -180/Mathf.PI*Mathf.Atan2(EnemyTarget.transform.position.z - this.transform.position.z, EnemyTarget.transform.position.x - this.transform.position.x), 0);
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
    /*
    [Rpc(SendTo.Everyone)]
    public void ChangeNearestEnemyRPC(string playerName)
    {
        Debug.Log("Name:" + playerName.ToString() + " : Name[4]: " + playerName[4].ToString());
        List<Transform> tanks = new List<Transform> ();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Tank"))
        {
            tanks.Add(enemy.transform);
        }
        tanks.Add(GameObject.FindGameObjectWithTag("MyTank").transform);

        foreach (Transform tank in tanks)
        {
            if (tank.name[4] == playerName[4])
            {
                
                EnemyTarget = tank;
                break;
            }
        }
    }*/
    [Rpc(SendTo.Everyone)]
    public void SendTurretIDRPC(ulong ID)
    {
        OwnerID = ID;
    }
}
