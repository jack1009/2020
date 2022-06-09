using System;
using System.IO.Ports;
using System.Threading;

namespace ClassLibraryScrewDriver
{
    public class StanleyDriver
    {
        /// <summary>
        /// Create Class
        /// </summary>
        public StanleyDriver(string comPort,int baudRate,Parity parity,int dataBits,StopBits stopBits)
        {
            InitialComPort(comPort, baudRate, parity, dataBits, stopBits);
        }
        #region Com Port
        private SerialPort mSerialPort;
        /// <summary>
        /// 初始化com port
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        private void InitialComPort(string comPort, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            mSerialPort = new SerialPort(comPort, baudRate, parity, dataBits, stopBits);
            mSerialPort.DataReceived += MSerialPort_DataReceived;
            mSerialPort.Close();
            Thread.Sleep(1000);
            //mSerialPort.Open();
        }
        /// <summary>
        /// 資料讀入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
            Byte[] buffer = new Byte[1024];
            Int32 length = (sender as SerialPort).Read(buffer, 0, buffer.Length);
            Array.Resize(ref buffer, length);
            string ss = System.Text.Encoding.ASCII.GetString(buffer);
            //HMI
            string[] ss1 = ss.Split(',');
            if (ss1.Length >= 9)
            {
                SpindleNumber = ss1[0];
                JobNumber = ss1[1];
                TorqueValue = ss1[2];
                TorqueJudgement = ss1[3];
                AngleValue = ss1[4];
                AngleJudgement = ss1[5];
                OverrallJudgement = ss1[6];
                ScrewDateTime = ss1[7];
                Barcode = ss1[8];
                if (On232DataReceived!=null)
                {
                    On232DataReceived();
                }
            }
        }

        public delegate void RS232DataReceivedHandler();
        public event RS232DataReceivedHandler On232DataReceived;
        public void ChangeDriverJob(string jobName)
        {
            mSerialPort.Write(jobName);
        }
        #endregion

        #region 屬性
        /// <summary>
        /// 軸號
        /// </summary>
        public string SpindleNumber { get; set; }
        /// <summary>
        /// 工作名稱
        /// </summary>
        public string JobNumber { get; set; }
        /// <summary>
        /// 鎖付扭力
        /// </summary>
        public string TorqueValue { get; set; }
        /// <summary>
        /// 鎖付扭力判定
        /// </summary>
        public string TorqueJudgement { get; set; }
        /// <summary>
        /// 鎖付角度
        /// </summary>
        public string AngleValue { get; set; }
        /// <summary>
        /// 鎖付角度判定
        /// </summary>
        public string AngleJudgement { get; set; }
        /// <summary>
        /// 總合判定
        /// </summary>
        public string OverrallJudgement { get; set; }
        /// <summary>
        /// 鎖付日期時間
        /// </summary>
        public string ScrewDateTime { get; set; }
        /// <summary>
        /// 條碼32字元
        /// </summary>
        public string Barcode { get; set; } 
        #endregion
    }
}
