using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PewPew : MonoBehaviour {
	
	public float MaxLifeTime = 4f;


	// Use this for initialization
	void Start () 
	{
		Destroy (gameObject, MaxLifeTime);
	}



	void OnTriggerEnter(Collider other)
	{
		other.GetComponent<ShipHealth> ().CurrentHealth -= 10;
	}


	// Update is called once per frame
	void Update ()
	{
		
		
	}
}
