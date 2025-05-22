using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Main Player")]
    public string playerName;
    public int playerLevel = 1 ;
    public float playerHP = 100;
    public float maxHP = 100;    
    public float playerMP = 100;
    public float maxMP = 100;
    public float playerStamina = 100;
    public float maxStamina = 100;
    public float staminaRegenCooldown;
    public int playerXP = 0;
    public int XPToLevelUp = 100;
    public GameObject PlayerPrefab;

    public int Strength = 5;
    public int Intelligence = 5;
    public int Dexterity = 5;
    public int Vitality = 5;
    public int UnAllocatedStatPoints = 5;

    public Text strengthValue;
    public Text intelligenceValue;
    public Text dexterityValue;
    public Text vitalityValue;

    public Text HPString;
    public Text MPString;
    public Text STMString;
    public Text LVLString;
    public Text EXPString;

    public Text UnAllocatedStatPointsValue;

    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

    public PlayerController _playerController;

    [Header("Player Attributes")]
    public List<PlayerAttributes> PlayerAttributes = new List<PlayerAttributes>();

    [Header("Player Skills Equipped")]
    public List<Skill> PlayerPhysicalSkills = new List<Skill>(5);
    public List<Skill> PlayerMagicalSkills = new List<Skill>(5);
    public List<Skill> PlayerAbilitySkills = new List<Skill>(2);

    [Header("Player Weapons Equipped")]
    public List<Weapon> PlayerWeaponsLeft = new List<Weapon>(3);
    public List<Weapon> PlayerWeaponsRight = new List<Weapon>(3);

    [Header("Player Skill HUD Indexes")]
    public int currentPhysicalIndex = 0;
    public int currentMagicIndex = 0;
    public int currentAbilityIndex = 0;

    [Header("Player Weapon Indexes")]
    public int currentWeaponLeft = 0;
    public int currentWeaponRight = 0;

    public PLAYERSTATUS PlayerStatus;

    public GameObject _spawnPoint;

    public GameObject _pauseMenu;

    public GameObject expGainPrefab;
    public GameObject expGainPos;

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

        // Setting max values of sliders
        HUDManager.Instance._healthSlider.maxValue = maxHP;
        HUDManager.Instance._mpSlider.maxValue = maxMP;
        HUDManager.Instance._staminaSlider.maxValue = maxStamina;

        // initializing values of sliders
        HUDManager.Instance._healthSlider.value = playerHP;
        HUDManager.Instance._mpSlider.value = playerMP;
        HUDManager.Instance._staminaSlider.value = playerStamina;

        _playerController = GetComponent<PlayerController>();

        PlayerStatus = PLAYERSTATUS.ALIVE;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateAttributes();
    }
    public void OnEnable()
    {
        HUDManager.Instance.OnSkillButtonPressed += _playerController.ActivateSkill;
    }

    public void OnDisable()
    {
        HUDManager.Instance.OnSkillButtonPressed -= _playerController.ActivateSkill;
    }

    public void AddHealth(float health)
    {
        playerHP = Mathf.Clamp(playerHP + health, 0, maxHP);

        if (HUDManager.Instance._healthSlider.value != playerHP)
            HUDManager.Instance._healthSlider.value = playerHP;
    }

    public void AddStamina(float stamina)
    {
        playerStamina = Mathf.Clamp(playerStamina + stamina, 0, maxStamina);

        if (HUDManager.Instance._staminaSlider.value != playerStamina)
            HUDManager.Instance._staminaSlider.value = playerStamina;
    }

    public void AddMP(float mp)
    {
        playerMP = Mathf.Clamp(playerMP + mp, 0, maxMP);

        if (HUDManager.Instance._mpSlider.value != playerMP)
            HUDManager.Instance._mpSlider.value = playerMP;
    }

    public void UpdateHealth(float health)
    {
        playerHP = Mathf.Clamp(health, 0, maxHP);

        if (HUDManager.Instance._healthSlider.value != playerHP)
            HUDManager.Instance._healthSlider.value = playerHP;
    }

    public void UpdateStamina(float stamina)
    {
        playerStamina = Mathf.Clamp(stamina, 0, maxStamina);

        if (HUDManager.Instance._staminaSlider.value != playerStamina)
            HUDManager.Instance._staminaSlider.value = playerStamina;
    }

    public void UpdateMP(float mp)
    {
        playerMP = Mathf.Clamp(mp, 0, maxMP);

        if (HUDManager.Instance._mpSlider.value != playerMP)
            HUDManager.Instance._mpSlider.value = playerMP;
    }

    public void UpdateXP(int XP)
    {
        playerXP = XP;

        if (playerXP >= XPToLevelUp)
        {
            playerLevel++;
            float tempXPToLevelUp = XPToLevelUp + ( XPToLevelUp * 25 ) / 100;
            int tempXP = (int) Mathf.Round(tempXPToLevelUp / 10) % 10;
            XPToLevelUp = tempXP;   
        }
    }

    public void StaminaRegen(float regenValue)
    {
        playerStamina += regenValue * Time.deltaTime;
        playerStamina = Mathf.Clamp(playerStamina, 0, maxStamina);
        UpdateStamina(playerStamina);
    }

    public void ManaRegen(float regenValue)
    {
        playerMP += regenValue * Time.deltaTime;
        playerMP = Mathf.Clamp(playerMP, 0, maxMP);
        UpdateMP(playerMP);
    }

    public bool TrySpendMana(float amount)
    {
        if (playerMP < amount) return false;
        playerMP -= amount;
        UpdateMP(playerMP);
        return true;
    }

    public bool TrySpendStamina(float amount)
    {
        if (playerStamina < amount) return false;
        playerStamina-= amount;
        UpdateStamina(playerStamina);
        return true;
    }

    public void IncreaseStrength()
    {
        if (UnAllocatedStatPoints <= 0)
            return;

        Strength += 1;
        UnAllocatedStatPoints -= 1;

        strengthValue.text = Strength.ToString();
        UnAllocatedStatPointsValue.text = UnAllocatedStatPoints.ToString();
        UpdateAttributes();
    }

    public void IncreaseIntelligence()
    {
        if (UnAllocatedStatPoints <= 0)
            return;

        Intelligence += 1;
        UnAllocatedStatPoints -= 1;

        intelligenceValue.text = Intelligence.ToString();
        UnAllocatedStatPointsValue.text = UnAllocatedStatPoints.ToString();
        UpdateAttributes();
    }

    public void IncreaseDexterity()
    {
        if (UnAllocatedStatPoints <= 0)
            return;

        Dexterity += 1;
        UnAllocatedStatPoints -= 1;

        maxStamina = maxStamina + (1 + Dexterity * 0.005f);

        dexterityValue.text = Dexterity.ToString();
        UnAllocatedStatPointsValue.text = UnAllocatedStatPoints.ToString();
        UpdateAttributes();
    }

    public void IncreaseVitality()
    {
        if (UnAllocatedStatPoints <= 0)
            return;

        Vitality += 1;
        UnAllocatedStatPoints -= 1;

        maxHP = maxHP + (1 + Vitality * 0.005f);

        vitalityValue.text = Vitality.ToString();
        UnAllocatedStatPointsValue.text = UnAllocatedStatPoints.ToString();
        UpdateAttributes();
    }

    public void UpdateAttributes()
    {
        strengthValue.text = Strength.ToString();
        intelligenceValue.text = Intelligence.ToString();
        dexterityValue.text = Dexterity.ToString();
        vitalityValue.text = Vitality.ToString();

        HPString.text= maxHP.ToString();
        MPString.text = maxMP.ToString();
        STMString.text = maxStamina.ToString();
        LVLString.text = playerLevel.ToString();
        EXPString.text = playerXP.ToString() + "/" + XPToLevelUp.ToString();

        UnAllocatedStatPointsValue.text = UnAllocatedStatPoints.ToString();
    }

    public void GiveEXP(int EXP)
    {
        Instantiate(expGainPrefab, expGainPos.transform.position, Quaternion.identity, transform);

        int calculatedEXP = Mathf.Clamp(playerXP + EXP, 0, XPToLevelUp);

        if (calculatedEXP >= XPToLevelUp)
        {
            playerLevel += 1;
            UnAllocatedStatPoints += 5;
            XPToLevelUp = XPToLevelUp * 2;
            playerXP = 0;
        }
    }
}
