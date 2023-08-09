using UnityEngine;

public class WaterCollider : MonoBehaviour
{
    private MeshCollider waterCollider;

    private void Start()
    {
        // Add a Mesh Collider to the GameObject
        waterCollider = gameObject.AddComponent<MeshCollider>();
        waterCollider.sharedMesh = GetComponent<MeshFilter>().mesh;

        // Set the collider as convex if the water surface is not flat
        // waterCollider.convex = true; // Uncomment this line if needed
    }

    // This method can be used to respond to objects entering the water collider
    private void OnTriggerEnter(Collider other)
    {
        // Apply damage to the player
        IDamage damageable = other.GetComponent<IDamage>();
        if (damageable != null)
        {
            damageable.TakeDamage(1000);
        }
        
    }

    private void EmitFoamParticles()
    {
        
    }

    // This method can be used to respond to objects exiting the water collider
    private void OnTriggerExit(Collider other)
    {
        
    }
}