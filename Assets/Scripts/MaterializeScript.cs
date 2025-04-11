using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Playables;
public class MaterializeScript : MonoBehaviour
{
    [SerializeField]
    private float scaleSize = 1;
    private GameObject matGO;  
    private Material newMat;
    private Material[] oldMat;
    private float startTime;
    private bool done = false;
    private float depth;
    private float transTime;
    private float rot;
    private float depthOrig;
    private float rotOrig;
    [SerializeField] bool playAudio;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        oldMat = this.gameObject.GetComponent<MeshRenderer>().materials;
        newMat = Resources.Load<Material>("Materialize Material");
        startTime = Time.time;
        depth = Random.Range(.2f, .6f);
        transTime = Random.Range(3.0f, 6.0f);
        rot = Random.Range(180f, 360f);
        Debug.Log("Rot:" + rot);
        depthOrig = this.transform.position.y - depth;
        rotOrig = this.transform.rotation.eulerAngles.x - rot;
        ResetObject();        
    }

	private void ResetObject()
	{
        this.gameObject.GetComponent<MeshRenderer>().material = newMat;
        Material[] mats = new Material[this.gameObject.GetComponent<MeshRenderer>().materials.Length];
        for (int i = 0; i < this.gameObject.GetComponent<MeshRenderer>().materials.Length; i++) {
            mats[i] = newMat;
        }
        this.gameObject.GetComponent<MeshRenderer>().materials = mats;

        this.transform.position = new Vector3(this.transform.position.x, depthOrig, this.transform.position.z);
        this.transform.eulerAngles = new Vector3(this.transform.rotation.eulerAngles.x, rotOrig, this.transform.rotation.eulerAngles.z);
        matGO = GameObject.Instantiate(Resources.Load<GameObject>("MaterializePF"), this.transform.parent);
        matGO.transform.position = new Vector3(this.transform.position.x, depthOrig + depth, this.transform.position.z);
        matGO.transform.localScale = scaleSize * new Vector3(.3f, .3f, .3f);
        if (playAudio)
        {
            AudioClip ac = Resources.Load<AudioClip>("thunder");
            matGO.GetComponent<AudioSource>().PlayOneShot(ac);
        }
    }
	// Update is called once per frame
	void Update()
    {


        if (!done)
        {
            if (Time.time - startTime < transTime) {
                float depthUp = depth *(Time.time - startTime) / transTime;
                float rotUp = rot * (Time.time - startTime) / transTime;
                float dissolve = 2 - (Time.time - startTime)*2 / transTime;
                this.transform.position = new Vector3(this.transform.position.x, depthOrig + depthUp, this.transform.position.z);
                this.transform.eulerAngles = new Vector3(this.transform.rotation.eulerAngles.x, rotOrig+ rotUp, this.transform.rotation.eulerAngles.z);
                foreach (Material mat in this.GetComponent<MeshRenderer>().materials)
                {
                    mat.SetFloat("_Clip", dissolve);
                }
                Debug.Log("dissolve: " +dissolve);
            }
            else
            {
                Debug.Log("Material changed");
             
                this.gameObject.GetComponent<MeshRenderer>().materials = oldMat;
                Destroy(matGO, 0.5f);
                done = true;
                if (playAudio)
                {
                    matGO.GetComponent<AudioSource>().Stop();
                }
            }
        }

    }
}

