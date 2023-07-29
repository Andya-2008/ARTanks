using System.Collections.Generic;
using UnityEngine;
using Complete;
public class ShootButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TankShootButton()
    {
        GameObject.FindGameObjectWithTag("Tank").GetComponent<TankShooting>().Fire();
    }
}
