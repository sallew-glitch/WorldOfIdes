using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;
    public static InventoryManager Instance { get { return _instance; } }
    public List<Item> items = new List<Item> ();

    public Transform _itemContent;
    public GameObject _itemUI;
    public Toggle _enableRemove;

    public InventoryItemController[] inventoryItems;
    private void Awake()
    {
        if (_instance != null && _instance != this )
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Add(Item item)
    {
        items.Add(item);

    }

    public void Remove(Item item)
    {
        items.Remove(item);
    }

    public void ListItems()
    {
        foreach(Transform item in _itemContent)
        {
            Destroy(item.gameObject);
        }

        foreach(var item in items)
        {
            GameObject obj = Instantiate(_itemUI, _itemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("ItemImage").GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();
            /*obj.transform.GetComponent<InventoryItemController>();*/

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;

            obj.GetComponent<InventoryItemController>()._item = item;

            if (_enableRemove.isOn)
                removeButton.gameObject.SetActive(true);
        }

        SetInventoryItems();
    }

    public void EnableItemsRemove()
    {
        if (_enableRemove.isOn)
        {
            foreach(Transform item in _itemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(true);
            }
        } else
        {
            foreach (Transform item in _itemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(false);
            }
        }
    }

    public void SetInventoryItems()
    {
        inventoryItems = _itemContent.GetComponentsInChildren<InventoryItemController>();

        for(int i = 0; i < items.Count; i++)
        {
            inventoryItems[i].AddItem(items[i]);
        }
    }
}
