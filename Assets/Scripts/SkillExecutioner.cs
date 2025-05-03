using UnityEngine;
using System.Collections.Generic;

public class SkillExecutioner : MonoBehaviour
{

    private void Cast<T>(List<T> arr, int i) where T : Skill
    {
        StartCoroutine(arr[i]?.Activate(transform));
    }

    public void CastMagicalSkill()
    {
        if (SkillManager.Instance.magicSkillState == ABILITYSTATE.READY)
        {
            int i = PlayerManager.Instance.currentMagicIndex;
            var skills = PlayerManager.Instance.PlayerMagicalSkills;

            if (i >= 0 && i < skills.Count && skills[i] != null)
            {                
                Cast(skills, i);
                SkillManager.Instance.magicSkillState = ABILITYSTATE.ACTIVE;
                SkillManager.Instance.activeTimeMagic = PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex].duration;
            }
        }
    }

    public void CastPhysicalSkill()
    {
        if (SkillManager.Instance.physicalSkillState == ABILITYSTATE.READY)
        {
            int i = PlayerManager.Instance.currentPhysicalIndex;
            var skills = PlayerManager.Instance.PlayerPhysicalSkills;

            if (i >= 0 && i < skills.Count && skills[i] != null)
            {
                Cast(skills, i);
                SkillManager.Instance.physicalSkillState = ABILITYSTATE.ACTIVE;
                SkillManager.Instance.activeTimePhysical = PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex].duration;
            }
        }
    }

    public void CastAbilitySkill()
    {
        if (SkillManager.Instance.abilitySkillState == ABILITYSTATE.READY)
        {
            int i = PlayerManager.Instance.currentAbilityIndex;
            var skills = PlayerManager.Instance.PlayerAbilitySkills;

            if (i >= 0 && i < skills.Count && skills[i] != null)
            {
                Cast(skills, i);
                SkillManager.Instance.abilitySkillState = ABILITYSTATE.ACTIVE;
                SkillManager.Instance.activeTimeAbility = PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex].duration;

            }
        }
    }
}
