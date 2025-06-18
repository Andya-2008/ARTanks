using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPowerup : MonoBehaviour
{

    [SerializeField] public int cost;
    [SerializeField] float initCoverupHeight = 200;
    [SerializeField] float initCoverupWidth = 133.3333f;
    [SerializeField] RectTransform myCoverup;
    [SerializeField] TextMeshProUGUI myText;
    [SerializeField] string powerupDisplayName;
    [SerializeField] int numOfBulletsOrTime;

    [SerializeField] public GameObject PowerupSlider;
    [SerializeField] public bool thisPowerupActive = false;

    //[SerializeField] public bool bulletBased;
    [SerializeField] public int powerUpType;
    // 0 = bullet based
    // 1 = time based
    // 2 = permanent multiplier

    [SerializeField] bool noTiming = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myText.text = cost.ToString() + " coins";
        PowerupSlider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectWithTag("MyTank") != null)
        {
            UpdateCoinCoverup();
        }
    }

    public void OnPressPowerupButton()
    {
        if (!thisPowerupActive)
        {
            
            if (GameObject.FindGameObjectWithTag("MyTank") != null)
            {
                if (GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().TotalCoins.Value >= cost)
                {
                    // If it's multiplier type and already maxed out, then you can't activate any more.
                    if (powerUpType==2 && PowerupSlider.GetComponent<PowerupSliderController>().multiplier >= PowerupSlider.GetComponent<PowerupSliderController>().maxMultiplier) {
                        return;
                    }
                    Debug.Log("Pressed powerup button");
                    GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().UpdateCoinsServerRPC(-1 * cost);
                    GameObject.Find("PowerupManager").GetComponent<PowerupManager>().SpawnPowerup(name, powerupDisplayName, numOfBulletsOrTime, powerUpType);
                    Debug.Log("2");

                    if (!noTiming)
                    {
                        thisPowerupActive = true;
                        PowerupSlider.SetActive(true);
                        PowerupSlider.GetComponent<PowerupSliderController>().powerup = name;
                        PowerupSlider.GetComponent<PowerupSliderController>().powerUpType = powerUpType;
                        PowerupSlider.GetComponent<PowerupSliderController>().totalBulletsOrTime = numOfBulletsOrTime;
                        PowerupSlider.GetComponent<PowerupSliderController>().bulletsFired = 0;
                        PowerupSlider.GetComponent<PowerupSliderController>().powerupCoverUp.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 211);
                        if (powerUpType == 0) {
                            
                        }
                        // Timer based
                        else if (powerUpType == 1)
                        {
                      
                            PowerupSlider.GetComponent<PowerupSliderController>().startPowerupTime = Time.time;
                        }
                        // Multiplier Based
                        else if (powerUpType == 2)
                        {
                            thisPowerupActive = false;
                            PowerupSlider.GetComponent<PowerupSliderController>().multiplier += 1;
                        }
                    }
                }
                else
                {
                    Debug.Log("Ur poor");
                }
            }
        }
        else
        {
            Debug.Log("Powerup already active!");
        }
    }

    public void UpdateCoinCoverup()
    {
        float newCoverupHeight;
        if (cost > 0)
        {
            newCoverupHeight = initCoverupHeight - initCoverupHeight * GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().TotalCoins.Value / cost;
        }
        else
        {
            newCoverupHeight = 0;
        }
        if(newCoverupHeight < 0)
        {
            newCoverupHeight = 0;
        }
        
        myCoverup.sizeDelta = new Vector2(initCoverupWidth, newCoverupHeight);
    }
}
