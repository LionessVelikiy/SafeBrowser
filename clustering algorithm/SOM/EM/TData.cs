using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace EMSplit
{
    public class TData
    {
 
        public int N = 0;
        public int M = 0;
        public double[,] XX;
        public double[,] X;
        public string[] fields;
        public string[] items;
        public int[] clusterNum;

        public bool isClustered = false;

        public double[] Min, Max;

        public TData(string[] data)
        {
            ArrayList SData = new ArrayList();





            var tmpList = data[0].Split(';').ToList(); tmpList.RemoveAt(0);

            this.fields = tmpList.ToArray();
            this.items = new string[data.Length - 1];

            foreach (string line in data)
            {
                string[] SS = line.Split(';');


                double[] D = new double[SS.Length-1];
                if (M == 0)
                {
                    M++;
                    continue;
                }
                for (int n = 0; n < SS.Length; n++)
                {
                    if (n == 0)
                    {
                        this.items[M - 1] = SS[n];
                        continue;
                    }

                    if (SS[n] == "") continue;
                    SS[n] = SS[n].Replace(".", ",");
                    D[n-1] = double.Parse(SS[n]);
                }
                M++;
                SData.Add(D);
            }


            M--;
            clusterNum = new int[M];
            N = ((double[])SData[0]).Length;

            X = new double[M, N];
            XX = new double[M, N];
            Min = new double[N];
            Max = new double[N];


            for (int n = 0; n < N; n++)
            {
                Min[n] = double.MaxValue;
                Max[n] = double.MinValue;

                for (int m = 0; m < M; m++)
                {
                    X[m, n] = ((double[])SData[m])[n];
                    XX[m, n] = ((double[])SData[m])[n];
                    if (X[m, n] < Min[n])
                    {
                        Min[n] = X[m, n];
                    }

                    if (X[m, n] > Max[n])
                    {
                        Max[n] = X[m, n];
                    }
                }
            }

            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < M; m++)
                {
                    X[m, n] = (X[m, n] - Min[n]) / (Max[n] - Min[n]);
                }
            }
        }
    }
}
