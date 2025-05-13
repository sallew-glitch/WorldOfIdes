using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will move the object over time and explode on contact
/// </summary>
public class MoveOverTime : MonoBehaviour
{
    public Vector3 travelDirection;     // Direction of travel
    public float speed = 1.0f;          // Speed of movement
    public float delay = 0.0f;          // Initial delay before movement starts
    public bool isMoving = false;       // True if moving

    [Header("Explosion Settings")]
    public GameObject explosionVFXPrefab;  // Assign this in Inspector
    public float damage = 15f;

    void Start()
    {
        Invoke("StartMoving", delay);
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            Debug.Log("Moving");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit the player");
            // Spawn explosion VFX
            if (explosionVFXPrefab != null)
            {
                Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
            }

            // Deal damage
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        /*
        else if (!other.isTrigger)
        {
            // Optional: explode on any other collision
            if (explosionVFXPrefab != null)
            {
                Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
        */
    }
}
