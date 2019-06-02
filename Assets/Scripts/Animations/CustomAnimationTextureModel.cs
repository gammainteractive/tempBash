using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomAnimationTextureModel {

    public Material m_material;
    public int m_rows;
    public int m_columns;
    public int m_frameSkips;

    public CustomAnimationTextureModel(Material m_material, int m_rows, int m_columns, int m_frameSkips)
    {
        this.m_material = m_material;
        this.m_rows = m_rows;
        this.m_columns = m_columns;
        this.m_frameSkips = m_frameSkips;
    }

    public Material Material
    {
        get
        {
            return m_material;
        }

        set
        {
            m_material = value;
        }
    }

    public int Rows
    {
        get
        {
            return m_rows;
        }

        set
        {
            m_rows = value;
        }
    }

    public int Columns
    {
        get
        {
            return m_columns;
        }

        set
        {
            m_columns = value;
        }
    }

    public int FrameSkips
    {
        get
        {
            return m_frameSkips;
        }

        set
        {
            m_frameSkips = value;
        }
    }
}
