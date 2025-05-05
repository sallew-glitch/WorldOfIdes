using UnityEngine;
using System.Collections.Generic;

public class CompositeBehaviours : MonoBehaviour, IEffectBehavior
{
    private List<IEffectBehavior> _effects = new List<IEffectBehavior>();

    public void AddEffect(IEffectBehavior effect)
    {
        _effects.Add(effect);
    }

    public void Initialize(Transform origin, SkillContext ctx)
    {
        foreach (var effect in _effects)
            effect.Initialize(origin, ctx);
    }

    public void Execute()
    {
        foreach (var effect in _effects)
            effect.Execute();
    }
}

