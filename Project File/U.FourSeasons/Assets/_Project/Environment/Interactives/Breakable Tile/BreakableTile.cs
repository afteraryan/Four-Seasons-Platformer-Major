using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BreakableTile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float breakDelay = 0.5f; // Delay after which the tile breaks
    [SerializeField] private float fixDelay = 3.0f;   // Delay after which the tile is fixed back

    private Rigidbody2D rb;
    private BoxCollider2D[] colliders;
    private Vector3 originalPosition;  // Store the original position of the tile

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();
        originalPosition = transform.position;  // Store the original position

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Tile broke");
        // Check if the collider belongs to the PlayerCharacter layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerCharacter"))
        {
            Invoke("BreakTile", breakDelay);
        }
    }

    private void BreakTile()
    {
        // Make the tile fall
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Disable all box colliders
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        // Schedule the tile to be fixed after a delay
        Invoke("FixTile", fixDelay);
    }

    private void FixTile()
    {
        // Reset the tile's position and properties
        transform.position = originalPosition;
        rb.velocity = Vector2.zero;  // Stop any movement
        rb.angularVelocity = 0f;     // Stop any rotation
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Enable all box colliders
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }

        Debug.Log("Tile fixed");
    }
}