using System;
using System.IO.Ports;
using System.Threading;

namespace demoForPortConsole31
{
    internal class Program
    {
        private static SerialPort _serialPort = null;
        static void Main(string[] args)
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = SetPortName(_serialPort.PortName);
            Console.WriteLine("Hello World!");
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
