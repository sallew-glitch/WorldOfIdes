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

public interface IState
{
    public void Tick();
    public void OnEnter();
    public void OnExit();
}

public interface IInteractableNFC
{
    public void OnEnterInteraction();
    public void OnExitInteraction();
}
