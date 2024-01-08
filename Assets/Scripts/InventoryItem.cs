using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public int onGridPositionX;
    public int onGridPositionY;
    public int itemIndex;

    internal void Set(ItemData itemData, int x, int y)
    {
        this.itemData = itemData;
        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = itemData.width * InventoryController.tileSizeWidth;
        size.y = itemData.height * InventoryController.tileSizeHeight;
        onGridPositionX = x;
        onGridPositionY = y;
        GetComponent<RectTransform>().sizeDelta = size;
    }
}
