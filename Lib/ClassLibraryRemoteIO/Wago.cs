using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyModbus;
using MachineLog;
using System.Threading;

namespace ClassLibraryRemoteIO
{
    public class Wago
    {
        int _numIn, _numOut;//input output的port數量
        csLog mLog;
        ModbusClient _Client;

        public delegate void ConnectWagoWarringHandler();
        public delegate void ConnectedHandler();
        public event ConnectWagoWarringHandler OnConnectWagoWarring;
        public event ConnectedHandler OnConnected;
        private void fConnectWagoWarring()
        {
            if (OnConnectWagoWarring != null)
            {
                OnConnectWagoWarring();
            }
        }
        private void fConnected()
        {
            if (OnConnected!=null)
            {
                OnConnected();
            }
        }

        #region prop
        public bool Connected { get; set; }
        /// <summary>
        /// input狀態
        /// </summary>
        public bool[] Inputs { get; set; }
        /// <summary>
        /// output狀態
        /// </summary>
        public bool[] Outputs { get; set; }
        /// <summary>
        /// 提供外部寫入控制output狀態
        /// </summary>
        public bool[] dirOutputs { get; set; }
        /// <summary>
        /// input狀態
        /// </summary>
        public int[] InputRegisters
        {
            get
            {
                int[] ivs = new int[_numIn];
                for (int i = 0; i < _numIn; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        int k = Convert.ToInt16(Inputs[i * 16 + j]);
                        if (k == 0)
                        {
                            k = 1;
                            k = k << j;
                            k = ~k;
                            ivs[i] = ivs[i] & k;
                        }
                        else
                        {
                            k = k << j;
                            ivs[i] = ivs[i] | k;
                        }
                    }
                }
                return ivs;
            }
        }
        /// <summary>
        /// output狀態
        /// </summary>
        public int[] OutputRegisters
        {
            get
            {
                int[] ivs = new int[_numOut];
                for (int i = 0; i < _numOut; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        int k = Convert.ToInt16(Outputs[i * 16 + j]);
                        if (k == 0)
                        {
                            k = 1;
                            k = k << j;
                            k = ~k;
                            ivs[i] = ivs[i] & k;
                        }
                        else
                        {
                            k = k << j;
                            ivs[i] = ivs[i] | k;
                        }
                    }
                }
                return ivs;
            }
        }
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="CountPortOfInput">input Port數量</param>
        /// <param name="CountPortOfOutput">output Port數量</param>
        public Wago(int CountPortOfInput, int CountPortOfOutput, string ip, int port)
        {
            mLog = new csLog("WagoLog");
            _numIn = CountPortOfInput;
            _numOut = CountPortOfOutput;

            Inputs = new bool[_numIn * 16];
            Outputs = new bool[_numOut * 16];
            dirOutputs = new bool[_numOut * 16];

            _Client = new ModbusClient(ip, port);
        }
        /// <summary>
        /// 讀取remote io
        /// </summary>
        public void readRemoteIO()
        {
            try
            {
                if (!_Client.Connected)
                {
                    _Client.Connect();
                }
                //input
                Inputs = _Client.ReadCoils(0x0, Inputs.Length);
                //output
                Outputs = _Client.ReadCoils(0x200, Outputs.Length);
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
                    fConnectWagoWarring();
                }
                Connected = false;
            }
        }
        /// <summary>
        /// 寫入remote io
        /// </summary>
        public void writeRemoteIO()
        {
            try
            {
                if (!_Client.Connected)
                {
                    _Client.Connect();
                }
                _Client.WriteMultipleCoils(0x200, dirOutputs);
                if (!Connected)
                {
                    fConnected();
                }
            }
            catch (Exception)
            {
                if (Connected)
                {
                    fConnectWagoWarring();
                }
            }
        }
       
    }
}
