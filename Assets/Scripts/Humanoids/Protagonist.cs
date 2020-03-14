using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protagonist 
    : Humanoid
{
    [Header ("Capabilities")]
    [Tooltip("Indicates how much the protagonist is hungry.")]
    public int full = 100;
    public int ammunition = 10;

    private Bullet shootWeapon;
    public Vector3 ShootPoint
    {
        get { return this.transform.position + (Vector3)DirectionGuide.getDirection(this.faceDirection).ToVector2(); }
    }

    #region Built-In Methods

    void Start()
    {
        // Instantiate the shoot weapon but deactivate it
        shootWeapon = Instantiate(this.gameManager.subResource.FindResource<Bullet>("Bullet"), this.transform);
        shootWeapon.gameObject.SetActive(false);
        // Setup the controls & refresh
        this.gameManager.subInterface.healthBar.Setup(this.health);
        this.gameManager.subInterface.hungerBar.Setup(this.full);
        this.RefreshInterface();
    }

    void Update()
    {
        // Don't move the protagonist if he is currently moving
        if (!this.isMoving)
            // Computer Test
            if (!Application.isMobilePlatform)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    this.Controller(KeyCode.UpArrow);
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    this.Controller(KeyCode.RightArrow);
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                    this.Controller(KeyCode.DownArrow);
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    this.Controller(KeyCode.LeftArrow);
                else if (Input.GetKeyDown(KeyCode.Space))
                    this.Attack();
            }
            // Mobile Test
            else if (Application.isMobilePlatform)
            { }
    }

    #endregion

    #region Overridden Methods

    public override void PickAction(Collider2D c)
    {
        if (c != null)
            if (c.GetComponent<Pickable>() != null)
            {
                if (!c.GetComponent<Pickable>().isWrapped)
                {
                    // Refresh vfx {Must be removed from here}
                    ParticleSystem p = this.gameManager.subResource.GetPreMadeResource<ParticleSystem>("Refresh");
                    ResourceManager.PlayVFX(p, this.Position.ToVector2());
                    // Pickable objects case
                    c.GetComponent<Pickable>().Use(this);
                }
            }
    }

    public override void AttackAction(Collider2D c)
    {
        if (c != null)
        {
            if (c.GetComponent<Pickable>() != null) // Pickable objects case
            {
                Pickable pick = c.GetComponent<Pickable>();
                if (pick.isWrapped)
                {
                    // Play the break vfx {This code Must be removed from here}
                    ParticleSystem p = this.gameManager.subResource.GetPreMadeResource<ParticleSystem>("Break");
                    ResourceManager.PlayVFX(p, pick.Cell.Position.ToVector2());
                    // Unwrap the box
                    c.GetComponent<Pickable>().Unwrap();
                }
            }
            else if (c.GetComponent<Antagonist>() != null)               // Antagonist case
                c.GetComponent<Antagonist>().TakeDamage(this.shootWeapon.damage);
        }
    }

    protected override IEnumerator IMove(IntVector2 direction, float transitionSpeed = 2.5f)
    {
        // Start the base implementation
        StartCoroutine(base.IMove(direction, transitionSpeed));
        // The world shall resume when the protagonist quit moving
        while (isMoving)
        { yield return new WaitForEndOfFrame(); }
        // Resume now
        this.gameManager.Resume();
        // Update camera position
        this.gameManager.MoveCamera(this.Position.ToVector2());
    }

    public override void TakeDamage(int damage)
    {
        // Do the common operation
        base.TakeDamage(damage);
        // Update interface
        this.RefreshInterface();
    }

    public override void Die()
    {
        // Update interface
        this.RefreshInterface();
        // Die
        base.Die();
    }

    #endregion

    // Attack operation
    public void Attack()
    {
        if (this.ammunition == 0 || this.shootWeapon.gameObject.activeInHierarchy)
            return;
        // Play attack animation
        this.PlayAnimation("State_Attack");
        // Fire weapon
        this.shootWeapon.Launch(this, this.faceDirection);
        // A bullet is fired then we must be running out of ammunition
        this.ammunition--;
        // Update interface
        this.RefreshInterface();
        // The world must resume
        this.gameManager.Resume();
    }

    // Special Methods
    public void Profit(int health, int fill, int ammu)
    {
        // Play Reloading animation
        if (ammu != 0)
        {
            this.ResetAnimations();
            this.PlayAnimation("State_Reload");
        }

        this.health += health;
        this.full += fill;
        this.ammunition += ammu;
        // Line for updating health and full bars
        this.RefreshInterface();
    }

    // Control Methods
    private void Controller(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.UpArrow: 
                this.Move(Direction.Up);
                break;
            case KeyCode.RightArrow: 
                this.Move(Direction.Right);
                break;
            case KeyCode.DownArrow: 
                this.Move(Direction.Down);
                break;
            case KeyCode.LeftArrow: 
                this.Move(Direction.Left);
                break;
        }
    }

    // Interface method
    private void RefreshInterface()
    {
        this.gameManager.subInterface.healthBar.Refresh(this.health);
        this.gameManager.subInterface.hungerBar.Refresh(this.full);
        this.gameManager.subInterface.ammuText.text = this.ammunition.ToString();
    }
}
