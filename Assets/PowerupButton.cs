using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class PowerupButton : MonoBehaviour
{
    [SerializeField] Canvas myCanvas;
    [SerializeField] GameObject button3d;
    [SerializeField] public int cost = 10;
    [SerializeField] public TextMeshProUGUI costText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myCanvas.worldCamera = Camera.main;
        costText.text = "Cost: " + cost.ToString() + " coins";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EngageButton()
    {
        //if is able to purchase powerup
        if (GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().TotalCoins.Value >= cost)
        {
            GameObject.Find("PowerupManager").GetComponent<PowerupManager>().SpawnPowerup(gameObject.name);
            button3d.GetComponent<PlayableDirector>().Play();
            GameObject.FindGameObjectWithTag("MyTank").GetComponent<CoinWallet>().UpdateCoinsServerRPC(-1*cost);
        }
    }
}
