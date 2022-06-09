using EasyModbus;
using MachineLog;
using System;

namespace ClassLibrarySafty
{
    public class SickSafty
    {
        csLog mLog;
        ModbusClient _client;
        public delegate void ConnectHandler();
        public delegate void NoConnectHandler();
        public event ConnectHandler OnConnected;
        public event NoConnectHandler OnConnectError;
        private void fConnectError()
        {
            if (OnConnectError!=null)
            {
                OnConnectError();
            }
        }
        private void fConnect()
        {
            if (OnConnected!=null)
            {
                OnConnected();
            }
        }
        public bool  Connected { get; set; }
        public bool[] Inputs { get; set; }
        public bool[] Outputs { get; set; }
        public SickSafty(string ip,int inputport,int outputport)
        {
            mLog = new csLog("Sick");
            _client = new ModbusClient(ip, 502);
            Inputs = new bool[inputport*16];
            Outputs = new bool[outputport*16];
            mLog.LogText = "Class Sick Created!";
        }
        public void Read()
        {
            try
            {
                if (!_client.Connected)
                {
                    _client.Connect();
                }
                int[] its= _client.ReadHoldingRegisters(1100, 10);
                //input
                for (int i = 0; i < 16; i++)
                {
                    int j = its[1] >> i;
                    int k = 1;
                    int m = j & k;
                    if (m==1)
                    {
                        Inputs[i] = true;
                    }
                    else
                    {
                        Inputs[i] = false;
                    }
                }
                for (int i = 0; i < 16; i++)
                {
                    int j = its[2] >> i;
                    int k = 1;
                    int m = j & k;
                    if (m == 1)
                    {
                        Inputs[i+16] = true;
                    }
                    else
                    {
                        Inputs[i+16] = false;
                    }
                }
                //output
                for (int i = 0; i < 16; i++)
                {
                    int j = its[7] >> i;
                    int k = 1;
                    int m = j & k;
                    if (m == 1)
                    {
                        Outputs[i] = true;
                    }
                    else
                    {
                        Outputs[i] = false;
                    }
                }
                if (!Connected)
                {
                    fConnect();
                }
                Connected = true;
            }
            catch (Exception)
            {
                if (Connected)
                {
                    fConnectError();
                }
                Connected = false;
            }
        }
    }
}
