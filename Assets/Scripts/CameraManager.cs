using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public Camera m_mainCamera;
    private float m_defaultSize = 0;
    public float m_zoomedSize = 3;

    private void Start()
    {
        m_defaultSize = m_mainCamera.orthographicSize;
    }

    public void ZoomIn(float _timer = 0)
    {
        m_mainCamera.orthographicSize = m_zoomedSize;
        if(_timer != 0)
        {
            StartCoroutine(IZoomOut(_timer));
        }
    }

    private IEnumerator IZoomOut(float _timer)
    {
        yield return new WaitForSeconds(_timer);
        ZoomOut();
    }

    public void ZoomOut()
    {
        m_mainCamera.orthographicSize = m_defaultSize;
    }
}
