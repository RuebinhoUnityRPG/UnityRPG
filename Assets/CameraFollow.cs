using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    #region Variables

    private GameObject player;

#endregion

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
    
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LateUpdate()
    {
        transform.position = player.transform.position;
    }
}
