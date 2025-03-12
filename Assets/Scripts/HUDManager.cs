using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    private static HUDManager _instance;
    public static HUDManager Instance { get { return _instance; } }

    public Button[] SkillButtons;

    public delegate void SkillButtonPressedEvent(int index);
    public event SkillButtonPressedEvent OnSkillButtonPressed;

    
    [Header("Player Status Sliders")]
    public Slider _healthSlider;
    public Slider _mpSlider;
    public Slider _staminaSlider;

    [Header("Skill Holder Icons")]
    [SerializeField ]private Image _physicalSkillIcon;
    [SerializeField] private Image _magicSkillIcon;
    [SerializeField] private Image _abilitySkillIcon;

    [Header("Skill Holder Text")]
    [SerializeField] private TextMeshProUGUI _physicalCooldownText;
    [SerializeField] private TextMeshProUGUI _magicCooldownText;
    [SerializeField] private TextMeshProUGUI _abilityCooldownText;

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

        for (int i = 0; i < SkillButtons.Length; i++)
        {
            int index = i;
            SkillButtons[i].onClick.AddListener(() => HandleSkillButtonPress(index));            
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject healthObj = GameObject.Find("HP");
        GameObject mpObj = GameObject.Find("MP");
        GameObject staminaObj = GameObject.Find("Stamina");

        if (healthObj != null)
            _healthSlider = healthObj.GetComponent<Slider>();
        else
            Debug.LogWarning("HealthSlider not found in scene: " + scene.name);

        if (mpObj != null)
            _mpSlider = mpObj.GetComponent<Slider>();
        else
            Debug.LogWarning("MPSlider not found in scene: " + scene.name);

        if (staminaObj != null)
            _staminaSlider = staminaObj.GetComponent<Slider>();
        else
            Debug.LogWarning("StaminaSlider not found in scene: " + scene.name);
    }

    public void Start()
    {

        if (PlayerManager.Instance.PlayerPhysicalSkills.Count != 0 && PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex])
        {
            _physicalSkillIcon.color = Color.white;
            _physicalSkillIcon.sprite = PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex].Icon;
        }
        else
            _physicalSkillIcon.color = Color.black;

        if (PlayerManager.Instance.PlayerMagicalSkills.Count != 0 && PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex])
        {
            _magicSkillIcon.color = Color.white;
            _magicSkillIcon.sprite = PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex].Icon;
        }
        else
            _magicSkillIcon.color = Color.black;

        if (PlayerManager.Instance.PlayerAbilitySkills.Count != 0 && PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex])
        {
            _abilitySkillIcon.color = Color.white;
            _abilitySkillIcon.sprite = PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex].Icon;
        }
        else
            _abilitySkillIcon.color = Color.black;
    }

    public void HandleSkillButtonPress(int index) => OnSkillButtonPressed?.Invoke(index);

    public void UpdateSkillHolderUI(SKILLTYPE skillType, Skill skill)
    {
        if (skill == null) return;

        switch (skillType)
        {
            case SKILLTYPE.PHYSICAL:
                _physicalSkillIcon.sprite = skill.Icon;
                break;
            case SKILLTYPE.MAGIC:
                _magicSkillIcon.sprite = skill.Icon;
                break;
            case SKILLTYPE.ABILITY:
                _abilitySkillIcon.sprite = skill.Icon;
                break;
        }
    }

    public void SkillHUDCooldown(SKILLTYPE skillType, Skill skill, float cooldown)
    {
        if (skill == null) return;      

        switch (skillType)
        {
            case SKILLTYPE.PHYSICAL:                
                StartCoroutine(ShowCooldown(_physicalSkillIcon, _physicalCooldownText, cooldown));
                break;
            case SKILLTYPE.MAGIC:
                StartCoroutine(ShowCooldown(_magicSkillIcon, _magicCooldownText, cooldown));
                break;
            case SKILLTYPE.ABILITY:          
                StartCoroutine(ShowCooldown(_abilitySkillIcon, _abilityCooldownText, cooldown));
                break;
        }

     
    }

    private IEnumerator ShowCooldown(Image skillIcon, TextMeshProUGUI cooldownText, float cooldown)
    {
        float elapsed = 0f;        
        skillIcon.color = Color.black;
        cooldownText.color = Color.white;
        cooldownText.gameObject.SetActive(true);

        while (elapsed < cooldown)
        {            
            float remainingTime = Mathf.Ceil(cooldown - elapsed);
            cooldownText.text = remainingTime.ToString();            

            Debug.Log("LOOPING");
            
            elapsed += Time.deltaTime;
            
            float alpha = elapsed / cooldown;
            skillIcon.color = new Color(1f, 1f, 1f, alpha);

            yield return null; 
        }
        
        skillIcon.color = Color.white;
        cooldownText.gameObject.SetActive(false);
    }
}
