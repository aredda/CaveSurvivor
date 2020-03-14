using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FieldCell 
    : MonoBehaviour
{
    // Input
    protected FieldCellType type;
    protected IntVector2 position;
    protected FieldCell[] edges = new FieldCell[4];
    protected bool walkable = false;

    #region Properties

    public IntVector2 Position
    {
        get { return position; }
        set { position = value; }
    }
    public FieldCell[] Edges
    {
        get { return edges; }
        set { edges = value; }
    }
    public FieldCellType Type
    {
        get { return type; }
        set { type = value; }
    }
    public bool IsWalkable
    {
        get { return walkable; }
        set { walkable = value; }
    }

    #endregion

    // Temporary (Or just for testing)
    public void SetSpriteRendererColor(Color color) 
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        try
        {
            if (sr == null)
                throw new Exception("This object does not contain a sprite renderer!");

            sr.color = color;
        }
        catch (Exception err)
        {
            Debug.Log(err.Message);
        }
    }
    public void SetSprite(Sprite s)
    {
        if (this.GetComponent<SpriteRenderer>() == null)
            throw new Exception("A SpriteRendererComponent is expected.");

        this.GetComponent<SpriteRenderer>().sprite = s;
    }
}
