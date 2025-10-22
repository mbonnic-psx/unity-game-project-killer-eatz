using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOverworld : MonoBehaviour
{
    [Header("Item List")]
    public List<Transform> overworldItems; // List of items in the overworld inventory
    public List<string> itemNames;
    public List<string> itemDescriptions;

    [Header("References")]
    public Transform scrollableParent;
    public InventoryCarousel inventoryCarousel;

    private int currentIndex = -1; // Tracks the currently active item index
    private bool isHidden = false;

    void Start()
    {
        // Ensure all items are deactivated at the start
        foreach (Transform child in scrollableParent)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void AddOverworldItem(string itemName, string itemDescription)
    {
        foreach (Transform child in scrollableParent)
        {
            ItemID itemID = child.GetComponent<ItemID>();
            if (itemID != null && itemID.itemID == itemName && !overworldItems.Contains(child))
            {
                overworldItems.Add(child);
                itemNames.Add(itemName);
                itemDescriptions.Add(itemDescription);
                return;
            }
        }


        Debug.LogWarning($"Item {itemName} could not be added. No matching child found under scrollable parent.");
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // Scroll up
        {
            ScrollToPreviousItem();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // Scroll down
        {
            ScrollToNextItem();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleItemsVisibility();
        }
    }

    private void ScrollToNextItem()
    {
        if (overworldItems.Count == 0) return;

        // Deactivate the current item
        if (currentIndex >= 0)
        {
            overworldItems[currentIndex].gameObject.SetActive(false);
        }

        // Move to the next item, looping back to the first item if at the end
        currentIndex = (currentIndex + 1) % overworldItems.Count;

        // Activate the new current item
        overworldItems[currentIndex].gameObject.SetActive(true);
    }

    private void ScrollToPreviousItem()
    {
        if (overworldItems.Count == 0) return;

        // Deactivate the current item
        if (currentIndex >= 0)
        {
            overworldItems[currentIndex].gameObject.SetActive(false);
        }

        // Move to the previous item, looping back to the last item if at the beginning
        currentIndex = (currentIndex - 1 + overworldItems.Count) % overworldItems.Count;

        // Activate the new current item
        overworldItems[currentIndex].gameObject.SetActive(true);
    }

    public void ToggleItemsVisibility()
    {
        isHidden = !isHidden;

        // If hiding, deactivate all items
        if (isHidden)
        {
            foreach (Transform item in overworldItems)
            {
                item.gameObject.SetActive(false);
            }
        }
        else
        {
            // If unhiding, activate the current item (if valid)
            if (currentIndex >= 0 && currentIndex < overworldItems.Count)
            {
                overworldItems[currentIndex].gameObject.SetActive(true);
            }
        }
    }
}
