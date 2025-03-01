using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    public Item _item;

    public Button _removeButton;
    public void RemoveItem()
    {
        InventoryManager.Instance.Remove(_item);

        Destroy(gameObject);
    }    

    public void AddItem(Item newItem)
    {
        _item = newItem;
    }

    public void UseItem()
    {
        _item.UseItem();
        RemoveItem();
    }

}
