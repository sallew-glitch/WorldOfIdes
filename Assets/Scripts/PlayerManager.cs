using System.Collections.Generic;
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
    public int playerXP = 0;
    public int XPToLevelUp = 100;
    public GameObject PlayerPrefab;

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

    public void OnEnable()
    {
        HUDManager.Instance.OnSkillButtonPressed += _playerController.ActivateSkill;
    }

    public void OnDisable()
    {
        
    }

    public void UpdateHealth(float health)
    {
        playerHP = health;

        if (HUDManager.Instance._healthSlider.value != playerHP)
            HUDManager.Instance._healthSlider.value = playerHP;
    }

    public void UpdateStamina(float stamina)
    {
        playerStamina = stamina;

        if (HUDManager.Instance._staminaSlider.value != playerStamina)
            HUDManager.Instance._staminaSlider.value = playerStamina;
    }

    public void UpdateMP(float mp)
    {
        playerMP = mp;

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

}
