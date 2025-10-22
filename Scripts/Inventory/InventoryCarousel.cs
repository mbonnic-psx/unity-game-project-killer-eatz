using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public string itemDescription;
}

public static class InventoryMemory
{
    public static List<InventoryItemData> savedItems = new List<InventoryItemData>();
}

[System.Serializable]
public class ItemPrefabEntry
{
    public string itemName;
    public GameObject prefab;
}


public class InventoryCarousel : MonoBehaviour
{
    [Header("Item List")]
    public List<Transform> items; // Assign items in the inspector
    public List<string> itemNames;
    public List<string> itemDescriptions;

    [Header("Text Elements")]
    public TextMeshProUGUI itemCounterText;
    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemDescription;

    [Header("Preferences")]
    public Transform cameraTransform;
    public float radius = 5f;
    public float userRotateSpeed = 100f;

    [Header("Prefabs")]
    public List<ItemPrefabEntry> prefabList;
    private Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
    public Transform inventoryCarouselParent;

    public static InventoryCarousel Instance;
    private bool hasInit = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize prefab dictionary
        prefabDict = new Dictionary<string, GameObject>();
        foreach (var entry in prefabList)
        {
            if (!prefabDict.ContainsKey(entry.itemName))
            {
                prefabDict.Add(entry.itemName, entry.prefab);
            }
        }
    }


    void Start()
    {
        if (!hasInit)
        {
            hasInit = true;

            // Load saved items (and instantiate prefabs)
            foreach (var itemData in InventoryMemory.savedItems)
            {
                if (prefabDict.TryGetValue(itemData.itemName, out GameObject prefab))
                {
                    GameObject newItem = Instantiate(prefab);
                    AddItem(newItem.transform, itemData.itemName, itemData.itemDescription);
                }
                else
                {
                    Debug.LogWarning($"Missing prefab for item: {itemData.itemName}");
                }
            }

            ArrangeItemsInCircle();
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up * userRotateSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.down * userRotateSpeed * Time.deltaTime);
        }

        itemCounterText.text = "ITEMS: " + items.Count.ToString();
        UpdateClosestItemText();
        ArrangeItemsInCircle();
    }

    public void ArrangeItemsInCircle()
    {
        if (items.Count == 0) return;

        float angleStep = 360f / items.Count;
        for (int i = 0; i < items.Count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            items[i].localPosition = Vector3.zero;

            items[i].localPosition = new Vector3(x, 0, z); // Adjust Y if needed

            //Debug.Log($"Item {items[i].name} positioned at {items[i].localPosition}");

        }
    }

    private void UpdateClosestItemText()
    {
        float closestDistance = Mathf.Infinity;
        Transform closestItem = null;

        foreach (Transform item in items)
        {
            float distance = Vector3.Distance(cameraTransform.position, item.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        if (closestItem != null)
        {
            // Update the UI with the item's specific details
            int index = items.IndexOf(closestItem);
            itemTitle.text = itemNames[index];
            itemDescription.text = itemDescriptions[index];
        }
    }

    public void AddItem(Transform itemTransform, string itemName, string itemDescription)
    {
        itemTransform.SetParent(transform); // Parent the item to the carousel
        items.Add(itemTransform); // Add to the item list
        itemNames.Add(itemName); // Add to the name list
        itemDescriptions.Add(itemDescription); // Add to the description list
        itemTransform.localPosition = Vector3.zero; // Reset position
        itemTransform.localRotation = Quaternion.identity; // Reset rotation
        ArrangeItemsInCircle(); // Re-arrange items in the circle

        InventoryMemory.savedItems.Add(new InventoryItemData
        {
            itemName = itemName,
            itemDescription = itemDescription
        });

        // Inform the overworld inventory
        if (FindObjectOfType<InventoryOverworld>() != null)
        {
            InventoryOverworld overworld = FindObjectOfType<InventoryOverworld>();
            overworld.AddOverworldItem(itemName, itemDescription);
        }

        //Debug.Log($"Added item to carousel: {itemName}");
    }

    public void RemoveItem(GameObject item, Transform itemTransform, string itemName, string itemDescription)
    {
        Destroy(item);
        items.Remove(itemTransform);
        itemNames.Remove(itemName);
        itemDescriptions.Remove(itemDescription);
    }

    void OnDisable()
    {
        if (Application.isPlaying && !Application.isEditor)
            return;

        items.Clear();
        itemNames.Clear();
        itemDescriptions.Clear();
    }

    void OnEnable()
    {
        ArrangeItemsInCircle();
    }

    public bool HasItem(string searchItemName)
    {
        return itemNames.Contains(searchItemName);
    }

}