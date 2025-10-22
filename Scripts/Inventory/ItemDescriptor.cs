using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ItemDescriptor : MonoBehaviour
{
    public string itemName;
    public string itemDescription;

    public string getName()
    {
        return itemName;
    }

    public string getDescription()
    {
        return itemDescription;
    }
}
