using Complete;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class TankPowerups : NetworkBehaviour
{
    public bool addHealth;
    float timeSinceLastAddHealth;
    public float healthAdditionTime = 1.5f;
    TankMovement tankMovement;
    TankHealth tankHealth;
    TankShooting tankShooting;

    [SerializeField] Transform tacticalSpawnPos;

    GameObject battleField;
    [SerializeField] GameObject omniWall;
    [SerializeField] GameObject alloWall;
    // Start is called before the first frame update
    void Start()
    {
        tankMovement = this.GetComponent<TankMovement>();
        tankShooting = this.GetComponent<TankShooting>();
        tankHealth = this.GetComponent<TankHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - timeSinceLastAddHealth >= healthAdditionTime && addHealth)
        {
            timeSinceLastAddHealth = Time.time;
            PassiveAddHealth();
        }
        battleField = GameObject.FindGameObjectWithTag("Battlefield");
    }


    [Rpc(SendTo.Everyone)]
    public void ActivateOrDeactivateTankPowerupRPC(string poweruptype, bool activate)
    {
        
        if (poweruptype.Contains("BulletReload"))
        {
            if (activate)
            {
                tankShooting.m_ReloadTime -= .75f;
            }
            else
            {
                tankShooting.m_ReloadTime += .75f;
            }
        }
        else if (poweruptype.Contains("BulletSpeed"))
        {
            if (activate)
            {
                tankShooting.m_BulletSpeed += .005f;
            }
            else
            {
                tankShooting.m_BulletSpeed -= .005f;
            }
        }
        else if (poweruptype.Contains("BulletPower"))
        {
            if (activate)
            {
                tankShooting.m_BulletPower += 50;
            }
            else
            {
                tankShooting.m_BulletPower -= 50;
            }
        }
        else if (poweruptype.Contains("TankSpeed"))
        {
            if(activate)
            {
                tankMovement.m_Speed += .3f;
            }
            else
            {
                tankMovement.m_Speed -= .3f;
            }
        }
        else if (poweruptype.Contains("Health"))
        {
            if(activate)
            {
                addHealth = true;
            }
            else
            {
                addHealth = false;
            }
        }
        else if (poweruptype.Contains("Overdrive"))
        {
            if (activate)
            {
                if (tankHealth.m_CurrentHealth - tankHealth.m_StartingHealth / 5 > 0)
                {
                    tankHealth.m_CurrentHealth -= tankHealth.m_StartingHealth / 5;
                }
                else if(tankHealth.m_CurrentHealth >= 5)
                {
                    tankHealth.m_CurrentHealth = 5;
                }
                tankMovement.m_Speed *= 1.4f;
                tankShooting.m_BulletSpeed *= 1.3f;
                tankShooting.m_BulletPower *= 1.3f;
                tankShooting.m_ReloadTime /= 1.5f;
            }
            else
            {
                tankMovement.m_Speed /= 1.4f;
                tankShooting.m_BulletSpeed /= 1.3f;
                tankShooting.m_BulletPower /= 1.3f;
                tankShooting.m_ReloadTime *= 1.5f;
            }
        }
        else if (poweruptype.Contains("Homing"))
        {
            if (activate)
            {
                tankShooting.homing = true;
            }
            else
            {
                tankShooting.homing = false;
            }
        }
        else if (poweruptype.Contains("Explosive"))
        {
            if (activate)
            {
                tankShooting.explosive = true;
            }
            else
            {
                tankShooting.explosive = false;
            }
        }
        else if (poweruptype.Contains("Omniwall"))
        {
            if (activate)
            {
                if(NetworkManager.IsServer)
                    SpawnWallServerRPC(false, worldToLocal(tacticalSpawnPos.position, battleField.transform), tacticalSpawnPos.rotation);
            }
        }
        else if(poweruptype.Contains("Allowall"))
        {
            if(activate)
            {
                if (NetworkManager.IsServer)
                    SpawnWallServerRPC(true, worldToLocal(tacticalSpawnPos.position, battleField.transform), tacticalSpawnPos.rotation);
            }
        }
    }

    public void PassiveAddHealth()
    {
        tankHealth.m_CurrentHealth += healthAdditionTime * tankHealth.m_StartingHealth / 12;
        if(tankHealth.m_CurrentHealth >= tankHealth.m_StartingHealth)
        {
            tankHealth.m_CurrentHealth = tankHealth.m_StartingHealth;
            EndPowerupEarly();
        }
    }

    public void EndPowerupEarly()
    {
        foreach (GameObject uiPowerup in GameObject.FindGameObjectsWithTag("UIPowerup"))
        {
            if (uiPowerup.name.Contains("Health"))
            {
                uiPowerup.GetComponent<UIPowerup>().PowerupSlider.GetComponent<PowerupSliderController>().EndPowerupEarly();
                break;
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void SpawnWallServerRPC(bool allow, Vector3 localpos, Quaternion spawnRot, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) { return; }
        var clientId = serverRpcParams.Receive.SenderClientId;
        Vector3 newPos = new Vector3(localpos.x, 0, localpos.z);
        GameObject instantiateWall;
        if(allow)
        {
            instantiateWall = alloWall;
        }
        else
        {
            instantiateWall = omniWall;
        }
        GameObject newWall = Instantiate(instantiateWall, newPos, spawnRot, GameObject.FindGameObjectWithTag("Battlefield").transform);

        newWall.transform.parent = battleField.transform;
        newWall.transform.localPosition =  newPos;
            
        newWall.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, false);
    }
    private Vector3 worldToLocal(Vector3 worldpos, Transform battlefield)
    {
        return battleField.transform.InverseTransformPoint(worldpos);
    }

    private Vector3 localToWorld(Vector3 localpos, Transform battlefield)
    {
        return battleField.transform.TransformPoint(localpos);
    }
}

