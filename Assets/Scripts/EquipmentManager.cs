using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentManger : MonoBehaviour
{
    private static EquipmentManger _instance;
    public static EquipmentManger Instance { get { return _instance; } }

    public Transform _equipmentContent;
    public Transform _physicalSkillContent;
    public Transform _magicalSkillContent;
    public Transform _abilitySkillContent;
    public GameObject _itemUI;

    private enum EQUIPMENT{
        HELMET,
        UPPER,
        LOWER
    }
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

    public void ListEquipmentAndSkills()
    {
        foreach (Transform item in _physicalSkillContent)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in _magicalSkillContent)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in _abilitySkillContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var skill in PlayerManager.Instance.PlayerPhysicalSkills)
        {
            GameObject obj = Instantiate(_itemUI, _physicalSkillContent);            
            var itemIcon = obj.transform.Find("ItemImage").GetComponent<Image>();            
            
            itemIcon.sprite = skill.Icon;
        }

        foreach (var skill in PlayerManager.Instance.PlayerMagicalSkills)
        {
            GameObject obj = Instantiate(_itemUI, _magicalSkillContent);
            var itemIcon = obj.transform.Find("ItemImage").GetComponent<Image>();

            itemIcon.sprite = skill.Icon;
        }
        foreach (var skill in PlayerManager.Instance.PlayerAbilitySkills)
        {
            GameObject obj = Instantiate(_itemUI, _abilitySkillContent);
            var itemIcon = obj.transform.Find("ItemImage").GetComponent<Image>();

            itemIcon.sprite = skill.Icon;
        }
    }

}
