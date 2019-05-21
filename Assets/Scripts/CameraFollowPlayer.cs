using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

    public Transform m_player;
    public Vector3 m_cameraOffset;

    // Use this for initialization
    void Start () {
		
	}

    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = m_player.transform.position + m_cameraOffset;
    }
}
