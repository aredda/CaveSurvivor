using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Requires an animator
[RequireComponent(typeof(Animator))]
// Requires a collider
[RequireComponent(typeof(Collider2D))]
// Requires a rigidbody
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Humanoid 
    : MonoBehaviour 
{
    // Communicating with the game manager
    protected Manager gameManager;

    // Own properties
    public int health = 100;

    // Object components
    protected Animator cmp_animator;
    protected Collider2D cmp_collider;
    protected Rigidbody2D cmp_rigidbody;

    // Utility properties
    protected bool isMoving = false;                        // Indicates whether the humanoid is moving or not
    protected Direction faceDirection = Direction.Right;    // Default face direction

    // Getters / Setters
    public Manager GameManager
    {
        get { return gameManager; }
        set { gameManager = value; }
    }
    public IntVector2 Position
    {
        get {
            return new IntVector2(this.transform.position);
        }
    }
    public FieldCell Cell
    {
        get { return this.gameManager.CaveGenerator.FindCell(this.Position); }
    }

    // Overriding super class methods
    protected virtual void Awake()
    {
        // Retrieve components
        this.cmp_animator = GetComponent<Animator>();
        this.cmp_collider = GetComponent<Collider2D>();
        this.cmp_rigidbody = GetComponent<Rigidbody2D>();

        // Initial settings
        this.cmp_rigidbody.gravityScale = 0;    // Disable gravity for this object
        this.cmp_collider.isTrigger = true;     // Make it a trigger collider

        // Stand animation is the initial animation
        this.ResetAnimations();
    }

    // Gameplay-Related Methods
    public virtual void Move(Direction direction)
    {
        // Just in case
        StopAllCoroutines();
        // Face that direction
        StartCoroutine(this.IFace(DirectionGuide.getRotation(direction, this.faceDirection)));
        // He faces that direction now
        this.faceDirection = direction;
        // Check if that destination exists and it is walkable
        IntVector2 dest = new IntVector2(DirectionGuide.getDirection(direction) + this.transform.position);
        FieldCell destCell = this.GameManager.CaveGenerator.FindCell(dest);
        Collider2D detectedObject = this.Detect();
        if (destCell != null) // If there is a cell in that direction
        {
            if (detectedObject != null)     // if the destination's cell is occupied?
                this.PickAction(detectedObject);
            else if (destCell.IsWalkable)
            {
                // The current position shall be marked as walkable again
                this.gameManager.CaveGenerator.MarkCellAsWalkable(this.Cell);
                // The destination mustn't be walkable anymore
                this.gameManager.CaveGenerator.MarkCellAsUnwalkable(destCell);
                // Move
                StartCoroutine(this.IMove(DirectionGuide.getDirection(direction)));
            }
        }
    }
    public virtual Collider2D Detect()
    {
        // Get the real direction
        Vector2 lookDirection = DirectionGuide.getDirection(this.faceDirection).ToVector2();
        Vector2 currentPos = this.transform.position;
        Vector2 destination = lookDirection + currentPos;
        // Disable this collider
        this.cmp_collider.enabled = false;
        // Cast a line
        RaycastHit2D hit = Physics2D.Linecast(currentPos, destination);
        // Enable collider
        this.cmp_collider.enabled = true;
        // Pass the result
        return hit.collider;
    }
    public virtual void TakeDamage(int damage)
    {
        if (damage >= health)
        {
            this.health = 0;
            this.Die();

            return;
        }

        // Play hurt animation
        this.PlayAnimation("State_Hurt");
        
        this.health -= damage;
    }
    public virtual void Die()
    {
        // Play Death Animation
        ParticleSystem p = this.gameManager.subResource.GetPreMadeResource<ParticleSystem>("Death");
        ResourceManager.PlayVFX(p, this.Position.ToVector2());
        // Make the spot available
        this.gameManager.CaveGenerator.MarkCellAsWalkable(this.Cell);
        // Deactivate object
        this.gameObject.SetActive(false);
    }
    public virtual void AttackAction(Collider2D c) {}
    public virtual void PickAction(Collider2D c) {}

    // Animation-Related Methods
    protected void ResetAnimations()
    {
        foreach (AnimatorControllerParameter p in this.cmp_animator.parameters)
            this.cmp_animator.SetBool(p.name, false);
    }
    protected void PlayAnimation(string parameter)
    {
        // Turn off all animations
        this.ResetAnimations();
        // Search this parameter
        AnimatorControllerParameter p = SearchAnimatorParameter(parameter);
        if (p == null)
            throw new Exception("Catched Bug: " + parameter + " can't be found!");
        // Therefore turn on that seleceted animation
        switch (p.type)
        {
            case AnimatorControllerParameterType.Bool:
                this.cmp_animator.SetBool(parameter, true);     break;
            case AnimatorControllerParameterType.Trigger:
                this.cmp_animator.SetTrigger(parameter);        break;
        }
    }
    protected AnimatorControllerParameter SearchAnimatorParameter(string parameter)
    {
        for (int i = 0; i < this.cmp_animator.parameterCount; i++)
            if (this.cmp_animator.GetParameter(i).name == parameter)
                return this.cmp_animator.GetParameter(i);
        return null;
    }

    // Time-Related Methods
    protected virtual IEnumerator IMove(IntVector2 direction, float transitionSpeed = 2.5f)
    {
        // Create shortcuts to decrease the quantity of the code
        Rigidbody2D r = this.cmp_rigidbody;
        Vector2 destination = direction + r.position;
        WaitForEndOfFrame eof = new WaitForEndOfFrame();

        // Move state is true
        this.isMoving = true;
        // Move animation
        this.PlayAnimation("State_Walk");

        float distance;
        do
        {
            // Approching destination
            r.position = Vector2.MoveTowards(r.position, destination, transitionSpeed * Time.deltaTime);
            // Update the distance variable
            distance = Vector2.Distance(r.position, destination);

            yield return eof;
        }
        while( !Mathf.Approximately(distance, 0) );

        // Move state is false
        this.isMoving = false;
        // Reset & stop playing the current animation
        this.ResetAnimations();
    }
    protected IEnumerator IFace(float angle, float turnSpeed = 360f)
    {
        Rigidbody2D r = this.cmp_rigidbody;
        WaitForEndOfFrame eof = new WaitForEndOfFrame();

        float angleDistance;
        do
        {
            // Move towards this angle
            r.rotation = Mathf.MoveTowards(r.rotation, angle, turnSpeed * Time.deltaTime); 
            // Update angle distance
            angleDistance = Mathf.Abs(r.rotation - angle);

            yield return eof;
        }
        while( !Mathf.Approximately(angleDistance, 0) );
    }
}
