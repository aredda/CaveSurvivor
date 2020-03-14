using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet 
    : MonoBehaviour 
{
    private Protagonist shooter;

    private Rigidbody2D cmp_rigidbody;
    private Collider2D cmp_collider;

    [Header("Damage")]
    public int damage = 10;

    void Awake()
    {
        this.cmp_rigidbody = GetComponent<Rigidbody2D>();
        this.cmp_collider = GetComponent<Collider2D>();

        this.cmp_rigidbody.gravityScale = 0;

        this.cmp_collider.enabled = false;
        this.cmp_collider.isTrigger = true;
    }

    public void Launch(Protagonist shooter, Direction direction, float speed = 7.5f)
    {
        this.cmp_rigidbody.velocity = Vector2.zero;         // Reset the velocity
        this.shooter = shooter;
        this.transform.position = this.shooter.ShootPoint;  // Reset the position of the bullet

        this.gameObject.SetActive(true);                    // Show the weapon
        this.cmp_collider.enabled = true;                   // Activates collider

        this.cmp_rigidbody.velocity = DirectionGuide.getDirection(direction).ToVector2() * speed;
    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        this.shooter.AttackAction(hit);
        // Once it touches something, that's when the bullet's life ends
        this.cmp_collider.enabled = false;
        this.gameObject.SetActive(false);
    }
}
