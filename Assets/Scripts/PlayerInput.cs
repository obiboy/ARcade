using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {

    public LayerMask targetLayerMask;
    private Camera cam;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
		if (GameManager.instance.gameState == GameState.Running) {
			KeyboardInput ();
		}
    }

    private void KeyboardInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        //check for target
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, targetLayerMask))
        {
            Target target = hit.collider.GetComponentInParent<Target>();
            target.Hit(hit);
        }
    }
}
