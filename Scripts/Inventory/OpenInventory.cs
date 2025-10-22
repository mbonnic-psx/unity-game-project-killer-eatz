using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenInventory : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject inventoryCamera;
    public GameObject gameCamera;

    [Header("Reference")]
    public PlayerMovement player;

    private bool isInventoryOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        inventoryCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (isInventoryOpen)
        {
            // Open inventory
            inventoryCamera.SetActive(true);
            gameCamera.SetActive(false);
            player.EnableMovement(false);
            player.EnableRotation(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Close inventory
            inventoryCamera.SetActive(false);
            gameCamera.SetActive(true);
            player.EnableMovement(true);
            player.EnableRotation(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
