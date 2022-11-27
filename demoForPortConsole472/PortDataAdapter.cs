using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace demoForPortConsole472
{
    internal interface IPortDataAdapter
    {
        event EventHandler<PortMessageEventArgs> MessageReceived;

        void Config(PortDataAdapterOptions options);

        void Open();

        void Close();

        void Send(byte[] contents, int offset, int length);

        void Send(byte[] contents);

        void Send(string content);
    }

    internal class PortDataAdapterOptions
    {
        /// <summary>
        /// 端口
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// 串行波特率
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// 奇偶校验检查协议
        /// </summary>
        public Parity? Parity { get; set; }

        /// <summary>
        /// 每个字节的标准数据位长度
        /// </summary>
        public int DataBits { get; set; }

        /// <summary>
        /// 每个字节的标准停止位数
        /// </summary>
        public StopBits? StopBits { get; set; }

        /// <summary>
        /// 读操作的超时时间(毫秒数)
        /// </summary>
        public int ReadTimeout { get; set; }

        /// <summary>
        /// 写操作的超时时间(毫秒数)
        /// </summary>
        public int WriteTimeout { get; set; }
    }

    internal class PortMessageEventArgs : EventArgs
    {
        public byte[] Data { get; private set; }

        public PortMessageEventArgs(byte[] data)
        {
            this.Data = data;
        }
    }

    internal class PortDataAdapter : IPortDataAdapter
    {
        public event EventHandler<PortMessageEventArgs> MessageReceived;

        private SerialPort _serialPort;
        private const int _baudRate = 115200;
        private const int _dataBits = 8;
        private const Parity _parity = Parity.None;
        private const StopBits _stopBits = StopBits.One;
        private const int _readTimeout = 500;
        private const int _writeTimeout = 500;
        private Thread _thread;
        private bool _continue;

        public PortDataAdapter() { }

        public void Config(PortDataAdapterOptions options)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (string.IsNullOrEmpty(options.PortName)) throw new ArgumentNullException("options.PortName");

            _serialPort = new SerialPort(options.PortName);
            _serialPort.BaudRate = options.BaudRate > 0 ? options.BaudRate : _baudRate;
            _serialPort.Parity = options.Parity ?? _parity;
            _serialPort.DataBits = options.DataBits > 0 ? options.DataBits : _dataBits;
            _serialPort.StopBits = options.StopBits ?? _stopBits;
            _serialPort.ReadTimeout = options.ReadTimeout > 0 ? options.ReadTimeout : _readTimeout;
            _serialPort.WriteTimeout = options.WriteTimeout > 0 ? options.WriteTimeout : _writeTimeout;
        }

        public void Open()
        {
            _serialPort.Open();
            Console.WriteLine($"串口是否开启:{_serialPort.IsOpen}，端口名称:{_serialPort.PortName}");

            _continue = true;
            _thread = new Thread(Read);
            _thread.IsBackground = true;
            _thread.Start();
            Console.WriteLine($"端口名称:{_serialPort.PortName}，消息监听开始");
        }

        private void Read()
        {
            while (_continue)
            {
                try
                {
                    if(_serialPort.BytesToRead > 0)
                    {
                        var data = new byte[_serialPort.BytesToRead];
                        _serialPort.Read(data, 0, data.Length);
                        Console.WriteLine($"接收到消息");
                        MessageReceived?.Invoke(this, new PortMessageEventArgs(data));
                    }
                }
                catch (TimeoutException) { }
                catch (Exception) { _continue = false; }
            }
        }

        public void Send(string content)
        {
            var contents = UTF8Encoding.GetEncoding("utf-8").GetBytes(content);
            Send(contents);
        }

        public void Send(byte[] contents)
        {
            Send(contents, 0, contents.Length);
        }

        public void Send(byte[] contents, int offset, int length)
        {
            if (_serialPort == null) throw new ArgumentNullException("_serialPort");
            if (!_serialPort.IsOpen) throw new ArgumentNullException("_serialPort is Closed");
            _serialPort.Write(contents, offset, length);
        }

        public void Close() 
        {
            _continue = false;
            _thread.Join();
            _serialPort.Close();
            Console.WriteLine($"串口是否开启:{_serialPort.IsOpen}，端口名称:{_serialPort.PortName}");
        }
    }
}
