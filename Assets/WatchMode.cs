using UnityEngine;
using Unity.Cinemachine;
public class WatchMode : MonoBehaviour
{

    [SerializeField] private CinemachineTargetGroup ctg;
    [SerializeField] private Canvas c1;
    [SerializeField] private Canvas c2;
    [SerializeField] private Canvas c3;
    [SerializeField] private Canvas c4;
    [SerializeField] private Canvas c5;

    private Canvas c6;
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private void Start()
	{
        c6 = GameObject.Find("RoomCodeCanvas").GetComponent<Canvas>();

	}

	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C");

            GameObject[] tanks = GameObject.FindGameObjectsWithTag("Tank");
            foreach (GameObject tankGO in tanks)
            {
                CinemachineTargetGroup.Target t = new CinemachineTargetGroup.Target { Object = tankGO.transform, Radius = 1, Weight = 1 };
                ctg.Targets.Add(t);
            }
            GameObject[] tanks2 = GameObject.FindGameObjectsWithTag("MyTank");
            foreach (GameObject tankGO in tanks2)
            {
                CinemachineTargetGroup.Target t = new CinemachineTargetGroup.Target { Object = tankGO.transform, Radius = 1, Weight = 1 };
                ctg.Targets.Add(t);
            }

            c1.enabled = false;
            c2.enabled = false;
            c3.enabled = false;
            c4.enabled = false;
            c5.enabled = false;
            c6.enabled = false;

        }
    }
}
