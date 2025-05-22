using UnityEngine;

public class Loadout : MonoBehaviour
{
    [System.Serializable]
    public class LoadoutSlot
    {
        public string itemName;
        public GameObject itemPrefab; // The actual item GameObject
        public KeyCode keyCode;
    }

    public LoadoutSlot[] loadoutSlots = new LoadoutSlot[4];
    private int currentSlotIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Deactivate all items at start
        foreach (var slot in loadoutSlots)
        {
            if (slot.itemPrefab != null)
                slot.itemPrefab.SetActive(false);
        }

        EquipItem(0);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < loadoutSlots.Length; i++)
        {
            if (Input.GetKeyDown(loadoutSlots[i].keyCode))
            {
                EquipItem(i);
                break;
            }
        }
    }

    public void EquipItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= loadoutSlots.Length)
            return;

        // Unequip current item
        if (currentSlotIndex >= 0 && loadoutSlots[currentSlotIndex].itemPrefab != null)
        {
            loadoutSlots[currentSlotIndex].itemPrefab.SetActive(false);
        }

        // Equip new item
        var newSlot = loadoutSlots[slotIndex];
        if (newSlot.itemPrefab != null)
        {
            newSlot.itemPrefab.SetActive(true);
            Debug.Log("Equipped: " + newSlot.itemName);
            currentSlotIndex = slotIndex;
        }
        else
        {
            Debug.LogWarning("No item assigned to this slot.");
        }
    }

    public GameObject GetEquippedItem()
    {
        if (currentSlotIndex >= 0 && currentSlotIndex < loadoutSlots.Length)
        {
            return loadoutSlots[currentSlotIndex].itemPrefab;
        }
        return null;
    }
}
