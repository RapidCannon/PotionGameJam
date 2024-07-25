using UnityEngine;

public class PotionPour : MonoBehaviour
{
    // Particle system for pouring effect
    public ParticleSystem pourParticleSystem;

    // Serialized field for the potion's color
    [SerializeField] private Color potionColor;

    private void Start()
    {
        // Ensure the particle system is assigned
        if (pourParticleSystem == null)
        {
            Debug.LogError("Particle system not assigned!");
            return;
        }

        // Set the color of the particles
        var mainModule = pourParticleSystem.main;
        mainModule.startColor = potionColor;
    }
}
