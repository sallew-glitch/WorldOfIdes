using UnityEngine;
using System.Collections;

public class ProjectileMotion : MonoBehaviour, IEffectBehavior
{
    private float velocity;
    private float travelDistance;
    private int travelTime;
    private Vector3 direction;

    private float distanceTraveled;
    private float timeElapsed;

    bool _isInitialized = false;

    void Update()
    {
        if (!_isInitialized)
            return;

        float deltaMove = velocity * Time.deltaTime;
        transform.position += direction * deltaMove;
        distanceTraveled += deltaMove;
        timeElapsed += Time.deltaTime;

        if (distanceTraveled >= travelDistance || timeElapsed >= travelTime)
        {
            Destroy(gameObject);
        }
    }
    public void Initialize(Transform origin, SkillContext ctx)
    {
        transform.position = origin.position;
        velocity = ctx.velocity;
        travelDistance = ctx.travelDistance;
        travelTime = ctx.travelTime;
        direction = ctx.direction;

        _isInitialized = true;
    }

    public void Execute()
    {
        
    }
    
}
