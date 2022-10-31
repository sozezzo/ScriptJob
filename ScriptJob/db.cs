using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ScriptJob
{
    public class Db
    {
        /// <summary>
        /// Get the first value, first column, first line
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="scriptSql"></param>
        /// <returns></returns>
        public static string ExecuteGetValue(SqlCommand cmd, string scriptSql)
        {
            cmd.CommandText = scriptSql;
            SqlDataReader reader = cmd.ExecuteReader();
            string ret = "";

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ret = reader.GetString(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Show(ex.Message);
            }

            if (!reader.IsClosed) reader.Close();
            return ret;
        }

        public static string ExecuteGetColumnToString(SqlCommand cmd, string scriptSql, int columnNameId)
        {
            cmd.CommandText = scriptSql;
            SqlDataReader reader = cmd.ExecuteReader();
            string ret = "";
            string line;

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        line = Convert.ToString(reader.GetString(columnNameId));
                        ret += string.IsNullOrEmpty(line) ? "\r\n" : reader.GetString(columnNameId)+"\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                Show(ex.Message);
            }

            if (!reader.IsClosed) reader.Close();
            return ret;
        }

        /// <summary>
        /// Get the first value, first column, first line
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="scriptSql"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ExecuteGetValue(SqlCommand cmd, string scriptSql, DateTime defaultValue)
        {
            cmd.CommandText = scriptSql;
            SqlDataReader reader = cmd.ExecuteReader();
            DateTime ret = defaultValue;

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ret = reader.GetDateTime(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Show(ex.Message);
            }

            if (!reader.IsClosed) reader.Close();
            return ret;
        }

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="scriptSql"></param>
        /// <returns></returns>
        public static bool ExecuteNonQuery(SqlCommand cmd, string scriptSql)
        {
            cmd.CommandText = scriptSql;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Show(ex.Message);
                return false;
            }
            return true;
        }

        private static void Show(string text)
        {
            Console.WriteLine(text);
        }
    }
}
