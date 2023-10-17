using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BreakableTile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float breakDelay = 0.5f; // Delay after which the tile breaks

    private Rigidbody2D rb;
    private BoxCollider2D[] colliders;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();

        // At the start, the Rigidbody2D should not affect physics simulation
        //rb.isKinematic = true;
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
        //rb.isKinematic = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Disable all box colliders
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
