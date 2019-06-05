using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueuedAnimatedTiledTexture : AnimateTiledTexture {

    public List<CustomAnimationTextureModel> m_queuedAnimations;
    private bool m_isQueueMoves = true;

    private void Start()
    {
        m_queuedAnimations = new List<CustomAnimationTextureModel>();
    }

    public bool QueueMoves
    {
        set { m_isQueueMoves = value; }
        get { return m_isQueueMoves; }
    }

    public void ChangeAnimationFPS(int _fps)
    {
        base._framesPerSecond = _fps;
    }

    public bool PlayOnce
    {
        get { return _playOnce; }
        set { _playOnce = value; }
    }

    public bool IsPlaying()
    {
        return _isPlaying;
    }

    public void ChangeCustomAnimationMaterial(Material _material, int _rows, int _columns, int _ignoredAnimationFrames)
    {
        this._rows = _rows;
        this._columns = _columns;
        this.m_ignoredAnimationFrames = _ignoredAnimationFrames;
        base.ChangeMaterial(_material);
    }

    public void StopAllAnimations()
    {
        StopAllCoroutines();
    }

    public void Play()
    {
        StartCoroutine(IPlay());
    }

    public override IEnumerator IPlay()
    {
        if (m_isQueueMoves)
        {
            while (_isPlaying)
            {
                yield return null;
            }
            if(m_queuedAnimations.Count > 0)
            {
                CustomAnimationTextureModel m_temp = m_queuedAnimations[0];
                ChangeCustomAnimationMaterial(m_temp.Material, m_temp.Rows, m_temp.Columns, m_temp.FrameSkips);
                m_queuedAnimations.Remove(m_temp);
            }
        }
        // Make sure the renderer is enabled
        GetComponent<Renderer>().enabled = true;

        //Because of the way textures calculate the y value, we need to start at the max y value
        _index = _columns;

        // Start the update tiling coroutine
        f_updateTiling = StartCoroutine(updateTiling());
    }
    
    public void StopCurrentAnimation()
    {
        StopCoroutine(f_updateTiling);
    }

}
