using ClassLibraryTimer;

namespace ClassLibraryBowlFeeder
{
    public class BowlFeeder
    {
        OnDelayTimer[] t1 = new OnDelayTimer[3];
        public int AutoFlow { get; set; }
        public bool inProgramRun { get; set; }
        public bool inFeederSensor { get; set; }
        public int inNoObjSetCount { get; set; }
        public int inYesObjSetCount { get; set; }
        public int inErrorObjSetCount { get; set; }
        public bool outErrorFeeder { get; set; }
        public bool outBowlFeederRY { get; set; }
        public BowlFeeder()
        {
            for (int i = 0; i < t1.Length; i++)
            {
                t1[i] = new OnDelayTimer();
            }
        }
        public void RunAutoFlow()
        {
            //*********************************************************************************
            //power on
            if (!inProgramRun)
            {
                AutoFlow = 0;
            }
            //idle
            if (AutoFlow==0 & inProgramRun)
            {
                AutoFlow = 1;
            }
            if (AutoFlow==3 && !outBowlFeederRY)
            {
                AutoFlow = 1;
            }
            t1[0].inInterval = inNoObjSetCount;
            t1[0].inEN = AutoFlow == 1 && !inFeederSensor;
            //RY on
            if (AutoFlow==1 & t1[0].outQ)
            {
                AutoFlow = 2;
            }
            t1[1].inInterval = inYesObjSetCount;
            t1[1].inEN = AutoFlow == 2 && outBowlFeederRY && inFeederSensor;//OK
            t1[2].inInterval = inErrorObjSetCount;
            t1[2].inEN = AutoFlow == 2 && outBowlFeederRY && !inFeederSensor;//NG
            if (t1[2].outQ)
            {
                outErrorFeeder = true;
            }
            else
            {
                outErrorFeeder = false;
            }
            //RY OFF
            if (AutoFlow == 2 & t1[1].outQ)
            {
                AutoFlow = 3;
            }
            //*********************************************************************************
            //output
            if (AutoFlow == 2)
            {
                outBowlFeederRY = true;
            }
            else
            {
                outBowlFeederRY = false;
            }
        }
    }
}
