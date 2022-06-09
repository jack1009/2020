using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace ClassLibraryScrewDriver
{
    public class IrDriver
    {
        SerialPort Sp1;
        #region prop
        public string CycleNo { get; set; }
        public string ConfigNo { get; set; }
        public string Date { get; set; }
        public string TimeID { get; set; }
        public string CycleResult { get; set; }
        public string PeakTorque { get; set; }
        public string TorqueResult { get; set; }
        public string TorqueUnits { get; set; }
        public string PeakAngle { get; set; }
        public string AngleResult { get; set; }
        public string PeakCurrent { get; set; }
        public string CycleTime { get; set; }
        public string StrategyType { get; set; }
        public string TorqueHighLimit { get; set; }
        public string TorqueLowLimit { get; set; }
        public string AngleHighLimit { get; set; }
        public string AngleLowLimit { get; set; }
        public string ControlPoint { get; set; }
        public string Barcode { get; set; }
        #endregion
        #region event
        public delegate void DataReceivedCallBack();
        public event DataReceivedCallBack OnDataReceived;
        private void fDataReceived()
        {
            if (OnDataReceived != null)
            {
                OnDataReceived();
            }
        }
        #endregion

        public IrDriver(string comPort, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            InitialComPort(comPort, baudRate, parity, dataBits, stopBits);
        }
        private void InitialComPort(string comPort, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            Sp1 = new SerialPort(comPort, baudRate, parity, dataBits, stopBits);
            Sp1.DataReceived += Sp1_DataReceived;
            Sp1.Close();
            Thread.Sleep(1000);
            Sp1.Open();
        }

        private void Sp1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
            string s = Sp1.ReadLine();
            string[] ss = s.Split(',');
            if (ss.Length>=24)
            {
                //CycleNo = ss[0];
                //CycleNo = ss[1];
                //CycleNo = ss[2];
                CycleNo = ss[3];
                ConfigNo = ss[4];
                Date = ss[5];
                TimeID = ss[6];
                CycleResult = ss[7];
                PeakTorque = ss[8];
                TorqueResult = ss[9];
                TorqueUnits = ss[10];
                PeakAngle = ss[11];
                AngleResult = ss[12];
                //PeakCurrent = ss[13];
                PeakCurrent = ss[14];
                CycleTime = ss[15];
                StrategyType = ss[16];
                TorqueHighLimit = ss[17];
                TorqueLowLimit = ss[18];
                AngleHighLimit = ss[19];
                AngleLowLimit = ss[20];
                ControlPoint = ss[21];
                Barcode = ss[22];
                //ConfigNo = ss[23];
            }
            fDataReceived();
        }

    }
}
