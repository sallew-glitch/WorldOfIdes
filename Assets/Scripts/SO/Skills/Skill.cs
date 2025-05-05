using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string Name;    
    public string Description;
    public Sprite Icon;
    public int LevelNeeded;
    public int XPNeeded;
    public SKILLTYPE SkillType;
    public float duration;
    public float cooldown;

    public AudioClip soundEffect;
    [Range(0, 1)] public float soundVolume = 1f;
    [Range(-3, 3)] public float soundPitch = 1f;

    public List<PlayerAttributes>  AffectedAttributes = new List<PlayerAttributes>();

    public abstract IEnumerator Activate(Transform origin);    
    public abstract IEnumerator Cooldown(Transform origin);

}
