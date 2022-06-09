using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryControlLevel
{
    public class cControlLevel
    {
        public string ID { get; set; }
        public string Password { get; set; }
        public int ControlLevel { get; set; }
        public List<cControlLevel> getAllControlLevel( string filename)
        {
            List<cControlLevel> cls = new List<cControlLevel>();
            try
            {
                string[] ss = File.ReadAllLines(filename);
                foreach (var x in ss)
                {
                    string[] ss2 = x.Split(':');
                    if (ss2.Length == 3)
                    {
                        cControlLevel cl = new cControlLevel();
                        cl.ID = ss2[0];
                        cl.Password = ss2[1];
                        cl.ControlLevel = int.Parse(ss2[2]);
                        cls.Add(cl);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return cls;
        }
        public void saveControlLevelToFile(List<cControlLevel> cls, string filename)
        {
            string s = "";
            foreach (var x in cls)
            {
                s += $"{x.ID}:{x.Password}:{x.ControlLevel}{Environment.NewLine}";
            }
            File.WriteAllText(filename,s);
        }
    }
}
