using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinRotator : MonoBehaviour
{

	private void Awake()
	{
		this.transform.Rotate(new Vector3(90, 0, 0));
	}
	// Update is called once per frame
	void FixedUpdate()
	{
		this.transform.Rotate(new Vector3(0f, 0f, 1f));
	}
}
