using UnityEngine;

public interface IEffectBehavior
{
    void Initialize(Transform origin, SkillContext ctx);
    void Execute();
}

public interface IHitEffect
{
    void Apply(Enemy npc);
}
