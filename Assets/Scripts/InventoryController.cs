using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;

    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    InventoryItem[,] inventoryItemSlot;
    private int girdSizeWidth;
    private int girdSizeHeight;

    RectTransform rectTransform;

    InventoryItem selectedItem;
    InventoryItem overlapItem;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        girdSizeWidth = (int)(rectTransform.rect.width/tileSizeWidth);
        girdSizeHeight = (int)(rectTransform.rect.height/tileSizeHeight);
        inventoryItemSlot = new InventoryItem[girdSizeHeight, girdSizeWidth];
        Debug.Log(inventoryItemSlot.GetLength(0));
        Debug.Log(inventoryItemSlot.GetLength(1));
        Debug.Log(girdSizeWidth);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateRandomItem();
        }
    }

    private void CreateRandomItem()
    {
        GetItemCheck(itemPrefab.GetComponent<InventoryItem>());

    }

    // 아이템을 얻을시 인벤토리에 아이템이 들어갈 자리가 있는지 확인 후 자리가 있다면 아이템을 저장
    private bool GetItemCheck(InventoryItem item)
    {
        // 세로
        for(int  i = 0; i < inventoryItemSlot.GetLength(0); i++)
        {
            // 가로
            for(int j = 0; j<inventoryItemSlot.GetLength(1); j++) 
            {
                if (inventoryItemSlot[i, j] == null)
                {
                    if (EmptyCheck(j, i, item.itemData.width, item.itemData.height))
                    {
                        SaveItem(i, j, item);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private bool EmptyCheck(int width, int height, int itemwidth, int itemheight)
    {
        for (int i = 0; i < itemheight; i++)
        {
            for (int j = 0; j < itemwidth; j++)
            {
                if (inventoryItemSlot[height + i, width + j] != null)
                    return false;
            }
        }
        return true;
    }
    private void SaveItem(int i, int j, InventoryItem item)
    {
        RectTransform rectTransform = Instantiate(itemPrefab).GetComponent<RectTransform>();
        rectTransform.SetParent(this.transform);
        rectTransform.GetComponent<InventoryItem>().Set(item.itemData);
        Vector2 position = new Vector2();

        position.x = j * tileSizeWidth + tileSizeWidth * item.itemData.width / 2;
        position.y = -(i * tileSizeHeight + tileSizeHeight * item.itemData.height / 2);
        rectTransform.localPosition = position;

        for (int itemheight = 0; itemheight < item.itemData.height; itemheight++)
        {
            for (int itemwidth = 0; itemwidth < item.itemData.width; itemwidth++)
            {
                inventoryItemSlot[i + itemheight, j + itemwidth] = item;
            }
        }
    }
}
