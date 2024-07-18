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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myText.text = cost.ToString() + " coins";
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
        if (GameObject.FindGameObjectWithTag("MyTank") != null)
        {
            if (GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().TotalCoins.Value >= cost)
            {
                Debug.Log("Pressed powerup button");
                GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().UpdateCoinsServerRPC(-1 * cost);
                GameObject.Find("PowerupManager").GetComponent<PowerupManager>().SpawnPowerup(name);
            }
            else
            {
                Debug.Log("Ur poor");
            }
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
