//using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class PowerupSliderController : MonoBehaviour
{
    //public bool bulletBased;
    public int powerUpType;
    // 0 = bullet based
    // 1 = time based
    // 2 = permanent multiplier
    // 3 = other

    public int bulletsFired;

    public int totalBulletsOrTime;

    public string powerup;

    public GameObject powerupCoverUp;

    [SerializeField] GameObject PowerupParent;

    public float startPowerupTime;

    public bool endPowerup;

    public int multiplier;
    public int maxMultiplier = 5;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(powerupCoverUp.GetComponent<RectTransform>().rect.height <= 0 || endPowerup)
        {
            endPowerup = false;
            GameObject.Find("PowerupManager").GetComponent<PowerupManager>().DisablePowerup(powerup);
            PowerupParent.GetComponent<UIPowerup>().thisPowerupActive = false;
            powerupCoverUp.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 211);
            bulletsFired = 0;
            // bulletBased = false;
            powerup = "";
            gameObject.SetActive(false);
        }
        if (powerUpType == 0)
        {
            powerupCoverUp.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 211 - 211 * bulletsFired / totalBulletsOrTime);
        }
        else if (powerUpType == 1)
        {
            powerupCoverUp.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 211 - 211 * (Time.time - startPowerupTime) / totalBulletsOrTime);
        }
        else if (powerUpType == 2) {
            powerupCoverUp.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 211 * multiplier / maxMultiplier);
        }
    }

    public void BulletFired()
    {
        bulletsFired++;
    }

    public void EndPowerupEarly()
    {
        endPowerup = true;
    }
}
