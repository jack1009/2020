using ClassLibraryTimer;
using ClassLibraryTRIG;

namespace ClassLibraryLinerFeeder
{
    public class LinerFeeder
    {
        OnDelayTimer[] TON = new OnDelayTimer[5];
        F_TRIG fTrig = new F_TRIG();
        F_TRIG fTrig2 = new F_TRIG();
        public delegate void FeedErrorHandler();
        public event FeedErrorHandler OnFeedError;
        private void fFeedError()
        {
            if (OnFeedError!=null)
            {
                OnFeedError();
            }
        }
        public int AutoFlow { get; set; }
        /// <summary>
        /// AutoStatus
        /// </summary>
        public bool inProgramRun { get; set; }
        /// <summary>
        /// Reset PB
        /// </summary>
        public bool inResetError { get; set; }
        /// <summary>
        /// Error Status
        /// </summary>
        public bool inErrorStatus { get; set; }
        /// <summary>
        /// 無料時間設定
        /// </summary>
        public int inTPNoObject { get; set; }
        /// <summary>
        /// 有料延時時間設定
        /// </summary>
        public int inTPObjectExist { get; set; }
        /// <summary>
        ///進料檢查
        /// </summary>
        public bool inSensor { get; set; }
        /// <summary>
        /// 平送振動
        /// </summary>
        public bool outLinerRY { get; set; }
        /// <summary>
        /// 自動時SET SOL,執行後上位RESET
        /// </summary>
        public bool outSetSol { get; set; }
        /// <summary>
        /// 自動時RESET SOL,執行後上位RESET
        /// </summary>
        public bool outResetSol { get; set; }
        /// <summary>
        /// 氣缸LS異常
        /// </summary>
        public bool  outFeedError { get; set; }
        public LinerFeeder()
        {
            for (int i = 0; i < TON.Length; i++)
            {
                TON[i] = new OnDelayTimer();
            }
        }
        public void RunAutoFlow()
        {
            fTrig2.inClk = inProgramRun;
            fTrig2.RunCheckState();
            if (fTrig2.outQ)
            {
                outSetSol = false;
            }
            //異常處理
            fTrig.inClk = inErrorStatus;
            if (fTrig.outQ)
            {
                outSetSol = false;
            }
            //power on=0
            if (!inProgramRun || inErrorStatus)
            {
                AutoFlow = 0;
            }
            //idle=1
            if (AutoFlow==0 && inProgramRun && !inErrorStatus)
            {
                outSetSol = false;
                outResetSol = false;
                AutoFlow = 1;
            }
            if (AutoFlow==6 && !outLinerRY)
            {
                outResetSol = false;
                AutoFlow = 1;
            }
            //無料起動平送起振=2
            TON[0].inInterval = inTPNoObject;
            TON[0].inEN = AutoFlow == 1 && !inSensor;
            if (AutoFlow==1 && TON[0].outQ)
            {
                AutoFlow = 2;
            }
            if (AutoFlow==9999 && !outFeedError)
            {
                AutoFlow = 2;
            }
            //節流出=3
            TON[1].inInterval = 8;
            TON[1].inEN = AutoFlow == 2 && outLinerRY;
            if (AutoFlow==2 && TON[1].outQ)
            {
                outSetSol = true;
                AutoFlow = 3;
            }
            //有料OK=4
            TON[2].inInterval = inTPObjectExist;
            TON[2].inEN = AutoFlow == 3 && outSetSol;//有料
            if (AutoFlow==3 && TON[2].outQ)
            {
                outSetSol = false;
                AutoFlow = 4;
            }
            if (AutoFlow==9999 && !outFeedError && inSensor)
            {
                outSetSol = false;
                AutoFlow = 4;
            }
            //節流回=5
            if (AutoFlow==4)
            {
                outResetSol = true ;
                AutoFlow = 5;
            }
            //平送停振=6
            TON[4].inInterval = 10;
            TON[4].inEN = AutoFlow == 5 && !outSetSol && outResetSol;
            if (AutoFlow == 5 && inSensor && TON[4].outQ)
            {
                AutoFlow = 6;
            }
            //送料NG=9999
            if (AutoFlow == 5 && !inSensor && TON[4].outQ)
            {
                outResetSol = false;
                outFeedError = true;
                fFeedError();
                AutoFlow = 9999;
            }
            if (inResetError)
            {
                outFeedError = false;
            }
            //********************************************out
            if (AutoFlow>1 && AutoFlow<6)
            {
                outLinerRY = true;
            }
            else
            {
                outLinerRY = false;
            }
        }
    }
}
