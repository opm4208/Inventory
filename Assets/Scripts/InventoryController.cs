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
    public DBInventory dbInventory;
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
        GetItemCheck(Items[UnityEngine.Random.Range(0,3)]);
    }
 
    // DB에서 아이템정보를 받아 아이템 생성 후 배열에 저장
    public void InventoryRoad(int posX, int posY, int itemData, int itemIndex)
    {
        InventoryItem item = CreateItem(posY, posX, Items[itemData]);
        item.itemIndex = itemIndex;
        SaveItem(posY, posX, item);
    }

    // 아이템을 얻을시 인벤토리에 아이템이 들어갈 자리가 있는지 확인 후 자리가 있다면 아이템을 저장
    private bool GetItemCheck(ItemData item)
    {
        // 세로
        for (int  i = 0; i < inventoryItemSlot.GetLength(0); i++)
        {
            // 가로
            for (int j = 0; j<inventoryItemSlot.GetLength(1); j++) 
            {
                if (inventoryItemSlot[i, j] == null)
                {
                    if (EmptyCheck(j, i, item.width, item.height))
                    {
                        InventoryItem inventoryItem = CreateItem(i, j, item);
                        SaveItem(i, j, inventoryItem);
                        dbInventory.DBSave(j, i, item.itemID, inventoryItem.itemIndex);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private bool EmptyCheck(int width, int height, int itemWidth, int itemHeight)
    {
        // 인벤토리 범위를 넘는 인덱스를 탐색하는걸 막기위함
        if (width + itemWidth > inventoryItemSlot.GetLength(1) || height + itemHeight > inventoryItemSlot.GetLength(0))
            return false;
        // 아이템의 크기만큼 탐색하여 null이 아니면 실패를 반환
        for (int i = 0; i < itemHeight; i++)
        {
            for (int j = 0; j < itemWidth; j++)
            {
                if (inventoryItemSlot[height + i, width + j] != null)
                    return false;
            }
        }
        return true;
    }

    // 아이템 생성시 초기화 작업
    private InventoryItem CreateItem(int i, int j, ItemData item)
    {
        RectTransform rectTransform = Instantiate(itemPrefab).GetComponent<RectTransform>();
        rectTransform.SetParent(transform);
        rectTransform.GetComponent<InventoryItem>().Set(item, transform.childCount - 1, j, i);

        return rectTransform.GetComponent<InventoryItem>();
    }

    private void DeleteItem(int itemIndex)
    {
        // db에서 해당 아이템 삭제
        dbInventory.DBItemDelete(itemIndex);
        // 해당 아이템 배열에서 null로 변경
        InventoryItem item = itemTransform.GetComponent<InventoryItem>();
        DeleteItemArray(item.onGridPositionY, item.onGridPositionX, item);
        // 해당 아이템 게임 오브젝트 삭제
        Destroy(item.gameObject);
        // 남은 아이템 index 재설정
        StartCoroutine(ReIndex());
    }

    IEnumerator ReIndex()
    {
        yield return new WaitForSeconds(0.1f);
        int index = 0;
        foreach (InventoryItem item in transform.GetComponentsInChildren<InventoryItem>())
        {
            if (item.itemIndex != index)
            {
                dbInventory.DBIndexReSet(item.itemIndex, index);
                item.itemIndex = index;
            }
            index++;
        }
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
            }
        }
    }

    // 아이템의 위치를 변경한다.
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
        // 아이템을 선택하지 않았을시
        if (!itemDrag)
        {
            Debug.Log("X"+tileGridPosition.x);
            Debug.Log("Y" + tileGridPosition.y);
            // 인벤토리 범위 밖을 클릭하면 return
            if (tileGridPosition.x >= inventoryItemSlot.GetLength(1) || tileGridPosition.x < 0 || tileGridPosition.y >= inventoryItemSlot.GetLength(0) || tileGridPosition.y < 0)
                return;
            // 만약 마우스가 가리키는 공간에 아이템이 있을시
            if (inventoryItemSlot[tileGridPosition.y, tileGridPosition.x] != null)
            {
                // 이동시킬 아이템의 트랜스펌을 저장
                moveAbleHeaderUI.itemDrag = true;
                itemTransform = inventoryItemSlot[tileGridPosition.y, tileGridPosition.x].GetComponent<RectTransform>();
                InventoryItem inventoryItem = itemTransform.GetComponent<InventoryItem>();
                DeleteItemArray(inventoryItem.onGridPositionY, inventoryItem.onGridPositionX, inventoryItem);
                itemDrag = true;
            }
        }
        // 아이템을 선택했을시
        else
        {
            // 인벤토리 범위 밖을 클릭시 아이템 삭제
            if (tileGridPosition.x >= inventoryItemSlot.GetLength(1) || tileGridPosition.x < 0 || tileGridPosition.y >= inventoryItemSlot.GetLength(0) || tileGridPosition.y < 0)
            {
                DeleteItem(itemTransform.GetComponent<InventoryItem>().itemIndex);
                moveAbleHeaderUI.itemDrag = false;
                itemTransform = null;
                itemDrag = false;
            }
            else
            {
                ItemData itemData = itemTransform.GetComponent<InventoryItem>().itemData;
                // 클릭한 공간이 아이템이 없으면 그 위치로 이동
                if (EmptyCheck(tileGridPosition.x, tileGridPosition.y, itemData.width, itemData.height))
                {
                    SaveItem(tileGridPosition.y, tileGridPosition.x, itemTransform.GetComponent<InventoryItem>());
                    dbInventory.DBPositionReSet(tileGridPosition.x, tileGridPosition.y, itemTransform.GetComponent<InventoryItem>().itemIndex);
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
