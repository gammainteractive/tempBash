using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    public Transform m_mainCharacter;
    public enum MOVEMENT_DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    [Header("Movement Parameters")]
    public float m_walkSpeed = 0.02f;

	// Use this for initialization
	void Start () {
        m_mainCharacter = transform;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.W))
        {
            MoveCharacter(MOVEMENT_DIRECTION.UP);
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveCharacter(MOVEMENT_DIRECTION.LEFT);
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveCharacter(MOVEMENT_DIRECTION.DOWN);
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveCharacter(MOVEMENT_DIRECTION.RIGHT);
        }
    }

    public void MoveCharacter(MOVEMENT_DIRECTION _direction)
    {
        Vector3 m_movement = Vector3.zero;
        switch (_direction)
        {
            case MOVEMENT_DIRECTION.UP:
                m_movement = new Vector3(0, m_walkSpeed, 0);
            break;

            case MOVEMENT_DIRECTION.DOWN:
                m_movement = new Vector3(0, -m_walkSpeed, 0);
            break;

            case MOVEMENT_DIRECTION.LEFT:
                Debug.Log("Left");
                m_movement = new Vector3(-m_walkSpeed, 0, 0);
            break;

            case MOVEMENT_DIRECTION.RIGHT:
                m_movement = new Vector3(m_walkSpeed, 0, 0);
                break;
        }
        Debug.Log("Movement: " + m_movement);
        m_mainCharacter.position += m_movement;
    }
}
