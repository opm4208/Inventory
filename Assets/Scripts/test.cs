using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class test : MonoBehaviour
{
    public static MySqlConnection conn;
    private void Start()
    {
        Debug.Log("Connection Test : " + ConnectionTest());
    }

    public bool ConnectionTest()
    {
        string conStr = string.Format("Server={0};Database={1};Uid ={2};Pwd={3};",
            "127.0.0.1", "test", "root", "qkrals123#");

        DataSet ds = new DataSet();
        try
        {
            using (conn = new MySqlConnection(conStr))
            {
                conn.Open();
                string sql = "SELECT * FROM inventory";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr =cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Debug.Log( rdr["posX"]+" "+ rdr["posY"]);
                }
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("e : " + e.ToString());
            return false;
        }
    }
    public static DataSet OnSelectRequest(string p_query, string table_name)
    {
        try
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = p_query;

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sd.Fill(ds, table_name);
            return ds;
        }
        catch (System.Exception e)
        {
            Debug.Log("selectefalse");
            return null;
        }
    }
}