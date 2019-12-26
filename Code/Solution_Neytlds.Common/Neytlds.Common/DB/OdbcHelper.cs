using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace Neytlds.Common.DB
{
    /// <summary>
    /// 使用 Odbc 方式操作数据库
    /// </summary>
    public static class OdbcHelper
    {
        // 数据库连接字符串
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["OdbcConnectionString"].ConnectionString;

        // 存储缓存参数
        public static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 执行数据库修改操作
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="comType">指定如何解释字符串</param>
        /// <param name="comText">存储过程或 SQL 语句</param>
        /// <param name="commandParameter">用于执行命令的参数列表</param>
        /// <returns>影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType comType, string comText, params OdbcParameter[] commandParameter)
        {
            var cmd = new OdbcCommand();

            using var con = new OdbcConnection(connectionString);
            PrepareCommand(cmd, con, null, comType, comText, commandParameter);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行数据库修改操作
        /// </summary>
        /// <param name="connection">现有数据库连接</param>
        /// <param name="comType">指定如何解释字符串</param>
        /// <param name="comText">存储过程或 SQL 语句</param>
        /// <param name="commandParameter">用于执行命令的参数列表</param>
        /// <returns>影响的行数</returns>
        public static int ExecuteNonQuery(OdbcConnection connection, CommandType comType, string comText, params OdbcParameter[] commandParameter)
        {
            var cmd = new OdbcCommand();

            PrepareCommand(cmd, connection, null, comType, comText, commandParameter);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行数据库修改操作
        /// </summary>
        /// <param name="trans">现有 SQL 的事务</param>
        /// <param name="comType">指定如何解释字符串</param>
        /// <param name="comText">存储过程或 SQL 语句</param>
        /// <param name="commandParameter">用于执行命令的参数列表</param>
        /// <returns>影响的行数</returns>
        public static int ExecuteNonQuery(OdbcTransaction trans, CommandType comType, string comText, params OdbcParameter[] commandParameter)
        {
            var cmd = new OdbcCommand();

            PrepareCommand(cmd, trans.Connection, trans, comType, comText, commandParameter);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 查询数据并返回结果
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">指定如何解释字符串</param>
        /// <param name="cmdText">存储过程或 SQL 语句</param>
        /// <param name="commandParameter">用于执行命令的参数列表</param>
        /// <returns>查询结果</returns>
        public static OdbcDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params OdbcParameter[] commandParameter)
        {
            var cmd = new OdbcCommand();
            var conn = new OdbcConnection(connectionString);

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameter);
                OdbcDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 查询数据并返回结果的第一行第一列
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="comType">指定如何解释字符串</param>
        /// <param name="comText">存储过程或 SQL 语句</param>
        /// <param name="commandParameter">用于执行命令的参数列表</param>
        /// <returns>查询结果的第一行第一列</returns>
        public static object ExecuteScalar(string connectionString, CommandType comType, string comText, params OdbcParameter[] commandParameter)
        {
            var cmd = new OdbcCommand();

            using var connection = new OdbcConnection(connectionString);
            PrepareCommand(cmd, connection, null, comType, comText, commandParameter);
            var val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 查询数据并返回结果的第一行第一列
        /// </summary>
        /// <param name="connection">现有数据库连接</param>
        /// <param name="comType">指定如何解释字符串</param>
        /// <param name="comText">存储过程或 SQL 语句</param>
        /// <param name="commandParameter">用于执行命令的参数列表</param>
        /// <returns>查询结果的第一行第一列</returns>
        public static object ExecuteScalar(OdbcConnection connection, CommandType comType, string comText, params OdbcParameter[] commandParameter)
        {
            var cmd = new OdbcCommand();

            PrepareCommand(cmd, connection, null, comType, comText, commandParameter);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }


        /// <summary>
        /// 将参数数组添加到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存参数的 key 值</param>
        /// <param name="cmdParms">要缓存的 OdbcParameter 数组</param>
        public static void CacheParameters(string cacheKey, params OdbcParameter[] commandParameter)
        {
            parmCache[cacheKey] = commandParameter;
        }

        /// <summary>
        /// 检索缓存参数
        /// </summary>
        /// <param name="cacheKey">用于查找参数的 key 值</param>
        /// <returns>缓存在 OdbcParameter 的数组</returns>
        public static OdbcParameter[] GetCachedParameters(string cacheKey)
        {
            var cachedParms = (OdbcParameter[])parmCache[cacheKey];

            if (cachedParms == null) { return null; }

            var clonedParms = new OdbcParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
            {
                clonedParms[i] = (OdbcParameter)((ICloneable)cachedParms[i]).Clone();
            }

            return clonedParms;
        }


        #region Helper

        /// <summary>
        /// 参数赋值
        /// </summary>
        /// <param name="cmd">SqlCommand</param>
        /// <param name="conn">SqlConnection</param>
        /// <param name="trans">SqlTransaction</param>
        /// <param name="cmdType">指定如何解释字符串</param>
        /// <param name="cmdText">存储过程或 SQL 语句</param>
        /// <param name="cmdParms">用于执行命令的参数列表</param>
        private static void PrepareCommand(OdbcCommand cmd, OdbcConnection conn, OdbcTransaction trans, CommandType comType, string comText, OdbcParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open) { conn.Open(); }

            cmd.Connection = conn;
            cmd.CommandText = comText;

            if (trans != null) { cmd.Transaction = trans; }

            cmd.CommandType = comType;

            if (cmdParms != null)
            {
                foreach (OdbcParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }

        #endregion

    }
}
