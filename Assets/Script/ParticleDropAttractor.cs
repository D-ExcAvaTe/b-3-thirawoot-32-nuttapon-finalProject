using System;
using Coffee.UIExtensions;
using UnityEngine;

public class ParticleDropAttractor : MonoBehaviour
{
    public static ParticleDropAttractor instance;

    [Header("Settings")]
    [SerializeField] private ParticleSystem lootParticleSystem;
    [SerializeField] private Transform target; // Drag your Player or UI icon here
    [SerializeField] private float attractSpeed = 10f;
    [SerializeField] private float collectDistance = 0.5f;

    [Header("Behavior")]
    [SerializeField] private int particlesPerEnemy = 1;
    
    // We reuse this array to save memory
    private ParticleSystem.Particle[] _particles;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (lootParticleSystem == null) 
            lootParticleSystem = GetComponent<ParticleSystem>();
            
        // Initialize array with max particles of the system
        _particles = new ParticleSystem.Particle[lootParticleSystem.main.maxParticles];
    }

    private void Update()
    {
        if (target == null) return;

        // 1. Get all active particles
        int count = lootParticleSystem.GetParticles(_particles);

        for (int i = 0; i < count; i++)
        {
            // 2. Move particle towards target
            // We use the remaining lifetime to check if it's "fresh" or ready to move
            // (Optional: You can add a small delay so they explode out first, then fly)
            
            Vector3 direction = (target.position - _particles[i].position).normalized;
            
            // Move the particle
            _particles[i].position += direction * attractSpeed * Time.deltaTime;

            // 3. Check distance
            float distance = Vector3.Distance(_particles[i].position, target.position);
            
            if (distance < collectDistance)
            {
                // "Kill" the particle
                _particles[i].remainingLifetime = -1f;
                
                // TRIGGER YOUR LOGIC HERE (Add XP, Add Money, etc.)
                OnParticleCollected();
            }
        }

        // 4. Apply changes back to the system
        lootParticleSystem.SetParticles(_particles, count);
    }

    public void SpawnLoot(Vector3 position)
    {
        // EmitParams allows us to spawn a particle at a specific world position
        // even if the ParticleSystem object is somewhere else.
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.position = position;
        
        // You can also randomize color or size here
        // emitParams.startColor = Color.yellow;

        lootParticleSystem.Emit(emitParams, particlesPerEnemy);
    }

    private void OnParticleCollected()
    {
        // Example: Add XP to player
        // Player.instance.AddExp(10);
        Debug.Log("Loot Collected!");
    }
}