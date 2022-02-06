using EMSplit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SOM.KM
{
    class TKM
    {

        public TKM(TData td)
        {

            IClusterization<double> clusterization = new KMeans(2, new EuclideanDistance());


            List<DataItem<double>> data = new List<DataItem<double>>();
            //формируем множество точек
            for (int m = 0; m < td.M; m++)
            {
                double[] line = new double[td.N];
                for (int n = 0; n < td.N; n++)
                {
                    line[n] = td.XX[m,n];
                }
                data.Add(new DataItem<double>(line, null));
            }

            //РАСПРЕДЕЛЯЕМ ТОЧКИ РАНДОМНО
            ClusterizationResult<double> c = clusterization.MakeClusterization(data);

        
            this.matrix = getHashMatrix(td);
            int mNum = 0;

            //ПЕРЕВЫЧИСЛЯЕМ ЦЕНТР КАЖДОГО КЛАСТЕРА 
            for(int k = 0; k< c.Clusterization.Count; k++)
            {

                for(int i =0; i< c.Clusterization.Values.ElementAt(k).Count; i++)
                {

                    double[] line = new double[td.N];
                    for(int j=0; j< c.Clusterization.Values.ElementAt(k)[i].Input.Length; j++)
                    {
                        line[j] = c.Clusterization.Values.ElementAt(k)[i].Input[j];
                    }


                    //ПЕРЕРАСПРЕДЕЛЯЕМ ТОЧКИ
                    td.clusterNum[matrix.IndexOf(getHash(line))] = k;
                    mNum++;
                }
              
                
            }


            foreach (var centroid in c.Clusterization)
            {

                foreach (var t in centroid.Value)
                {
                    Console.WriteLine(t.Input[0] + ":" + t.Input[1]);
                }
          
            }

            //ПОСТРОИТЬ ГРАФИК
            Charts ch = new Charts(td);
            ch.WindowState = WindowState.Maximized;
            ch.Show();
        }

        public List<string> matrix = new List<string>();


        public List<string>  getHashMatrix(TData td)
        {

            List<string> matrix = new List<string>();
            for (int i =0; i< td.M; i++)
            {
                double[] line = new double[td.N];
                for (int j = 0; j < td.N; j++)
                {
                    line[j] = td.XX[i, j];
                }
                matrix.Add(getHash(line));
            }
           
            return matrix;
        }


        public string getHash(double[] line)
        {
            string hash = "";

            foreach(double d in line)
            {
                hash += d.ToString();
            }

            return hash;
        }


    }












    internal class KMeans : IClusterization<double>
    {

        IMetrics<double> _metrics;
        int _clusterCount;

        int _maxIterations = 10000;

        internal KMeans(int clusterCount, IMetrics<double> metrics)
        {
            _clusterCount = clusterCount;
            _metrics = metrics;


        }






        //ФОРМИРУЕМ КЛАСТЕРЫ
        public ClusterizationResult<double> MakeClusterization(IList<DataItem<double>> data)
        {

            Dictionary<double[], IList<DataItem<double>>> clusterization = new Dictionary<double[], IList<DataItem<double>>>();

            Random r = new Random();
            double[] min = new double[data.First().Input.Length];
            double[] max = new double[data.First().Input.Length];

            for (int i = 0; i < data.First().Input.Length; i++)
            {
                min[i] = (from d in data
                          select d.Input[i]).Min();
                max[i] = (from d in data
                          select d.Input[i]).Max();
            }
            for (int i = 0; i < _clusterCount; i++)
            {
                double[] v = new double[data.First().Input.Length];
                for (int j = 0; j < data.First().Input.Length; j++)
                {
                    v[j] = min[j] + r.NextDouble() * Math.Abs(max[j] - min[j]);
                }
                clusterization.Add(v, new List<DataItem<double>>());
            }




            bool convergence = true;
            double lastCost = Double.MaxValue;
            int iterations = 0;
            while (true)
            {
                convergence = true;






                //
                foreach (DataItem<double> item in data)
                {
                    var candidates = from v in clusterization.Keys
                                     select new
                                     {
                                         Dist = _metrics.Calculate(v, item.Input),
                                         Cluster = v
                                     };
                    double minDist = (from c in candidates
                                      select c.Dist).Min();
                    var minCandidates = from c in candidates
                                        where c.Dist == minDist
                                        select c.Cluster;
                    double[] key = minCandidates.First();
                    clusterization[key].Add(item);
                }



                double cost = 0;
                List<double[]> newMeans = new List<double[]>();
                foreach (double[] key in clusterization.Keys)
                {
                    double[] v = new double[key.Length];
                    if (clusterization[key].Count > 0)
                    {
                        v = _metrics.GetCentroid((from x in clusterization[key]
                                                  select x.Input).ToArray());
                        cost += (from d in clusterization[key]
                                 select Math.Pow(_metrics.Calculate(key, d.Input), 2)).Sum();
                    }
                    else
                    {
                        for (int j = 0; j < data.First().Input.Length; j++)
                        {
                            v[j] = min[j] + r.NextDouble() * Math.Abs(max[j] - min[j]);
                        }
                    }
                    newMeans.Add(v);
                }


                Console.WriteLine(lastCost + "________" + cost);
              /*  if (lastCost <= cost)
                {
                    break;
                }*/


                iterations++;
                if (iterations == _maxIterations)
                {
                    break;
                }


                lastCost = cost;

                clusterization.Clear();
                foreach (double[] mean in newMeans)
                {
                    clusterization.Add(mean, new List<DataItem<double>>());
                }

            }




            return new ClusterizationResult<double>()
            {
                Centroids = new List<double[]>(clusterization.Keys),
                Clusterization = clusterization,
                Cost = lastCost
            };

        }
    }






    //АЛГОРИТМ ПЕРЕСЧЕТА РАССТОЯНИЯ МЕЖДУ ТОЧКАМИ
    public interface IMetrics<T>
    {
        double Calculate(T[] v1, T[] v2);

        T[] GetCentroid(IList<T[]> data);
    }

    public abstract class MetricsBase<T> : IMetrics<T>
    {
        public abstract double Calculate(T[] v1, T[] v2);

        public virtual T[] GetCentroid(IList<T[]> data)
        {
            if (data == null)
            {
                throw new ArgumentException("Data is null");
            }
            if (data.Count == 0)
            {
                throw new ArgumentException("Data is empty");
            }
            double[][] dist = new double[data.Count][];
            for (int i = 0; i < data.Count - 1; i++)
            {
                dist[i] = new double[data.Count];
                for (int j = i; j < data.Count; j++)
                {
                    if (i == j)
                    {
                        dist[i][j] = 0;
                    }
                    else
                    {
                        dist[i][j] = Math.Pow(Calculate(data[i], data[j]), 2);
                        if (dist[j] == null)
                        {
                            dist[j] = new double[data.Count];
                        }
                        dist[j][i] = dist[i][j];
                    }
                }
            }
            double minSum = Double.PositiveInfinity;
            int bestIdx = -1;
            for (int i = 0; i < data.Count; i++)
            {
                double dSum = 0;
                for (int j = 0; j < data.Count; j++)
                {
                    dSum += dist[i][j];
                }
                if (dSum < minSum)
                {
                    minSum = dSum;
                    bestIdx = i;
                }
            }

            return data[bestIdx];
        }
    }






    
    internal class EuclideanDistance : MetricsBase<double>
    {

        internal EuclideanDistance()
        {
        }

        #region IMetrics Members

        public override double Calculate(double[] v1, double[] v2)
        {
            if (v1.Length != v2.Length)
            {
                throw new ArgumentException("Vectors dimensions are not same");
            }
            if (v1.Length == 0 || v2.Length == 0)
            {
                throw new ArgumentException("Vector dimension can't be 0");
            }
            double d = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                d += (v1[i] - v2[i]) * (v1[i] - v2[i]);
            }
            return Math.Sqrt(d);
        }

        public override double[] GetCentroid(IList<double[]> data)
        {
            if (data.Count == 0)
            {
                throw new ArgumentException("Data is empty");
            }
            double[] mean = new double[data.First().Length];
            for (int i = 0; i < mean.Length; i++)
            {
                mean[i] = 0;
            }
            foreach (double[] item in data)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    mean[i] += item[i];
                }
            }
            for (int i = 0; i < mean.Length; i++)
            {
                mean[i] = mean[i] / data.Count;
            }
            return mean;
        }

        #endregion
    }

    public interface IClusterization<T>
    {
        ClusterizationResult<T> MakeClusterization(IList<DataItem<T>> data);
    }
    public class ClusterizationResult<T>
    {
        public IList<T[]> Centroids { get; set; }
        public IDictionary<T[], IList<DataItem<T>>> Clusterization { get; set; }
        public double Cost { get; set; }
    }

    public class DataItem<T>
    {

        private T[] _input = null;
        private T[] _output = null;

        public DataItem(T[] input, T[] output)
        {
            _input = input;
            _output = output;
        }

        public T[] Input
        {
            get { return _input; }
        }

        public T[] Output
        {
            get { return _output; }
            set { _output = value; }
        }

    }


}
