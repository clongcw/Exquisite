using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbType = SqlSugar.DbType;

namespace Exquisite.Shared.Components
{
    public class DbContext<T> where T : class, new()
    {

        public SqlSugarClient Db;
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Environments.ConnectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                AopEvents = new AopEvents()
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                    }
                }
            });
        }
        public SimpleClient<T> CurrentDb => new SimpleClient<T>(Db);

        public bool CreateDatabase()
        {
            return CurrentDb.Context.DbMaintenance.CreateDatabase();
        }

        public void InitTables(params Type[] entityTypes)
        {
            CurrentDb.Context.CodeFirst.InitTables(entityTypes);
        }

        public virtual int InsertReturnIdentity(T insertObj)
        {
            return CurrentDb.InsertReturnIdentity(insertObj);
        }

        public virtual Task<T> GetByIdAsync(int id)
        {
            return CurrentDb.GetByIdAsync(id);
        }
        public virtual T GetById(int id)
        {
            return CurrentDb.GetById(id);
        }

        public virtual Task<List<T>> GetListAsync()
        {
            return CurrentDb.GetListAsync();
        }
        public virtual List<T> GetList()
        {
            return CurrentDb.GetList();
        }
        public virtual Task<bool> Delete(int id)
        {
            return CurrentDb.DeleteByIdAsync(id);
        }
        /*public virtual Task<bool> Update(T insertObj)
        {
            return CurrentDb.Update((it => new Order() { Name = "a", }, it => it.Id == insertObj));
        }*/
    }
}
