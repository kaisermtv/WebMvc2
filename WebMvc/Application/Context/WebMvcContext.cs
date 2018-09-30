   using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebMvc.Services;

namespace WebMvc.Application.Context
{
    public partial class WebMvcContext
    {
        private SqlConnection con;
        private SqlTransaction transaction;
        private string constr;
        private List<string> cacheStartsWithToClear;
        private readonly CacheService _cacheService;

        public WebMvcContext(CacheService cacheService)
        {
            _cacheService = cacheService;
            constr = ConfigurationManager.ConnectionStrings["WebMvcContext"].ToString();
        }

        public SqlTransaction BeginTransaction()
        {
            if (con == null)
            {
                con = new SqlConnection(constr);
                con.Open();
                transaction = con.BeginTransaction();
                cacheStartsWithToClear = new List<string>();
            }
            return transaction;
        }

        public DbCommand CreateCommand()
        {
            return new DbCommand(con, transaction, constr, cacheStartsWithToClear, _cacheService);
        }

        public class DbCommand : IDisposable
        {
            private SqlCommand _command;
            private SqlConnection con;
            private SqlTransaction transaction;
            private bool isclose = false;
            private List<string> cachekey;
            private readonly CacheService _cacheService;

            public SqlCommand command
            {
                get { return _command; }
            }

            public string CommandText
            {
                get
                {
                    return _command.CommandText;
                }
                set
                {
                    _command.CommandText = value;
                }
            }

            public SqlParameterCollection Parameters
            {
                get
                {
                    return _command.Parameters;
                }
            }

            #region AddParameters
            public void AddParameters(string key, SqlDbType sqltp, object value)
            {
                if (value == null)
                {
                    _command.Parameters.Add(key, sqltp).Value = DBNull.Value;
                }
                else
                {
                    _command.Parameters.Add(key, sqltp).Value = value;
                }
            }

            public void AddParameters(string key, Guid? value)
            {
                AddParameters(key, SqlDbType.UniqueIdentifier, value);
            }

            public void AddParameters(string key, int value)
            {
                AddParameters(key, SqlDbType.Int, value);
            }

			public void AddParameters(string key, double value)
			{
				AddParameters(key, SqlDbType.BigInt, value);
			}

			public void AddParameters(string key, string value)
            {
                AddParameters(key, SqlDbType.NVarChar, value);
            }

            public void AddParameters(string key, bool? value)
            {
                AddParameters(key, SqlDbType.Bit, value);
            }

            public void AddParameters(string key, DateTime value)
            {
                AddParameters(key, SqlDbType.DateTime, value);
            }
            #endregion


            public DbCommand(SqlConnection _con, SqlTransaction _transaction, string constr, List<string> _cachekey, CacheService cacheService)
            {
                con = _con;
                transaction = _transaction;
                cachekey = _cachekey;
                _cacheService = cacheService;

                if (con == null)
                {
                    con = new SqlConnection(constr);
                    con.Open();
                    isclose = true;
                }

                _command = con.CreateCommand();
                if (transaction != null) _command.Transaction = transaction;

            }

            public DataRow findFirst()
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = _command;
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0];
                }
                else
                {
                    return null;
                }
            }

			public Entity FindFirst<Entity>() where Entity : class, new()
			{
				SqlDataAdapter da = new SqlDataAdapter();
				da.SelectCommand = _command;
				DataSet ds = new DataSet();
				da.Fill(ds);

				if (ds.Tables[0].Rows.Count == 0) return null;
				var data = ds.Tables[0].Rows[0];

				var entity = new Entity();

				Type t = typeof(Entity);
				var p = t.GetProperties();

				foreach (var it in p)
				{
					if (ds.Tables[0].Columns[it.Name] != null)
					{
						if (data[it.Name] != DBNull.Value)
						{
							it.SetValue(entity, data[it.Name]);
						}
						else
						{
							it.SetValue(entity, null);
						}
					}
				}

				return entity;
			}

			public DataTable findAll()
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = _command;
                DataSet ds = new DataSet();
                da.Fill(ds);

                return ds.Tables[0];
            }

			public List<Entity> FindAll<Entity>() where Entity : class, new()
			{
				DataTable data = findAll();
				if (data == null) return null;

				var lst = new List<Entity>();
				Type t = typeof(Entity);
				var p = t.GetProperties();

				foreach (DataRow item in data.Rows)
				{
					var entity = new Entity();

					foreach (var it in p)
					{
						if (data.Columns[it.Name] != null)
						{
							if (item[it.Name] != DBNull.Value)
							{
								it.SetValue(entity, item[it.Name]);
							}
							else
							{
								it.SetValue(entity, null);
							}
						}
					}

					lst.Add(entity);
				}


				return lst;
			}

			public void Close()
            {
                if (isclose) con.Close();
            }

            public void cacheStartsWithToClear(string _cachekey)
            {
                if (cachekey != null)
                {
                    if (cachekey.IndexOf(_cachekey) == -1)
                        cachekey.Add(_cachekey);
                }
                else
                {
                    _cacheService.ClearStartsWith(_cachekey);
                }
            }

			public void Dispose()
			{
				Close();
			}
		}

        public void Dispose()
        {
            if (con != null)
            {
                con.Close();
                con = null;
                transaction = null;
                _cacheService.ClearStartsWith(cacheStartsWithToClear);
            }

        }
    }
}