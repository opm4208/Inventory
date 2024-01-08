using JetBrains.Annotations;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using UnityEngine;

public class DBInventory : MonoBehaviour
{
    public static MySqlConnection conn;
    string conStr = string.Format("Server={0};Database={1};Uid ={2};Pwd={3};",
            "127.0.0.1", "test", "root", "qkrals123#");
    private void Start()
    {
        conn = new MySqlConnection(conStr);
    }

    // 게임이 시작되면 db에서 값을 받아 인벤토리에 아이템을 생성
    public void DBRead()
    {
        try
        {
            using (conn)
            {
                conn.Open();
                string sql = "SELECT * FROM inventory";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    // inventorycontroller의 함수를 호출해 아이템 생성
                    Debug.Log(rdr["posX"] + " " + rdr["posY"]);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("e : " + e.ToString());
        }
    }

    // 아이템 생성시 db에 새로운 데이터행을 추가
    public void DBSave(int posX, int posY, int itemID, int itemIndex)
    {
        try
        {
            using (conn)
            {
                conn.Open();
                string sql = $"INSERT INTO inventory VALUES({posX},{posY},{itemID},{itemIndex})";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
            }
        }
        catch (Exception e)
        {
            Debug.Log("e : " + e.ToString());
        }
    }

    // 아이템 삭제시 db에 해당 index를 가지고 있는 
    public void DBItemDelete(int index)
    {
        try
        {
            using (conn)
            {
                conn.Open();
                string sql = $"DELETE FROM inventory WHERE itemIndex={index}";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
            }
        }
        catch (Exception e)
        {
            Debug.Log("e : " + e.ToString());
        }
    }

    // 아이템을 삭제할시 아이템들의 인덱스를 변화해줘야한다
    public void DBIndexReSet(int index, int newindex)
    {
        try
        {
            using (conn)
            {
                conn.Open();
                string sql = $"UPDATE inventory SET itemIndex={newindex} WHERE itemIndex={index}";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Debug.Log(index++);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("e : " + e.ToString());
        }
    }

    // 인벤토리에서 아이템의 위치 변화시 db에 해당 아이템의 posX, posY값 저장
    public void DBPositionReSet(int posX, int posY, int index)
    {
        try
        {
            using (conn)
            {
                conn.Open();
                string sql = $"UPDATE inventory SET posX={posX}, posY={posY} WHERE itemIndex={index}";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
            }
        }
        catch (Exception e)
        {
            Debug.Log("e : " + e.ToString());
        }
    }
}
