using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public ItemData[] Items;
    private int girdSizeWidth;
    private int girdSizeHeight;
    public bool itemDrag = false;

    RectTransform rectTransform;
    RectTransform itemTransform;

    MoveAbleHeaderUI moveAbleHeaderUI;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        moveAbleHeaderUI = GetComponentInParent<MoveAbleHeaderUI>();
    }
    private void Start()
    {
        girdSizeWidth = (int)(rectTransform.rect.width/tileSizeWidth);
        girdSizeHeight = (int)(rectTransform.rect.height/tileSizeHeight);
        inventoryItemSlot = new InventoryItem[girdSizeHeight, girdSizeWidth];
    }
    private void Update()
    {
        ItemIconDrag();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateRandomItem();
        }

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }
    }

    private void CreateRandomItem()
    {
        GetItemCheck(itemPrefab.GetComponent<InventoryItem>());
    }
 
    public void InventoryRoad(int posX, int posY, int itemData, int itemIndex)
    {

    }

    // �������� ������ �κ��丮�� �������� �� �ڸ��� �ִ��� Ȯ�� �� �ڸ��� �ִٸ� �������� ����
    private bool GetItemCheck(InventoryItem item)
    {
        // ����
        for(int  i = 0; i < inventoryItemSlot.GetLength(0); i++)
        {
            // ����
            for(int j = 0; j<inventoryItemSlot.GetLength(1); j++) 
            {
                if (inventoryItemSlot[i, j] == null)
                {
                    if (EmptyCheck(j, i, item.itemData.width, item.itemData.height))
                    {
                        SaveItem(i, j, CreateItem(i, j, item));
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private bool EmptyCheck(int width, int height, int itemwidth, int itemheight)
    {
        // �κ��丮 ������ �Ѵ� �ε����� Ž���ϴ°� ��������
        if(width + itemwidth > inventoryItemSlot.GetLength(1) || height + itemheight > inventoryItemSlot.GetLength(0))
            return false;
        // �������� ũ�⸸ŭ Ž���Ͽ� null�� �ƴϸ� ���и� ��ȯ
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

    // ������ ������ �ʱ�ȭ �۾�
    private InventoryItem CreateItem(int i, int j, InventoryItem item)
    {
        RectTransform rectTransform = Instantiate(itemPrefab).GetComponent<RectTransform>();
        rectTransform.SetParent(transform);
        rectTransform.GetComponent<InventoryItem>().Set(item.itemData, j, i);

        return rectTransform.GetComponent<InventoryItem>();
    }

    private void SaveItem(int i, int j, InventoryItem item)
    {
        //������ ��ġ ����
        PlaceItem(i, j, item);

        // �κ��丮 �迭�� ������ ����
        for (int itemheight = 0; itemheight < item.itemData.height; itemheight++)
        {
            for (int itemwidth = 0; itemwidth < item.itemData.width; itemwidth++)
            {
                inventoryItemSlot[i + itemheight, j + itemwidth] = item;
                Debug.Log(inventoryItemSlot[i + itemheight, j + itemwidth]);
            }
        }
    }

    private void PlaceItem(int i, int j, InventoryItem item)
    {
        Vector2 position = new Vector2();
        position.x = j * tileSizeWidth + tileSizeWidth * item.itemData.width / 2;
        position.y = -(i * tileSizeHeight + tileSizeHeight * item.itemData.height / 2);
        item.onGridPositionX = j;
        item.onGridPositionY = i;
        item.GetComponent<RectTransform>().localPosition = position;
    }

    // �������� �ִ� �迭�� �����͸� ����
    private void DeleteItemArray(int i, int j, InventoryItem item)
    {
        for (int itemheight = 0; itemheight < item.itemData.height; itemheight++)
        {
            for (int itemwidth = 0; itemwidth < item.itemData.width; itemwidth++)
            {
                inventoryItemSlot[i + itemheight, j + itemwidth] = null;

            }
        }
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition(Input.mousePosition);

        // ���콺�� ��ǥ�� �κ��丮 �ȿ� ������ �۵�
        if (tileGridPosition.x >= inventoryItemSlot.GetLength(1) || tileGridPosition.y >= inventoryItemSlot.GetLength(0))
            return;
        // ���� ���콺�� ����Ű�� ������ �������� ������
        if (inventoryItemSlot[tileGridPosition.y, tileGridPosition.x] != null)
        {
            // ���� �������� �̵���Ű�� ���� ������
            if (!itemDrag)
            {
                // �̵���ų �������� Ʈ�������� ����
                moveAbleHeaderUI.itemDrag=true;
                itemTransform = inventoryItemSlot[tileGridPosition.y, tileGridPosition.x].GetComponent<RectTransform>();
                InventoryItem inventoryItem = itemTransform.GetComponent<InventoryItem>();
                DeleteItemArray(inventoryItem.onGridPositionY, inventoryItem.onGridPositionX, inventoryItem);
                itemDrag = true;
            }
        }
        else
        {
            // ���� �������� �̵���Ű�� ������
            if (itemDrag)
            {
                ItemData itemData = itemTransform.GetComponent<InventoryItem>().itemData;
                if (EmptyCheck(tileGridPosition.x, tileGridPosition.y, itemData.width, itemData.height))
                {
                    SaveItem(tileGridPosition.y, tileGridPosition.x, itemTransform.GetComponent<InventoryItem>());
                    moveAbleHeaderUI.itemDrag = false;
                    itemTransform = null;
                    itemDrag = false;
                }
            }
        }
    }

    // ���콺�� ��ǥ�� �κ��丮 ĭ�� �°� ���
    private Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        Vector2Int tileGridPosition = new Vector2Int();

        tileGridPosition.x =(int)(mousePosition.x - rectTransform.position.x);
        tileGridPosition.y = (int)(rectTransform.position.y - mousePosition.y);
        tileGridPosition.x = (int)(tileGridPosition.x / tileSizeWidth);
        tileGridPosition.y = (int)(tileGridPosition.y / tileSizeHeight);

        return tileGridPosition;
    }

    private void ItemIconDrag()
    {
        if (itemDrag)
        {
            itemTransform.position = Input.mousePosition;
        }
    }
}
