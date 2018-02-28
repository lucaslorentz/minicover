using System;
using System.Collections.Generic;
using System.IO;

namespace MiniCover
{
    public static class HitService
    {
        private static readonly object lockObject = new object();

        private static Dictionary<string, Dictionary<int, int>> dctWritersIdCount = new Dictionary<string, Dictionary<int, int>>();      

        /// <summary>
        /// only have to save if a new hit is recorded (to avoid saving twice)
        /// </summary>
        private static bool isANewHitRecorded = false;        

        /// <summary>
        /// summary: 
        /// Init - End - Hit - Hit - Hit... - End => fast (usingSlowMode = false)
        /// Init - Hit - Hit - Hit...             => slow (usingSlowMode = true)
        /// 
        /// To go fast, some dll has to be outside the coverage test, this dlls are the "End" because
        /// they are the starter and finisher to save the middle dll that are under code coverage
        /// The save action is done in the outside dll's, the register action are in the inside dll
        /// To determine who is inside or outside, the parameter source is used, the source indicate
        /// the inside dll's.
        /// But it everything is inside, no ones save the state
        /// this variable start indicating that everything is inside, so in each step has to be saved
        /// When a End is called, implies than a outside dll's is alive and the fast version come to live
        /// </summary>
        private static bool isEverythingInsideTesting = true;

        /// <summary>
        /// the slow mode indicated by previous parameter is active and it will remain active
        /// </summary>
        private static bool usingSlowMode = false;

        public static void Init(string fileName)
        {
            lock (lockObject)
            {
                if (!dctWritersIdCount.ContainsKey(fileName))
                {
                    if (File.Exists(fileName)) 
                    {
                        // a previous test was found
                        dctWritersIdCount.Add(fileName, new Dictionary<int, int>());
                        foreach (string line in File.ReadAllLines(fileName))
                        {
                            string[] parts = line.Split(' ');
                            dctWritersIdCount[fileName].Add(int.Parse(parts[0]), int.Parse(parts[1]));
                        };
                        
                    }
                    else 
                    {
                        dctWritersIdCount.Add(fileName, new Dictionary<int, int>());
                    }
                }
            }
        }

        public static void Hit(string fileName, int id)
        {
            lock (lockObject)
            {   
                isANewHitRecorded = true;
                if (dctWritersIdCount[fileName].ContainsKey(id)) 
                {
                    dctWritersIdCount[fileName][id]++;
                } 
                else 
                {
                    dctWritersIdCount[fileName].Add(id, 1);
                }

                if (isEverythingInsideTesting) 
                {
                    usingSlowMode = true; 
                    Save(fileName);
                }
            }
        }

        public static void End(string fileName, int id)
        {
            if (usingSlowMode)
                return;

            lock (lockObject)
            {                              
                isEverythingInsideTesting = false;  
                if (isANewHitRecorded) 
                {
                   Save(fileName);
                }                
            }
        }

        private static void Save(string fileName) 
        {
            using (StreamWriter sw = new StreamWriter(File.Open(fileName, FileMode.Create))) {
                foreach (KeyValuePair<int, int> kvp in dctWritersIdCount[fileName]) {
                    sw.WriteLine($"{kvp.Key} {kvp.Value}");
                }
            }
        }
    }
}
