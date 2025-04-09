using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
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
    TankPlayer tankPlayer;
    TankLaser tankLaser;

    [SerializeField] Transform tacticalSpawnPos;

    GameObject battleField;
    [SerializeField] GameObject omniWall;
    [SerializeField] GameObject alloWall;
    [SerializeField] GameObject CoinFactory;
    [SerializeField] GameObject repairFactory;
    [SerializeField] List<GameObject> shields = new List<GameObject>();
    [SerializeField] GameObject shieldParent;

    public List<GameObject> enemyTanks;
    // Start is called before the first frame update
    void Start()
    {
        shields = shieldParent.GetComponent<ShieldParent>().shields;
        tankMovement = this.GetComponent<TankMovement>();
        tankShooting = this.GetComponent<TankShooting>();
        tankHealth = this.GetComponent<TankHealth>();
        tankPlayer = this.GetComponent<TankPlayer>();
        tankLaser = this.GetComponent<TankLaser>();
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
    public void ActivateOrDeactivateTankPowerup(string poweruptype, bool activate)
    {
        if (poweruptype.Contains("OilSpill"))
        {
            ActivateOrDeactivateOtherTankPowerupRPC(poweruptype, activate);
             
        }
        else if(poweruptype.Contains("EMPBlast"))
        {
            ActivateOrDeactivateOtherTankPowerupRPC(poweruptype, activate);
        }
        else
        {
            GameObject.Find("SFXManager").GetComponent<SFXManager>().PlaySFXRPC("UsePowerup");
            if(poweruptype.Contains("TankSpeed") && activate)
            {
                GameObject.Find("SFXManager").GetComponent<SFXManager>().PlaySFXRPC("Turbo");

            }
            ActivateOrDeactivateTankPowerupRPC(poweruptype, activate);
        }
    }
    [Rpc(SendTo.NotMe)]
    public void ActivateOrDeactivateOtherTankPowerupRPC(string poweruptype, bool activate)
    {
        if (poweruptype.Contains("OilSpill"))
        {
            if (activate)
            {
                GameObject.Find("DisruptionCanvas").GetComponent<OilSpillManager>().StartOilSpill();
            }
            else
            {
                GameObject.Find("DisruptionCanvas").GetComponent<OilSpillManager>().StopOilSpill();
            }
        }
        else if (poweruptype.Contains("EMPBlast"))
        {
            if (activate)
            {
                GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankShooting>().canShoot = false;
            }
            else
            {
                GameObject.FindGameObjectWithTag("MyTank").GetComponent<TankShooting>().canShoot = true;
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ActivateOrDeactivateTankPowerupRPC(string poweruptype, bool activate)
    {
        Debug.Log("1");
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
        if (poweruptype.Contains("BulletRange"))
        {
            if (activate)
            {
                tankShooting.m_BulletRange -= 1.5f;
            }
            else
            {
                tankShooting.m_BulletRange += 1.5f;
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
            if (activate)
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
            if (activate)
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
                else if (tankHealth.m_CurrentHealth >= 5)
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
        else if (poweruptype.Contains("Invisibility"))
        {
            if (activate)
            {
                if (gameObject.tag != "MyTank")
                {
                    tankPlayer.ToggleInvisibility(true);
                }
            }
            else
            {
                if (gameObject.tag != "MyTank")
                {
                    tankPlayer.ToggleInvisibility(false);
                }
            }
        }
        else if (poweruptype.Contains("Tanksformation"))
        {
            if (activate)
            {
                transform.localScale = new Vector3(.6f, .6f, .6f);
                tankMovement.m_Speed += .05f;
            }
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                tankMovement.m_Speed -= .05f;
            }
        }
        else if (poweruptype.Contains("PhaseShift"))
        {
            if (activate)
            {
                this.gameObject.layer = 12;
            }
            else
            {

                if (NetworkObject.IsOwner)
                {
                    this.gameObject.layer = 6;
                }
                else
                {
                    this.gameObject.layer = 3;
                }
            }
        }
        else if (poweruptype.Contains("Basic_Shield"))
        {
            if (activate)
            {
                shields[0].SetActive(true);
            }
            else
            {

                shields[0].SetActive(false);
            }
        }
        else if (poweruptype.Contains("VampireBullets"))
        {
            if(activate)
            {
                tankShooting.vampire = true;
            }
            else
            {
                tankShooting.vampire = false;
            }
        }
        else if (poweruptype.Contains("PhantomRounds"))
        {
            if (activate)
            {
                tankShooting.phantom = true;
            }
            else
            {
                tankShooting.phantom = false;
            }
        }
        else if (poweruptype.Contains("Golden_Shield"))
        {
            if (activate)
            {
                shields[1].SetActive(true);
            }
            else
            {

                shields[1].SetActive(false);
            }
        }
        else if (poweruptype.Contains("Absorption_Shield"))
        {
            if (activate)
            {
                shields[2].SetActive(true);
            }
            else
            {
                shields[2].GetComponent<ShieldController>().ShieldHealth = 100f;
                shields[2].SetActive(false);
            }
        }

        else if (poweruptype.Contains("Annihilaser"))
        {
            if (activate)
            {
                tankMovement.canMove = false;
                StartCoroutine(tankLaser.Shoot());
            }
            else
            {
                tankMovement.canMove = true;
                tankLaser.StopShoot();
            }
        }
        else if (poweruptype.Contains("Molotov"))
        {
            if (activate)
            {

                tankShooting.molotov = true;
            }
            else
            {
                tankShooting.molotov = false;

            }
        }
        else if (poweruptype.Contains("Shock"))
        {
            if (activate)
            {
                tankShooting.shock = true;
            }
            else
            {
                tankShooting.shock = false;
            }
        }

        else if (poweruptype.Contains("EMPNet"))
        {
            if (activate)
            {

            }
            else
            {

            }
        }
        else if (poweruptype.Contains("Hydraulic"))
        {
            if (activate)
            {
                tankMovement.hydraulic = true;
            }
            else
            {
                tankMovement.hydraulic = false;
            }
        }
        else if (poweruptype.Contains("Flame_Turret"))
        {
            if (activate)
            {

            }
            else
            {

            }
        }
        else if (poweruptype.Contains("Lightning_Turret"))
        {
            if (activate)
            {

            }
            else
            {

            }
        }
        else if (poweruptype.Contains("Sniper_Turret"))
        {
            if (activate)
            {

            }
            else
            {

            }
        }
        else if (poweruptype.Contains("MiniTank"))
        {
            if (activate)
            {

            }
            else
            {

            }
        }
        else if (poweruptype.Contains("Artillery"))
        {
            if (activate)
            {

            }
            else
            {

            }
        }
        else if (poweruptype.Contains("Omniwall"))
        {
            if (activate)
            {
                if (NetworkManager.IsServer)
                    SpawnBuildingServerRPC("omniwall", worldToLocal(tacticalSpawnPos.position, battleField.transform), tacticalSpawnPos.rotation);
            }
        }
        else if (poweruptype.Contains("Allowall"))
        {
            if (activate)
            {
                if (NetworkManager.IsServer)
                    SpawnBuildingServerRPC("allowall", worldToLocal(tacticalSpawnPos.position, battleField.transform), tacticalSpawnPos.rotation);
            }
        }
        else if (poweruptype.Contains("CoinFactory"))
        {
            if (activate)
            {
                if (NetworkManager.IsServer)
                    SpawnBuildingServerRPC("coinfactory", worldToLocal(tacticalSpawnPos.position, battleField.transform), tacticalSpawnPos.rotation);
            }
        }
        else if (poweruptype.Contains("RepairBot"))
        {
            if (activate)
            {
                if (NetworkManager.IsServer)
                    SpawnBuildingServerRPC("repairfactory", worldToLocal(tacticalSpawnPos.position, battleField.transform), tacticalSpawnPos.rotation);
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
    public void SpawnBuildingServerRPC(string building, Vector3 localpos, Quaternion spawnRot, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) { return; }
        var clientId = serverRpcParams.Receive.SenderClientId;
        Vector3 newPos = new Vector3(localpos.x, 0, localpos.z);
        GameObject instantiateWall;
        if (building == "allowall")
        {
            instantiateWall = alloWall;
        }
        else if (building == "omniwall")
        {
            instantiateWall = omniWall;
        }
        else if (building == "coinfactory")
        {
            instantiateWall = CoinFactory;
        }
        else if (building == "repairfactory")
        {
            instantiateWall = repairFactory;
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

