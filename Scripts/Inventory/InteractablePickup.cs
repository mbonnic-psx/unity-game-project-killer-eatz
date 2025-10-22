using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePickup : MonoBehaviour
{
    public string itemName;
    public string itemDescription;

    public void Pickup(InventoryCarousel carousel)
    {
        if (carousel != null)
        {
            transform.SetParent(carousel.transform);
            carousel.AddItem(transform, itemName, itemDescription);

            transform.localPosition = Vector3.zero; // Will be adjusted by ArrangeItemsInCircle
            transform.localRotation = Quaternion.identity;

            //Debug.Log($"Picked up: {itemName}");
        }

    }
}
