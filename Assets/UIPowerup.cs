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

    [SerializeField] public bool bulletBased;
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
            thisPowerupActive = true;
            if (GameObject.FindGameObjectWithTag("MyTank") != null)
            {
                if (GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().TotalCoins.Value >= cost)
                {
                    
                    Debug.Log("Pressed powerup button");
                    GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().UpdateCoinsServerRPC(-1 * cost);
                    GameObject.Find("PowerupManager").GetComponent<PowerupManager>().SpawnPowerup(name, powerupDisplayName, numOfBulletsOrTime, bulletBased);
                    Debug.Log("2");
                    PowerupSlider.SetActive(true);

                    PowerupSlider.GetComponent<PowerupSliderController>().powerup = name;
                    PowerupSlider.GetComponent<PowerupSliderController>().bulletBased = bulletBased;
                    PowerupSlider.GetComponent<PowerupSliderController>().totalBulletsOrTime = numOfBulletsOrTime;
                    PowerupSlider.GetComponent<PowerupSliderController>().bulletsFired = 0;
                    PowerupSlider.GetComponent<PowerupSliderController>().powerupCoverUp.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 211);
                    if (!bulletBased)
                    {
                        PowerupSlider.GetComponent<PowerupSliderController>().startPowerupTime = Time.time;
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
