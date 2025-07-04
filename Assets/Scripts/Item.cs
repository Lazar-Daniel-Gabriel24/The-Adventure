using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;
    internal int quantity;

    public virtual void UseItem()
    {
        Debug.Log("Using item " + Name);
    }

    public virtual void PickUp()
    {
        Sprite itemIcon = GetComponent<SpriteRenderer>().sprite;
        if (ItemPickupUIController.Instance != null)
        {
            ItemPickupUIController.Instance.ShowItemPickup(Name, itemIcon);
        }
    }
}