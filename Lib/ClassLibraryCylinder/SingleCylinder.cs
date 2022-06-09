using ClassLibraryTimer;
using ClassLibraryTRIG;

namespace ClassLibraryCylinder
{
    public class SingleCylinder
    {
        OnDelayTimer[] t1 = new OnDelayTimer[2];
        /// <summary>
        /// 氣缸狀態
        /// </summary>
        public bool inSolState { get; set; }
        /// <summary>
        /// Reset Error,例:復歸按鈕
        /// </summary>
        public bool inResetError { get; set; }
        /// <summary>
        /// 氣缸回LS
        /// </summary>
        public bool inBackSensor { get; set; }
        /// <summary>
        /// 氣缸出LS
        /// </summary>
        public bool inOutSensor { get; set; }
        /// <summary>
        /// interlock-回
        /// </summary>
        public bool inInterlockBack { get; set; }
        /// <summary>
        /// interlock-出
        /// </summary>
        public bool inInterlockOut { get; set; }
        /// <summary>
        /// 異常計時時間設定
        /// </summary>
        public int inTPCheckError { get; set; }
        /// <summary>
        /// 輸出氣缸
        /// </summary>
        public bool outSol { get; set; }
        /// <summary>
        /// 輸出異常-回
        /// </summary>
        public bool outErrorBack { get; set; }
        /// <summary>
        /// 輸出異常-出
        /// </summary>
        public bool outErrorOut { get; set; }
        public SingleCylinder()
        {
            t1[0] = new OnDelayTimer();
            t1[1] = new OnDelayTimer();
        }
        public void RunSetCylinder()
        {
            if (inInterlockOut)
            {
                outSol = true;
            }
        }
        public void RunResetCylinder()
        {
            if (inInterlockBack)
            {
                outSol = false;
            }
        }
        public void RunCheckError()
        {
            //back
            t1[0].inInterval = inTPCheckError;
            t1[0].inEN = !outSol && !inBackSensor;
            if (t1[0].outQ)
            {
                outErrorBack = true;
            }
            //out
            t1[1].inInterval = inTPCheckError;
            t1[1].inEN = outSol && !inOutSensor;
            if (t1[1].outQ)
            {
                outErrorOut = true;
            }
            //RESET
            if (inResetError)
            {
                outErrorBack = false;
                outErrorOut = false;
            }
        }
    }
}
