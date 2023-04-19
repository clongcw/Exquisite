using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using Exquisite.Shared.Components;

namespace Exquisite.Utils;

public static class CommunicationUtil
{
    public static List<CommunicatePort> CommunicatePortList = new();

    public static void InitDevice(string serialNum, string protocolName, byte slaveId, string port, int boudrate,
        int dataBit, int parityBit, int stopBit)
    {
        var exist = false;
        var p = new CommunicatePort();
        //首先检测当前设备需要注册的端口是否已经被注册过，如果没有则直接注册进去，如果已经有了，那么只需要把设备注册到这个端口
        int i;
        for (i = 0; i < CommunicatePortList.Count; i++)
            if (CommunicatePortList[i].hardwareType.Equals("COM"))
                if (CommunicatePortList[i].port.Equals(port, StringComparison.OrdinalIgnoreCase))
                {
                    //这个端口已经注册
                    Logger.Instance.Debug("initDevice", "当前串口--->" + port + "   已经注册，只需要挂载设备");
                    exist = true;
                    break;
                }

        //注册设备
        var dci = new DeviceCommunicateInfo();
        dci.serialNum = serialNum;
        dci.deviceId = slaveId.ToString();
        dci.protocolName = protocolName;
        dci.PublishFrameQueue = new Queue<(string, int)>();

        if (!exist)
        {
            //端口没有注册过
            Logger.Instance.Information("initDevice", "注册串口--->" + port);
            p.hardwareType = "COM";
            p.port = port; //串口号
            p.boudrate = boudrate;
            p.dataBit = dataBit;
            p.parityBit = parityBit;
            p.stopBit = stopBit;
            p.devices.Add(dci);
            CommunicatePortList.Add(p);
        }
        else
        {
            //端口已经注册过，只要在这个端口上面注册设备
            CommunicatePortList[i].devices.Add(dci);
        }
    }

    public static void StartAllDevice()
    {
        foreach (var cp in CommunicatePortList)
            //依次检测所有的端口，并启动对应的线程
            if (cp.hardwareType.Equals("COM", StringComparison.OrdinalIgnoreCase))
                Task.Run(() =>
                {
                    COM_WAIT_DEVICE:
                    while (true)
                    {
                        var needRun = false;
                        Task.Delay(1000).Wait();
                        for (var i = 0; i < cp.devices.Count; i++)
                            // 该端口下只要还有一个设备要运行，那么就不能关闭端口，仅仅是停止对应设备的通讯
                            if (cp.devices[i].communicateEnable)
                                needRun = true;

                        if (needRun) break;
                    }

                    COM_INIT:
                    for (var i = 0; i < cp.devices.Count; i++)
                    {
                        cp.devices[i].connected = false;
                        cp.devices[i].timeoutCount = 4;
                    }

                    var serialPort = new SerialPort(cp.port, cp.boudrate, (Parity)cp.parityBit, cp.dataBit,
                        (StopBits)cp.stopBit);
                    var receive = new byte[256];
                    var receiveTmp = new byte[256];
                    var deviceIndex = 0;
                    try
                    {
                        serialPort.ReadTimeout = 1000;
                        serialPort.Open();
                        Console.WriteLine(">>>>>>>>启动串口 " + cp.port);

                        // 串口打开成功，将该信息传递给该串口下的所有设备
                        for (var i = 0; i < cp.devices.Count; i++) cp.devices[i].connected = true;

                        Logger.Instance.Information(cp.port, "打开串口成功");
                    }
                    catch (Exception e)
                    {
                        // 串口打开成功，将该信息传递给该串口下的所有设备
                        for (var i = 0; i < cp.devices.Count; i++) cp.devices[i].connected = false;

                        Logger.Instance.Error("打开串口 " + cp.port + "失败，延时1秒后重试。", e.ToString());
                        Task.Delay(1000).Wait();
                        goto COM_INIT;
                    }

                    while (serialPort.IsOpen)
                        try
                        {
                            deviceIndex = 0;
                            for (deviceIndex = 0; deviceIndex < cp.devices.Count; deviceIndex++)
                            {
                                if (cp.devices[deviceIndex].communicateEnable == false)
                                {
                                    // 当前端口下有一个设备禁止通讯了
                                    // 检测是否还有其他设备
                                    var allDeviceDisable = true;
                                    for (var i = 0; i < cp.devices.Count; i++)
                                        // 该端口下只要还有一个设备要运行，那么就不能关闭端口，仅仅是停止对应设备的通讯
                                        if (cp.devices[i].communicateEnable)
                                            allDeviceDisable = false;

                                    if (allDeviceDisable)
                                    {
                                        // 所有设备都禁用通讯了,关闭串口，并进入到等到设备使能的循环中
                                        serialPort.Close();
                                        goto COM_WAIT_DEVICE;
                                    }

                                    // 当前设备是关闭通讯的，立即跳转到下一条指令
                                    continue;
                                }

                                // 串口设备由于是独享的，所以该通讯口下面所有的设备将轮流使用串口
                                // 每个设备每次发送一条指令，然后就切换到下一个设备，这样可以保证每个设备都能够在较短的时间内使用到串口
                                // 而不会应该某台设备发送的指令多而导致其他的设备响应变慢
                                if (cp.devices[deviceIndex].PublishFrameQueue.Count > 0)
                                {
                                    (string, int) frame;
                                    lock (cp.sendQLock)
                                    {
                                        frame = cp.devices[deviceIndex].PublishFrameQueue.Dequeue(); //取出需要发送的数据
                                    }
                                }

                                // 每进行一次指令交互后，延时50ms
                                // 理论上多设备的情况下，这里不需要进行延时，因为下一条指令是给下一个设备的
                                // 但是这里为了方便多设备也会进行延时
                                Task.Delay(50).Wait();
                            }
                        }
                        catch (Exception ee)
                        {
                            Logger.Instance.Error(cp.devices[deviceIndex].serialNum,"串口" + cp.port + " ArgumentException\n" + ee);
                            goto COM_INIT;
                        }
                });
    }
}

public class DeviceCommunicateInfo
{
    public readonly Queue<string> ReceiveFrameQueue = new(); //数据帧处理队列
    public int canChannel; // 当前设备用到的CAN盒的通道号
    public bool communicateEnable = true; //该设备是否需要进行通讯，默认需要通讯

    // 是否建立了连接，对于串口就是串口是否正常打开，网络通讯就是是否能建立TCP连接，UDP的则是是否能正常binding对于的端口
    public bool connected;
    public string deviceId; //设备id
    public string deviceIp; //设备ip
    public string devicePort; //设备网络端口号
    public string netType; //UDP还是TCP
    public string protocolName; //使用的协议，MODBUS-RTU
    public Queue<(string, int)> PublishFrameQueue = new(); //数据帧发送队列
    public string serialNum; //设备序列号

    // 设备通讯超时计数，每次正确通讯将置0，通讯超时加1，调用方通过这个变量来确定当前设备通讯是否正常，前提是connected为true，这个值才是有效的
    public int timeoutCount = 4;

    // 设备通讯超时计数，这个值是不会清零的，会记录软件启动以来的所有的超时数量
    public int timeoutTotalCount;
}

public class CommunicatePort
{
    //如果是串口，那么这里需要初始化对于的串口参数
    public int boudrate; // 针对串口和CAN有效
    public int dataBit;
    public List<DeviceCommunicateInfo> devices = new(); //这个通讯口下面挂的设备列表

    public string hardwareType; //通讯口类型

    // 网络端口号，UDP的时候会用到，有些UDP服务器端会设定这个端口，其他端口的数据不会进行接收
    // 如果这个参数为null或者空，那么就使用随机端口
    public string netPort;
    public string netType; //如果是网口，那么要确定是UDP还是TCP
    public int parityBit;
    public string port; //如果是串口，这里就是串口号，如果是网口就是本机网址,如果是CAN，就是通道号

    public object receiveQLock = new();

    //以下两个object用于多线程操作时避免对端口的同时操作
    public object sendQLock = new();
    public int stopBit;
}