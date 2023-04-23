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
    private readonly DialogHost _dialogHost;
    public ObservableCollection<string> Batterys { get; set; } = new();
    public ObservableCollection<string> Processes { get; set; } = new();
    public ObservableCollection<Step> Steps { get; set; } = new();

    public string SelectedBattery { get; set; }
    public string SelectedProcess { get; set; }


    DbContext<Battery> ORDB = new();
    public SqlSugarClient CurrentDb { get; set; }

    public DcViewModel()
    {
        _dialogHost = new DialogHost();

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
        CurrentDb.CodeFirst.InitTables<Battery, Process, Step>();


    }

    public void Cancel()
    {
        DialogHost.Close(null!);
    }

    public void AddBattery()
    {
        Battery battery = new Battery();
        battery.Name = "BatteryC";

        Process process = new Process();
        Step step = new Step();
        try
        {
            process = new Process();
            process.Name = "";
            process.BatteryName = battery.Name; // 设置外键关联

            step = new Step();
            step.Voltage = 220;
            step.ProcessName = process.Name; // 设置外键关联
            step.Deadline = DateTime.Now.ToString(); // 设置外键关联
        }
        catch (Exception ee)
        {
            Logger.Instance.Error(ee.Message);
        }

    }

    public async void ExecuteRunDialog()
    {
        Batterys.Clear();

        ObservableCollection<Battery> a1 = new ObservableCollection<Battery>(CurrentDb.Queryable<Battery>()
            .OrderBy(x => x.Name)
            .ToList());

        foreach (var item in a1)
        {
            Batterys.Add(item.Name);
        }

        //let's set up a little MVVM, cos that's what the cool kids are doing:
        Dc_Battery view = new Dc_Battery() { DataContext = this };

        //show the dialog
        var result = await DialogHost.Show(view, null, ClosingEventHandler, ClosedEventHandler);
    }

    public void RefreshProcess()
    {
        Processes.Clear();
        ObservableCollection<Process> a2 = new ObservableCollection<Process>(CurrentDb.Queryable<Process>()
           .Where(x => x.BatteryName == SelectedBattery)
           .OrderBy(x => x.Name)
            .ToList());
        foreach (var item in a2)
        {
            Processes.Add(item.Name);
        }
    }

    public void RefreshSteps()
    {
        Steps.Clear();
        Steps = new ObservableCollection<Step>(CurrentDb.Queryable<Step>()
            .Where(x => x.ProcessName == SelectedProcess)
            .OrderBy(x => x.ProcessName)
            .ToList()); ;
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

    [SugarColumn(IsNullable = true)]
    public List<Process>? Processes { get; set; }
}

public class Process
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    public string? Name { get; set; }


    [SugarColumn(IsNullable = true)]
    public DateTime CreateTime { get; set; }

    public string BatteryName { get; set; }
    [SugarColumn(IsNullable = true)]
    public List<Step>? Steps { get; set; }

    internal static void Add(string? name)
    {
        throw new NotImplementedException();
    }
}

public class Step
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Power { get; set; }
    public string? Deadline { get; set; }
    public bool NeedRun { get; set; }


    [SugarColumn(IsNullable = true)]
    public DateTime CreateTime { get; set; }

    public string ProcessName { get; set; }
}