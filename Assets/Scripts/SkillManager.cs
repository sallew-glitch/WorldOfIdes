using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private static SkillManager _instance;
    public static SkillManager Instance { get { return _instance; } }

    [HideInInspector] public ABILITYSTATE physicalSkillState = ABILITYSTATE.READY;
    [HideInInspector] public ABILITYSTATE magicSkillState = ABILITYSTATE.READY;
    [HideInInspector] public ABILITYSTATE abilitySkillState = ABILITYSTATE.READY;

    [HideInInspector] public float activeTimePhysical;
    [HideInInspector] public float activeTimeMagic;
    [HideInInspector] public float activeTimeAbility;
    [HideInInspector] public float cooldownTimePhysical;
    [HideInInspector] public float cooldownTimeMagic;
    [HideInInspector] public float cooldownTimeAbility;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (PlayerManager.Instance.PlayerStatus == PLAYERSTATUS.ALIVE)
        {
            MonitorSkillState();
        }
    }

    public void CycleSkill(SKILLTYPE skillType)
    {
        switch (skillType)
        {
            case SKILLTYPE.PHYSICAL:
                PlayerManager.Instance.currentPhysicalIndex = (PlayerManager.Instance.currentPhysicalIndex + 1) % PlayerManager.Instance.PlayerPhysicalSkills.Count;
                HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.PHYSICAL, PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex]);
                break;
            case SKILLTYPE.MAGIC:
                PlayerManager.Instance.currentMagicIndex = (PlayerManager.Instance.currentMagicIndex + 1) % PlayerManager.Instance.PlayerMagicalSkills.Count;
                HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.MAGIC, PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex]);
                break;
            case SKILLTYPE.ABILITY:
                PlayerManager.Instance.currentAbilityIndex = (PlayerManager.Instance.currentAbilityIndex + 1) % PlayerManager.Instance.PlayerAbilitySkills.Count;
                HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.ABILITY, PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex]);
                break;
        }
    }


    public void MonitorSkillState()
    {
        MonitorPhysicalSkillState();
        MonitorMagicalSkillState();
        MonitorAbilitySkillState();
    }

    public void MonitorPhysicalSkillState()
    {
        switch (physicalSkillState)
        {
            case ABILITYSTATE.READY:
                break;
            case ABILITYSTATE.ACTIVE:
                if (activeTimePhysical > 0)
                    activeTimePhysical -= Time.deltaTime;
                else
                {                    
                    physicalSkillState = ABILITYSTATE.COOLDOWN;
                    cooldownTimePhysical = PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex].cooldown;
                    HUDManager.Instance.SkillHUDCooldown(SKILLTYPE.PHYSICAL, PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex], cooldownTimePhysical);
                }
                break;
            case ABILITYSTATE.COOLDOWN:
                if (cooldownTimePhysical > 0)
                    cooldownTimePhysical -= Time.deltaTime;
                else
                {
                    HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.PHYSICAL, PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex]);
                    physicalSkillState = ABILITYSTATE.READY;
                }
                break;
        }
    }

    public void MonitorMagicalSkillState()
    {
        switch (magicSkillState)
        {
            case ABILITYSTATE.READY:
                break;
            case ABILITYSTATE.ACTIVE:
                if (activeTimeMagic > 0)
                    activeTimeMagic -= Time.deltaTime;
                else
                {
                    //PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex].Cooldown(transform);
                    magicSkillState = ABILITYSTATE.COOLDOWN;
                    cooldownTimeMagic = PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex].cooldown;
                    HUDManager.Instance.SkillHUDCooldown(SKILLTYPE.MAGIC, PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex], cooldownTimeMagic);
                }
                break;
            case ABILITYSTATE.COOLDOWN:
                if (cooldownTimeMagic > 0)
                    cooldownTimeMagic -= Time.deltaTime;
                else
                {
                    HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.MAGIC, PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex]);
                    magicSkillState = ABILITYSTATE.READY;
                }
                break;
        }
    }

    public void MonitorAbilitySkillState()
    {
        switch (abilitySkillState)
        {
            case ABILITYSTATE.READY:
                break;
            case ABILITYSTATE.ACTIVE:
                if (abilitySkillState > 0)
                    activeTimeAbility -= Time.deltaTime;
                else
                {
                    //PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex].Cooldown(transform);
                    abilitySkillState = ABILITYSTATE.COOLDOWN;
                    cooldownTimeAbility = PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex].cooldown;
                    HUDManager.Instance.SkillHUDCooldown(SKILLTYPE.ABILITY, PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex], cooldownTimeAbility);
                }
                break;
            case ABILITYSTATE.COOLDOWN:
                if (cooldownTimeAbility > 0)
                    cooldownTimeAbility -= Time.deltaTime;
                else
                {
                    HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.ABILITY, PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex]);
                    abilitySkillState = ABILITYSTATE.READY;
                }
                break;
        }
    }
}
