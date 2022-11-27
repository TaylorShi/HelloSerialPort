using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace demoForPortConsole472
{
    internal class Program
    {
        private static SerialPort _serialPort = null;
        private static bool _continue;

        private static void PortDataAdapter_MessageReceived(object sender, PortMessageEventArgs e)
        {
            
        }

        static void Main(string[] args)
        {
            var options = new PortDataAdapterOptions
            {
                PortName = "COM1"
            };
            IPortDataAdapter portDataAdapter = new PortDataAdapter();
            portDataAdapter.Config(options);
            portDataAdapter.MessageReceived += PortDataAdapter_MessageReceived;
            portDataAdapter.Open();
            portDataAdapter.Send("haha");
            portDataAdapter.Close();

            _serialPort = new SerialPort();
            var defaultPortName = _serialPort.PortName;
            Console.WriteLine($"默认端口:{defaultPortName}");

            foreach (var portName in SerialPort.GetPortNames())
            {
                Console.WriteLine($"可用端口:{portName}");
            }

            var defaultBaudRate = _serialPort.BaudRate;
            Console.WriteLine($"默认串行波特率:{defaultBaudRate}");

            var defaultParity = _serialPort.Parity;
            Console.WriteLine($"默认奇偶校验检查协议:{defaultParity}");

            foreach (var parity in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine($"可选奇偶校验检查协议:{parity}");
            }

            var defaultDataBits = _serialPort.DataBits;
            Console.WriteLine($"默认每个字节的标准数据位长度:{defaultDataBits}");

            var defaultStopBits = _serialPort.StopBits;
            Console.WriteLine($"默认每个字节的标准停止位数:{defaultStopBits}");

            foreach (var stopBit in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine($"可选每个字节的标准停止位数:{stopBit}");
            }

            var defaultHandshake = _serialPort.Handshake;
            Console.WriteLine($"默认串行端口数据传输的握手协议:{defaultHandshake}");

            foreach (var handshake in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine($"可选串行端口数据传输的握手协议:{handshake}");
            }

            var defaultReadTimeout = _serialPort.ReadTimeout;
            Console.WriteLine($"默认读取操作未完成时发生超时之前的毫秒数:{defaultReadTimeout}");

            // 设置读操作的超时时间(毫秒数)
            _serialPort.ReadTimeout = 500;

            var defaultWriteTimeout = _serialPort.WriteTimeout;
            Console.WriteLine($"默认写入操作未完成时发生超时之前的毫秒数:{defaultWriteTimeout}");

            // 设置写操作的超时时间(毫秒数)
            _serialPort.WriteTimeout = 500;

            // 已通过由SerialPort对象表示的端口接收了数据
            _serialPort.DataReceived += SerialPort_DataReceived;

            // 由SerialPort对象表示的端口上发生了错误
            _serialPort.ErrorReceived += SerialPort_ErrorReceived;

            // 由SerialPort对象表示的端口上发生了非数据信号事件
            _serialPort.PinChanged += SerialPort_PinChanged;

            _serialPort.Open();
            Console.WriteLine($"串口是否开启:{_serialPort.IsOpen}，端口名称:{_serialPort.PortName}");

            Thread readThread = new Thread(Read);
            _continue = true;
            readThread.Start();
            _serialPort.WriteLine("message");

            var contents = new byte[1024];
            SendMessage(contents, 0, contents.Length);

            _serialPort.Close();
            Console.WriteLine($"串口是否开启:{_serialPort.IsOpen}，端口名称:{_serialPort.PortName}");

            Console.ReadLine();
        }



        private static Thread _readThread = null;

        private static void Open()
        {
            _serialPort.Open();
            Console.WriteLine($"串口是否开启:{_serialPort.IsOpen}，端口名称:{_serialPort.PortName}");

            _readThread = new Thread(Read);
            _readThread.IsBackground = true;
            _readThread.Start();
            Console.WriteLine($"端口名称:{_serialPort.PortName}，消息监听开始");
        }

        private static void SendMessage(byte[] contents, int offset, int length)
        {
            _serialPort.Write(contents, offset, length);
        }

        private static void SerialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Console.WriteLine($"非数据信号改变类型:{e.EventType}");
        }

        private static void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine($"串口消息错误类型:{e.EventType}");
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string message = sp.ReadExisting();
            Console.WriteLine($"接收到消息:{message}");
        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    Console.WriteLine($"接收到消息:{message}");
                }
                catch (TimeoutException) { }
                catch (Exception) { _continue = false; }
            }
        }

        public static string SetPortName(string defaultPortName)
        {
            string portName;

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
            portName = Console.ReadLine();

            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
            {
                portName = defaultPortName;
            }
            return portName;
        }
    }
}
