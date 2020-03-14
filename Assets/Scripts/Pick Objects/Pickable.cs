using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (
    typeof (Collider2D), 
    typeof (SpriteRenderer))
]
public class Pickable 
    : MonoBehaviour 
{
    // Components
    protected Collider2D cmp_collider;
    protected SpriteRenderer cmp_renderer;

    [Header ("Wrap Settings")]
    public Sprite spriteWrap;
    protected Sprite spriteOriginal;

    // State
    protected bool wrapped;

    // References
    protected FieldCell cell;

    public FieldCell Cell
    {
        get { return cell; }
        set { cell = value; }
    }
    public bool isWrapped
    {
        get { return wrapped; }
        set { wrapped = value; }
    }

    protected virtual void Awake()
    {
        // Retrieve components
        this.cmp_collider = GetComponent<Collider2D>();
        this.cmp_renderer = GetComponent<SpriteRenderer>();

        // Save the original sprite
        this.spriteOriginal = this.cmp_renderer.sprite;
        // Wrap item
        this.wrapped = true;
        this.cmp_renderer.sprite = this.spriteWrap;
    }

    // Unveil object
    public void Unwrap()
    {
        // Unwrap item
        this.wrapped = false;
        this.cmp_renderer.sprite = this.spriteOriginal;
    }

    // Use object
    public virtual void Use (Protagonist trigger)
    { }
}
