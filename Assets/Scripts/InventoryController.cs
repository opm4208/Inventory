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
        // 인벤토리 범위를 넘는 인덱스를 탐색하는걸 막기위함
        if(width + itemwidth > inventoryItemSlot.GetLength(1) || height + itemheight > inventoryItemSlot.GetLength(0))
            return false;
        // 아이템의 크기만큼 탐색하여 null이 아니면 실패를 반환
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

    // 아이템 생성시 초기화 작업
    private InventoryItem CreateItem(int i, int j, InventoryItem item)
    {
        RectTransform rectTransform = Instantiate(itemPrefab).GetComponent<RectTransform>();
        rectTransform.SetParent(transform);
        rectTransform.GetComponent<InventoryItem>().Set(item.itemData, j, i);

        return rectTransform.GetComponent<InventoryItem>();
    }

    private void SaveItem(int i, int j, InventoryItem item)
    {
        //아이템 위치 변경
        PlaceItem(i, j, item);

        // 인벤토리 배열에 아이템 저장
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

    // 아이템이 있던 배열의 데이터를 제거
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

        // 마우스의 좌표가 인벤토리 안에 있으면 작동
        if (tileGridPosition.x >= inventoryItemSlot.GetLength(1) || tileGridPosition.y >= inventoryItemSlot.GetLength(0))
            return;
        // 만약 마우스가 가리키는 공간에 아이템이 있을시
        if (inventoryItemSlot[tileGridPosition.y, tileGridPosition.x] != null)
        {
            // 만약 아이템을 이동시키고 있지 않으면
            if (!itemDrag)
            {
                // 이동시킬 아이템의 트랜스펌을 저장
                moveAbleHeaderUI.itemDrag=true;
                itemTransform = inventoryItemSlot[tileGridPosition.y, tileGridPosition.x].GetComponent<RectTransform>();
                InventoryItem inventoryItem = itemTransform.GetComponent<InventoryItem>();
                DeleteItemArray(inventoryItem.onGridPositionY, inventoryItem.onGridPositionX, inventoryItem);
                itemDrag = true;
            }
        }
        else
        {
            // 만약 아이템을 이동시키고 있으면
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

    // 마우스의 좌표를 인벤토리 칸에 맞게 계산
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
