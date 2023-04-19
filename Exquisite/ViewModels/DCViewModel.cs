using Exquisite.Shared.Components;
using System.Security.Principal;
using System;
using SqlSugar;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Threading;
using MaterialDesignThemes.Wpf;
using System.Diagnostics;
using Exquisite.Views.Dialogs;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Windows;

namespace Exquisite.ViewModels;

public class DcViewModel : ViewModelBase
{
    public ObservableCollection<Battery> BatteryLibs { get; set; }
    DbContext<Battery> ORDB = new();
    public SqlSugarClient CurrentDb { get; set; }

    public DcViewModel()
	{
        CurrentDb = new SqlSugarClient(new ConnectionConfig()
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
        //删除数据库
        //CurrentDb.Context.DbMaintenance.TruncateTable<Battery, Process, Step>();
        // 创建数据库，没有就创建，有就跳过
        CurrentDb.CodeFirst.InitTables();
        // 创建表，没有就创建，有就跳过
        CurrentDb.CodeFirst.InitTables<Battery,Process,Step>();

        
    }

    public void AddBattery()
    {
        Battery battery = new Battery();
        battery.Name = "BatteryB";

        Process process = new Process();
        process.Name = "Process12";
        process.BatteryId = battery.Id; // 设置外键关联

        Step step = new Step();
        step.Voltage = 220;
        step.ProcessId = process.Id; // 设置外键关联
        step.Deadline = DateTime.Now.ToString(); // 设置外键关联

        try
        {
            // 添加Battery记录
            CurrentDb.Insertable(battery).ExecuteReturnIdentity();

            // 添加Process记录
            CurrentDb.Insertable(process).ExecuteReturnIdentity();

            // 添加Step记录
            CurrentDb.Insertable(step).ExecuteReturnIdentity();
        }
        catch (Exception ee)
        {
            if (ee.ToString().Contains("UNIQUE"))
            {
                Logger.Instance.Error(ee.Message);
            }
        }
    }

    public async void ExecuteRunDialog()
    {
        //let's set up a little MVVM, cos that's what the cool kids are doing:
        var view = new Dc_Battery() { DataContext = this };

        //show the dialog
        var result = await DialogHost.Show(view, null, ClosingEventHandler, ClosedEventHandler);
        //check the result...
        Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
    }

    private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
            => Debug.WriteLine("You can intercept the closing event, and cancel here.");

    private void ClosedEventHandler(object sender, DialogClosedEventArgs eventArgs)
        => Debug.WriteLine("You can intercept the closed event here (1).");

    public async void Add()
    {
        await Task.Factory.StartNew(() =>
        {
            ORDB.InsertReturnIdentity(new Battery() { Name = "66", CreateTime = DateTime.Now });

        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

}

[SugarIndex("unique_codetable1_Name", nameof(Battery.Name), OrderByType.Asc, true)]
public class Battery
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    
    public string? Name { get; set; }


    [SugarColumn(IsNullable = true)] 
    public DateTime CreateTime { get; set; }
   
    public List<Process> Processes { get; set; }
}

public class Process
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    public string? Name { get; set; }


    [SugarColumn(IsNullable = true)]
    public DateTime CreateTime { get; set; }

    public int BatteryId { get; set; }

    public List<Step> Steps { get; set; }
}

public class Step
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Power { get; set; }
    public string Deadline { get; set; }
    public bool NeedRun { get; set; }


    [SugarColumn(IsNullable = true)]
    public DateTime CreateTime { get; set; }

    public int ProcessId { get; set; }
}