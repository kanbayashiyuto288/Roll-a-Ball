﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System;

namespace AppKit
{
    /// <summary>
    /// UART communication for Unity
    /// </summary>
    public class SerialHandler : MonoBehaviour
    {
        public enum Baudrate
        {
            B_300 = 300,
            B_1200 = 1200,
            B_2400 = 2400,
            B_4800 = 4800,
            B_9600 = 9600,
            B_19200 = 19200,
            B_38400 = 38400,
            B_57600 = 57600,
            B_74880 = 74880,
            B_115200 = 115200,
            B_230400 = 230400,
            B_250000 = 250000
        }

        public event Action<string> OnDataReceived;

        public SerialPort _serialPort;
        Thread _thread;
        Queue<string> _messages;
        string _data = "";
        bool _isRunning;

        void Update()
        {
            if (!_isRunning)
            {
                return;
            }
            lock (_messages)
            {
                while (_messages.Count > 0)
                {
                    string msg = _messages.Dequeue();
                    if (OnDataReceived != null)
                    {
                        OnDataReceived(msg);
                    }
                }
            }
        }

        #region public

        public static string[] GetPortNames()
        {
            if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                // Custom port name in MacOS
                // return Directory.GetFiles(@"/dev/", "cu.usb*", SearchOption.AllDirectories);
                return Directory.GetFiles(@"/dev/", "tty.usb*", SearchOption.AllDirectories);
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                return SerialPort.GetPortNames();
            }
            // else
            Debug.LogError("Unsupported platform");
            return new string[0];
        }

        /// <summary>
        /// Open the specified portName and baudRate.
        /// </summary>
        /// <param name="portName">Port name.</param>
        /// <param name="baudRate">Baud rate.</param>
        public bool Open(string portName = null, Baudrate baudRate = Baudrate.B_9600)
        {
            if (string.IsNullOrEmpty(portName))
            {
                // Connect to default port
                var ports = GetPortNames();

                if (Debug.isDebugBuild)
                {
                    foreach (var port in ports)
                    {
                        Debug.LogFormat("port : {0}", port);
                    }
                    if (ports.Length == 0)
                    {
                        Debug.LogWarning("Serial port not found");
                    }
                }

                if (ports.Length == 0)
                {
                    return false;
                }
                portName = ports[0];
            }
            _messages = new Queue<string>();
            _serialPort = new SerialPort(portName, (int)baudRate, Parity.None, 8, StopBits.One);
            //            _serialPort.ReadTimeout = 10000;
            //            _serialPort.WriteTimeout = 10000;
            try
            {
                _serialPort.Open();
            }
            catch (IOException e)
            {
                Debug.LogError(e);
                _serialPort.Dispose();
                return false;
            }

            _isRunning = true;

            _thread = new Thread(Read);
            _thread.Start();

            return true;
        }

        public void Close()
        {
            _isRunning = false;

            if (_thread != null && _thread.IsAlive)
            {
                _thread.Join();
            }
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }
        }

        public void Write(string message)
        {
            try
            {
                _serialPort.Write(message);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        #endregion

        #region Private
        void Read()
        {
            while (_isRunning && _serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    // http://answers.unity3d.com/questions/1022086/reading-serial-data-in-unity-from-android-app.html
                    // ReadLine freeze on Unity5 && Windows!!
                    byte b = (byte)_serialPort.ReadByte();
                    while (b != 255 && _isRunning)
                    {
                        char c = (char)b;
                        if (c == '\n')
                        {
                            lock (_messages)
                            {
                                _messages.Enqueue(_data);
                                _data = "";
                            }
                        }
                        else
                        {
                            _data += c;
                        }
                        b = (byte)_serialPort.ReadByte();
                    }
                    //                    if (_serialPort.BytesToRead > 0)
                    //                    {
                    //                        lock(_messages)
                    //                        {
                    //                            string msg = _serialPort.ReadLine();
                    //                            _messages.Enqueue(msg);
                    //                        }
                    //                    }
                    //
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
                Thread.Sleep(1);
            }
        }
        #endregion

        #region Singleton
        static SerialHandler _instance;

        public static SerialHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject(typeof(SerialHandler).ToString());
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<SerialHandler>();
                }
                return _instance;
            }
        }
        #endregion
    }

}