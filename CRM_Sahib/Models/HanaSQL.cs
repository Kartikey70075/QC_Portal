using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace V_Weave_Qc.Models
{
    public class HanaSQL
    {

        public DataTable GetHanaDataSQL(string Command)



        {
            try

            {
                HanaConnection con = new HanaConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Hana"].ConnectionString);

                string SCHEMA = con.ConnectionString.Split(';')[2].Split('=')[1];
                con.Open();
                HanaDataAdapter dataAdapter = new HanaDataAdapter(Command, con);
                DataTable Table = new DataTable();
                dataAdapter.Fill(Table);
                con.Close();
                return Table;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName].ToString(), null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}