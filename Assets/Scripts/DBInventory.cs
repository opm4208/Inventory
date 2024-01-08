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

    // ������ ���۵Ǹ� db���� ���� �޾� �κ��丮�� �������� ����
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
                    // inventorycontroller�� �Լ��� ȣ���� ������ ����
                    Debug.Log(rdr["posX"] + " " + rdr["posY"]);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("e : " + e.ToString());
        }
    }

    // ������ ������ db�� ���ο� ���������� �߰�
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

    // ������ ������ db�� �ش� index�� ������ �ִ� 
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

    // �������� �����ҽ� �����۵��� �ε����� ��ȭ������Ѵ�
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

    // �κ��丮���� �������� ��ġ ��ȭ�� db�� �ش� �������� posX, posY�� ����
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
