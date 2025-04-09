using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{

    private static ClientSingleton instance;

    public ClientGameManager GameManager { get; private set; }

    public static ClientSingleton Instance
    {
        get
        {
            Debug.Log("ClientSingleton Instance");
            if (instance != null) {
                Debug.Log("ClientSingleton Instance:" + instance.gameObject.name);
                return instance; 
            }
            Debug.Log("Find ClientSingleton Instance");
            instance = FindObjectOfType<ClientSingleton>();

            if (instance == null)
            {
                
                Debug.LogError("No ClientSingleton in the scene!");
                return null;
                
            }

            return instance;
        }
    }

    private void Start()
    {
        Debug.Log("ClientSingleton Start1");
        DontDestroyOnLoad(gameObject);
        Debug.Log("ClientSingleton Start2");
    }
    public async Task<bool> CreateClient(string username)
    {
        Debug.Log("Create Client");
        GameManager = new ClientGameManager();
        return await GameManager.InitAsync(username);
 
    }
}
