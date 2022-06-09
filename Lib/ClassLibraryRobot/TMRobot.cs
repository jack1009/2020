using EasyModbus;
using MachineLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClassLibraryRobot
{
    public class TMRobot
    {
        ModbusClient _Client;
        csLog mLog;

        public delegate void ConnectTMWarringHandler();
        public delegate void ConnectedHandler();
        public delegate void StatusErrorHandler();
        public delegate void StatusRunningHandler();
        public delegate void StatusEditingHandler();
        public delegate void StatusPauseHandler();
        public delegate void StatusEStopHandler();
        public event ConnectTMWarringHandler OnConnectWarring;
        public event ConnectedHandler OnConnected;
        public event StatusErrorHandler OnStatusError;
        public event StatusRunningHandler OnStatusRunning;
        public event StatusEditingHandler OnStatusEditing;
        public event StatusPauseHandler OnStatusPause;
        public event StatusEStopHandler OnStatusEStop;
        private void fConnectTMWarring()
        {
            if (OnConnectWarring != null)
            {
                OnConnectWarring();
            }
        }
        private void fConnected()
        {
            if (OnConnected != null)
            {
                OnConnected();
            }
        }
        private void fStatusError()
        {
            if (OnStatusError!=null)
            {
                OnStatusError();
            }
        }
        private void fStatusRunning()
        {
            if (OnStatusRunning!=null)
            {
                OnStatusRunning();
            }
        }
        private void fStatusEditing()
        {
            if (OnStatusEditing!=null)
            {
                OnStatusEditing();
            }
        }
        private void fStatusPause()
        {
            if (OnStatusPause!=null)
            {
                OnStatusPause();
            }
        }
        private void fStatusEStop()
        {
            if (OnStatusEStop!=null)
            {
                OnStatusEStop();
            }
        }
        
        /// <summary>
        /// 建構TM
        /// </summary>
        /// <param name="IPAddress">modbus ip address</param>
        /// <param name="port">502 port</param>
        /// <param name="userDIPortNum">自定義DI的PORT數</param>
        public TMRobot(string IPAddress,int port,int userDIPortNum)
        {
            mLog = new csLog("TMLog");
            _Client = new ModbusClient(IPAddress, port);
            EndmoduleAI0 = new float[2];
            UserDefineTMCommand = new int[70];
            UserDefineDIPort = new int[userDIPortNum*16];
            dirUserDefineDIPort = new int[userDIPortNum];
            UserDefineUpperCommand = new int[20];
            dirUserDefineUpperCommand = new int[20];
        }

        #region prop
        public bool Connected { get; set; }
        public bool StatusError { get; set; }
        public bool StatusProjectRunning { get; set; }
        public bool StatusProjectEditing { get; set; }
        public bool StatusProjectPause { get; set; }
        public bool StatusGetControl { get; set; }
        public bool StatusLight { get; set; }
        public bool StatusUserConnectedExternalSafeguardInputPort { get; set; }
        public bool StatusEStop { get; set; }
        public bool EndmoduleDI0 { get; set; }
        public bool EndmoduleDI1 { get; set; }
        public bool EndmoduleDI2 { get; set; }
        public bool EndmoduleDO0 { get; set; }
        public bool EndmoduleDO1 { get; set; }
        public bool EndmoduleDO2 { get; set; }
        public bool EndmoduleDO3 { get; set; }
        public float[] EndmoduleAI0 { get; set; }
        /// <summary>
        /// 當前程式名稱
        /// </summary>
        public string CurrentProject { get; set; }
        public string ScanedBarcode { get; set; }
        /// <summary>
        /// 自定義TM命令,9550#70
        /// </summary>
        public int[] UserDefineTMCommand { get; set; }
        /// <summary>
        /// 自定義DI PORT,9000#4
        /// </summary>
        public int[] UserDefineDIPort { get; set; }
        /// <summary>
        /// 寫入自定義DI PORT,9000#4
        /// </summary>
        public int[] dirUserDefineDIPort { get; set; }
        /// <summary>
        /// 自定義上位命令,9050#20
        /// </summary>
        public int[] UserDefineUpperCommand { get; set; }
        /// <summary>
        /// 寫入自定義上位命令,9050#20
        /// </summary>
        public int[] dirUserDefineUpperCommand { get; set; }
        #endregion
        private bool[] oldStatus = new bool[8];
        public void readTMRobot()
        {
            try
            {
                if (!_Client.Connected && _Client!=null)
                {
                    _Client.Connect();
                }
                //robot status
                bool[] bs = _Client.ReadDiscreteInputs(7201, 8);
                StatusError = bs[0];
                StatusProjectRunning = bs[1];
                StatusProjectEditing = bs[2];
                StatusProjectPause = bs[3];
                StatusGetControl = bs[4];
                StatusLight = bs[5];
                StatusUserConnectedExternalSafeguardInputPort = bs[6];
                StatusEStop = bs[7];
                //***********************異常事件***************************
                if (StatusError && !oldStatus[0])
                {
                    fStatusError();
                }
                if (StatusProjectRunning && !oldStatus[1])
                {
                    fStatusRunning();
                }
                if (StatusProjectEditing && !oldStatus[2])
                {
                    fStatusEditing();
                }
                if (StatusProjectPause && !oldStatus[3])
                {
                    fStatusPause();
                }
                if (StatusEStop && !oldStatus[7])
                {
                    fStatusEStop();
                }
                //***********************異常事件***************************
                oldStatus = bs;
                //End Module
                bs = _Client.ReadDiscreteInputs(800, 3);
                EndmoduleDI0 = bs[0];
                EndmoduleDI1 = bs[1];
                EndmoduleDI2 = bs[2];
                bs = _Client.ReadCoils(800, 4);
                EndmoduleDO0 = bs[0];
                EndmoduleDO1 = bs[1];
                EndmoduleDO2 = bs[2];
                EndmoduleDO3 = bs[3];
                int[] its = _Client.ReadInputRegisters(800, 2);
                EndmoduleAI0[0] = float.Parse(its[0].ToString());
                EndmoduleAI0[1] = float.Parse(its[1].ToString());
                //current project
                its = _Client.ReadInputRegisters(7701, 99);
                List<byte> ltbs = new List<byte>();
                foreach (var x in its)
                {
                    int iconv = 0xFF;
                    int ii = iconv & x;
                    int ii2 = x >> 8;
                    ltbs.Add((byte)ii2);
                    ltbs.Add((byte)ii);
                }
                byte[] bts = ltbs.ToArray();
                CurrentProject = Encoding.ASCII.GetString(bts);
                //******************************************自定義****************************************
                //DI PORT
                UserDefineDIPort = _Client.ReadHoldingRegisters(9000, UserDefineDIPort.Length);
                //上位=>TM add.9050,20個
                UserDefineUpperCommand = _Client.ReadHoldingRegisters(9050, 20);
                //TM=>上位 add.9550,70個
                UserDefineTMCommand = _Client.ReadHoldingRegisters(9550, 70);
                //barcode
                its = _Client.ReadHoldingRegisters(9596, 16);
                ltbs = new List<byte>();
                foreach (var x in its)
                {
                    int iconv = 0xFF;
                    int ii = iconv & x;
                    int ii2 = x >> 8;
                    ltbs.Add((byte)ii2);
                    ltbs.Add((byte)ii);
                }
                bts = ltbs.ToArray();
                ScanedBarcode = Encoding.ASCII.GetString(bts);
                if (!Connected)
                {
                    fConnected();
                }
                Connected = true;
            }
            catch (Exception)
            {
                if (Connected)
                {
                    fConnectTMWarring();
                }
                Connected = false;
            }
        }
        public void writeTMRobot()
        {
            try
            {
                if (!_Client.Connected)
                {
                    _Client.Connect();
                }
                //user define input
                _Client.WriteMultipleRegisters(9000, dirUserDefineDIPort);
                //user define command
                _Client.WriteMultipleRegisters(9050, dirUserDefineUpperCommand);
                if (!Connected)
                {
                    fConnected();
                }
            }
            catch (Exception)
            {
                if (Connected)
                {
                    fConnectTMWarring();
                }
            }
        }
        
        /// <summary>
        /// Set or Reset PcCommand
        /// </summary>
        /// <param name="index">PcCommand指標</param>
        public void SRUserDefineCommand(int index,int command)
        {
            UserDefineUpperCommand[index] = command;
        }
        /// <summary>
        /// 切換Project
        /// </summary>
        /// <param name="ProjectName">專案名稱</param>
        public void setProjectName(string ProjectName)
        {
            string s = ProjectName + @"\0";
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            List<int> lis = new List<int>();
            foreach (var x in bytes)
            {
                lis.Add(x);
            }
            int[] ints = lis.ToArray();
            _Client.WriteMultipleRegisters(7701, ints);
        }
        
    }
}
