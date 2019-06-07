using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhasesUI : MonoBehaviour {


    public enum UI_OBJECTS
    {
        MODE_C_FAIL,
        MODE_C_SUCCESS,
        MODE_C_WATCH,
        MODE_C_REPEAT,
        MODAL,
    }

    public GameObject[] m_objects;
    
    public void DisableAllObjects()
    {
        for (int i = 0; i < m_objects.Length; i++)
        {
            m_objects[i].SetActive(false);
        }
    }

    public void ShowModeCResult(bool _isSuccess)
    {
        DisableAllObjects();
        m_objects[(int)UI_OBJECTS.MODAL].SetActive(true);
        if (_isSuccess)
        {
            m_objects[(int)UI_OBJECTS.MODE_C_SUCCESS].SetActive(true);
        }
        else
        {
            m_objects[(int)UI_OBJECTS.MODE_C_FAIL].SetActive(true);
        }
    }

    public void ShowPhase(int _uiPhase)
    {
        DisableAllObjects();
        m_objects[(int)UI_OBJECTS.MODAL].SetActive(true);
        m_objects[_uiPhase].SetActive(true);
    }
}
