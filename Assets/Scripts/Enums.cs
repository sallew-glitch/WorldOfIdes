using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SKILLTYPE
{
    PHYSICAL,
    MAGIC,
    ABILITY
}

public enum ABILITYSTATE
{ 
    READY,
    ACTIVE,
    COOLDOWN
}

public enum WEAPONTYPE
{
    LEFT,
    RIGHT
}

public enum PLAYERSTATUS
{
    ALIVE,
    DEAD
}

public enum EQUIPMENT
{
    HELMET,
    UPPER,
    LOWER
}

public interface IEffectBehavior
{
    void Initialize(Transform origin, SkillContext ctx);
    void Execute();
}

public interface IHitEffect
{
    void Apply(NPCController npc);
}

public class SkillContext
{
    public float damage;
    public float areaRadius;
    public LayerMask targetLayerMask;
    public float duration;
    public float velocity;
    public float travelDistance;
    public int travelTime;
    public Vector3 direction;
    public float range;
    public float healValue;
    public float damagePerSecond;
    public float tickInterval;
    public float freezeDuration;
    public NPCController npcController;
    public float damageMultiplier;
    public float movementMultiplier;
    public float healthMultiplier;
    public float MPMultiplier;
}

