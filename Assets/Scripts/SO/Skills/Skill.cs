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

    public List<PlayerAttributes>  AffectedAttributes = new List<PlayerAttributes>();

    public abstract void Activate(Transform origin);    
    public abstract void Cooldown(Transform origin);

}
