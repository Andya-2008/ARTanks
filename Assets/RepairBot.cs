using Complete;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class RepairBot : MonoBehaviour
{
    public GameObject targetTank;
    [SerializeField] float moveSpeed;
    [SerializeField] float repairHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            targetTank = GameObject.FindGameObjectWithTag("MyTank");
        }
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
        if(other.gameObject.name == targetTank.name)
        {
            targetTank.GetComponent<TankHealth>().AddHealth(repairHealth);
            Destroy(this.gameObject);
        }
    }
}