using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class RepairBot : MonoBehaviour
{
    public GameObject targetTank;
    [SerializeField] float moveSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (GetComponent<NetworkObject>().IsOwner)
        {
            FollowTank();
        }
    }

    public void FollowTank()
    {
        transform.LookAt(targetTank.transform);
        transform.Translate(Vector3.Normalize(targetTank.transform.position - this.transform.position) * moveSpeed);
    }
}
