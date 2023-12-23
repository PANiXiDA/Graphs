using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

// неориентированный граф -  Граф, ни одному ребру которого не присвоено направление
// взвешенный граф - граф, в котором между вершинами есть расстояние

namespace GraphsTeory
{
    public class Graph
    {
        public Dictionary<string, Dictionary<string, double>> MainGraph = new Dictionary<string, Dictionary<string, double>>(); //словарь для графа
        public Dictionary<int, string> Sopostavitel = new Dictionary<int, string>();
        public Dictionary<string, int> DeSopostavitel = new Dictionary<string, int>();
        public Dictionary<string, int> nov = new Dictionary<string, int>(); //словарь для вершин, true = 0, false = 1
        public string type;
        public Graph() { }
        public Graph(string type) { this.type = type; }
        public Graph(string Path, string type)// path - путь к файлу, type - тип графа
        {
            using (StreamReader file = new StreamReader(Path))
            {
                try
                {
                    this.type = type;
                    if (type == "t1")
                    {
                        while (!file.EndOfStream)
                        {
                            string Point = file.ReadLine();
                            Dictionary<string, double> Link = new Dictionary<string, double>();
                            string LinkString = file.ReadLine();
                            if (String.IsNullOrEmpty(LinkString))
                            {
                                MainGraph.Add(Point, Link);
                                nov.Add(Point, 0);
                            }
                            else
                            {
                                char[] c = { ' ' };
                                List<string> Points = LinkString.Split(c, StringSplitOptions.RemoveEmptyEntries).ToList(); //разбиваем строку на подстроки по разделителю
                                for (int i = 0; i < Points.Count; i++)
                                {
                                    Link.Add(Points[i], 1);
                                }
                                MainGraph.Add(Point, Link);
                                nov.Add(Point, 0);
                            }
                        }
                    }
                    if (type == "t2")
                    {
                        while (!file.EndOfStream)
                        {
                            string Point = file.ReadLine();
                            Dictionary<string, double> Link = new Dictionary<string, double>();
                            string LinkString = file.ReadLine();
                            if (String.IsNullOrEmpty(LinkString))
                            {
                                MainGraph.Add(Point, Link);
                                nov.Add(Point, 0);
                            }
                            else
                            {
                                char[] c = { ' ' };
                                List<string> PointsAndWeights = LinkString.Split(c, StringSplitOptions.RemoveEmptyEntries).ToList();
                                for (int i = 0; i < PointsAndWeights.Count; i += 2)
                                {
                                    Link.Add(PointsAndWeights[i], double.Parse(PointsAndWeights[i + 1]));
                                }
                                MainGraph.Add(Point, Link);
                                nov.Add(Point, 0);
                            }
                        }
                    }
                    if (type == "t3")
                    {
                        while (!file.EndOfStream)
                        {
                            string Point = file.ReadLine();
                            Dictionary<string, double> Link = new Dictionary<string, double>();
                            string LinkString = file.ReadLine();
                            if (String.IsNullOrEmpty(LinkString))
                            {
                                MainGraph.Add(Point, Link);
                                nov.Add(Point, 0);
                            }
                            else
                            {
                                char[] c = { ' ' };
                                List<string> Points = LinkString.Split(c, StringSplitOptions.RemoveEmptyEntries).ToList();
                                for (int i = 0; i < Points.Count; i++)
                                {
                                    Link.Add(Points[i], 1);
                                }
                                MainGraph.Add(Point, Link);
                                nov.Add(Point, 0);
                            }
                        }
                    }
                    if (type == "t4")
                    {
                        while (!file.EndOfStream)
                        {
                            string Point = file.ReadLine();
                            Dictionary<string, double> Link = new Dictionary<string, double>();
                            string LinkString = file.ReadLine();
                            if (String.IsNullOrEmpty(LinkString))
                            {
                                MainGraph.Add(Point, Link);
                                nov.Add(Point, 0);
                            }
                            else
                            {
                                char[] c = { ' ' };
                                List<string> PointsAndWeights = LinkString.Split(c, StringSplitOptions.RemoveEmptyEntries).ToList();
                                for (int i = 0; i < PointsAndWeights.Count; i += 2)
                                {
                                    Link.Add(PointsAndWeights[i], double.Parse(PointsAndWeights[i + 1]));
                                }
                                MainGraph.Add(Point, Link);
                                nov.Add(Point, 0);
                            }
                        }
                    }
                }
                catch { }
            }
            Console.WriteLine("Граф создан");
        }
        public Graph(Graph graph)
        {
            foreach (var Point in graph.MainGraph)
            {
                Dictionary<string, double> Link = new Dictionary<string, double>();
                foreach (var Points in Point.Value)
                {
                    Link.Add(Points.Key, Points.Value);
                }
                MainGraph.Add(Point.Key, Link);
                this.type = graph.type;
                nov.Add(Point.Key, 0);
                foreach (var Points in graph.Sopostavitel)
                {
                    this.Sopostavitel.Add(Points.Key, Points.Value);
                }
                foreach (var Points in graph.DeSopostavitel)
                {
                    this.DeSopostavitel.Add(Points.Key, Points.Value);
                }
            }

            Console.WriteLine("Копия создана");
        }
        public void ShowMainGraph()
        {
            foreach (var Point in MainGraph)
            {
                Console.WriteLine("{0} ", Point.Key);
                foreach (var Points in Point.Value)
                {
                    if (Points.Value == 0)
                    {
                        Console.WriteLine("-> {0}", Points.Key);
                    }
                    else
                    {
                        Console.WriteLine("-> {0} - {1}", Points.Key, Points.Value);
                    }
                }
            }
        }
        //////////////////////////////Очистка словаря обхода
        public void ClearNov()
        {
            foreach (var Point in MainGraph)
            {
                nov[Point.Key] = 0;
            }
            //Console.WriteLine("Очистка массива для обходов прошла успешно");
        }

        //////////////////////////////Создание сопоставителей
        public void SetSopostavitels()
        {
            int i = 0;
            foreach (var Point in MainGraph)
            {
                Sopostavitel.Add(i, Point.Key);
                DeSopostavitel.Add(Point.Key, i);
                i++;
            }
        }

        //////////////////////////////Создание матрицы смежности
        public double[,] CreateMatrixMinW()
        {
            double[,] array = new double[MainGraph.Count, MainGraph.Count];
            for (int i = 0; i < MainGraph.Count; i++)
            {
                for (int j = 0; j < MainGraph.Count; j++)
                {
                    array[i, j] = 0;
                }
            }
            int q1 = 0, q2 = 0;
            foreach (var Point in MainGraph)
            {
                foreach (var Points in Point.Value)
                {
                    array[DeSopostavitel[Point.Key], DeSopostavitel[Points.Key]] = Points.Value;
                    q2++;
                }
                q2 = 0;
                q1++;
            }
            for (int i = 0; i < MainGraph.Count; i++)
            {
                for (int j = 0; j < MainGraph.Count; j++)
                {
                    if (array[i, j] == 0)
                    {
                        array[i, j] = int.MaxValue;
                    }
                }
            }
            return array;
        }
        public double[,] CreateMatrix()
        {
            double[,] array = new double[MainGraph.Count, MainGraph.Count];
            for (int i = 0; i < MainGraph.Count; i++)
            {
                for (int j = 0; j < MainGraph.Count; j++)
                {
                    array[i, j] = 0;
                }
            }
            int q1 = 0, q2 = 0;
            foreach (var Point in MainGraph)
            {
                foreach (var Points in Point.Value)
                {
                    array[DeSopostavitel[Point.Key], DeSopostavitel[Points.Key]] = Points.Value;
                    q2++;
                }
                q2 = 0;
                q1++;
            }
            return array;
        }

        //////////////////////////////Копирование словаря обхода

        public Dictionary<string, int> CopyNov()
        {
            Dictionary<string, int> temp = new Dictionary<string, int>();
            foreach (var key in nov)
            {
                temp.Add(key.Key, key.Value);
            }
            return temp;
        }

        //////////////////////////////Очистка сопоставителей
        public void ClearSopostavitels()
        {
            Sopostavitel.Clear();
            DeSopostavitel.Clear();
        }
        public void FindStepen1(Dictionary<string, Dictionary<string, double>> MyGraphCop)
        {
            int k;
            foreach (var Point1 in MyGraphCop)
            {
                k = MyGraphCop[Point1.Key].Count;
                foreach (var Point2 in MyGraphCop)
                {
                    if (MyGraphCop[Point1.Key].ContainsKey(Point2.Key) || MyGraphCop[Point2.Key].ContainsKey(Point1.Key))
                    {
                        k++;
                    }
                }
                if (k == 1)
                {
                    Console.WriteLine("{0} висячая вершина", Point1.Key);
                }
            }
            Console.WriteLine("Введите следующую команду");
        }
        public void FindGeneralVertex(Dictionary<string, Dictionary<string, double>> MyGraphCop, string Point1, string Point2)
        {
            int flag = 0;
            foreach (var Point3 in MyGraphCop)
            {
                if (MyGraphCop[Point1].ContainsKey(Point3.Key) || MyGraphCop[Point3.Key].ContainsKey(Point1))
                {
                    if (MyGraphCop[Point2].ContainsKey(Point3.Key) || MyGraphCop[Point3.Key].ContainsKey(Point2))
                    {
                        Console.WriteLine("{0} общая вершина между {1} и {2}", Point3.Key, Point1, Point2);
                        flag = 1;
                    }
                }
            }
            if (flag == 0)
            {
                Console.WriteLine("Общих вершин нет");
            }
        }

        public Graph Adition()
        {
            Graph DopGraph = new Graph(this.type);
            if (this.type == "t3")
            {
                foreach (var Point in MainGraph)
                {
                    DopGraph.Add_Vertex(Point.Key);
                }
                foreach (var Point1 in MainGraph)
                {
                    foreach (var Point2 in MainGraph)
                    {
                        if (Point1.Key != Point2.Key)
                        {
                            if (!MainGraph[Point1.Key].ContainsKey(Point2.Key))
                            {
                                DopGraph.AddWayFromVertexes(Point1.Key, Point2.Key);
                            }
                        }
                    }
                }
            }
            if (this.type == "t4")
            {
                //Random rnd = new Random();
                foreach (var Point in MainGraph)
                {
                    DopGraph.Add_Vertex(Point.Key);
                }
                foreach (var Point1 in MainGraph)
                {
                    foreach (var Point2 in MainGraph)
                    {
                        if (Point1.Key != Point2.Key)
                        {
                            if (!MainGraph[Point1.Key].ContainsKey(Point2.Key))
                            {
                                if (MainGraph[Point2.Key].ContainsKey(Point1.Key))
                                {
                                    DopGraph.AddWayFromVertexes(Point1.Key, Point2.Key, (MainGraph[Point2.Key])[Point1.Key]);
                                }
                                else
                                {
                                    DopGraph.AddWayFromVertexes(Point1.Key, Point2.Key, 2);
                                }
                            }
                        }
                    }
                }
            }
            return DopGraph;
        }
        public void Add_Vertex(string Point) //добавление вершины
        {
            Dictionary<string, double> Link = new Dictionary<string, double>();
            if (MainGraph.ContainsKey(Point)) // проверка, содержится ли данная точка уже в нашем словаре
            {
                Console.WriteLine("Ошибка: Такая точка уже есть");
            }
            else
            {
                MainGraph.Add(Point, Link);
                nov.Add(Point, 0);
                //Console.WriteLine("Точка добавлена");
            }
        }
        public void AddWayFromVertexes(string Point1, string Point2, double Weight) // добавление ребра для взвешенного графа
        {
            if (this.type == "t2")
            {
                if (MainGraph.ContainsKey(Point1) && MainGraph.ContainsKey(Point2) && (!MainGraph[Point1].ContainsKey(Point2) || !MainGraph[Point2].ContainsKey(Point1)))
                {
                    MainGraph[Point1].Add(Point2, Weight);
                    MainGraph[Point2].Add(Point1, Weight);
                    //Console.WriteLine("Ребро создано");
                }
                else
                {
                    Console.WriteLine("Ошибка:");
                    Console.WriteLine("обе вершины должны быть в графе или ребро между вершинами уже есть");
                }
            }
            if (this.type == "t4")
            {
                if (MainGraph.ContainsKey(Point1) && MainGraph.ContainsKey(Point2) && (!MainGraph[Point1].ContainsKey(Point2) || !MainGraph[Point2].ContainsKey(Point1)))
                {
                    MainGraph[Point1].Add(Point2, Weight);
                    //Console.WriteLine("Ребро создано");
                }
                else
                {
                    Console.WriteLine("Ошибка:");
                    Console.WriteLine("обе вершины должны быть в графе или ребро между вершинами уже есть");
                }
            }
        }

        public void AddWayFromVertexes(string Point1, string Point2) // добавление ребра для невзвешенного графа
        {
            if (this.type == "t1")
            {
                if (MainGraph.ContainsKey(Point1) && MainGraph.ContainsKey(Point2) && (!MainGraph[Point1].ContainsKey(Point2) || !MainGraph[Point2].ContainsKey(Point1)))
                {
                    MainGraph[Point1].Add(Point2, 1);
                    MainGraph[Point2].Add(Point1, 1);
                    //Console.WriteLine("Ребро создано");
                }
                else
                {
                    Console.WriteLine("Ошибка:");
                    Console.WriteLine("обе вершины должны быть в графе или ребро между вершинами уже есть");
                }
            }
            if (this.type == "t3")
            {
                if (MainGraph.ContainsKey(Point1) && MainGraph.ContainsKey(Point2) && (!MainGraph[Point1].ContainsKey(Point2) || !MainGraph[Point2].ContainsKey(Point1)))
                {
                    MainGraph[Point1].Add(Point2, 1);
                    //Console.WriteLine("Ребро создано");
                }
                else
                {
                    Console.WriteLine("Ошибка:");
                    Console.WriteLine("обе вершины должны быть в графе или ребро между вершинами уже есть");
                }
            }
        }
        public void Delete_Vertex(string Point) // удаление вершины 
        {
            Dictionary<string, double> Link = new Dictionary<string, double>();
            if (!MainGraph.ContainsKey(Point))
            {
                Console.WriteLine("Ошибка: точка не существует");
            }
            else
            {
                MainGraph.Remove(Point);
                nov.Remove(Point);
                foreach (string Points in MainGraph.Keys)
                {
                    if (MainGraph[Points].ContainsKey(Point))
                    {
                        MainGraph[Points].Remove(Point);
                    }
                }
                Console.WriteLine("Точка удалена");
            }
        }
        public void DeleteWayFromVertexes(string Point1, string Point2)
        {
            if (this.type == "t1" || this.type == "t2")
            {
                if (MainGraph.ContainsKey(Point1) && MainGraph.ContainsKey(Point2) && MainGraph[Point1].ContainsKey(Point2) && MainGraph[Point2].ContainsKey(Point1))
                {
                    if (MainGraph[Point1].ContainsKey(Point2) && MainGraph[Point2].ContainsKey(Point1))
                    {
                        MainGraph[Point1].Remove(Point2);
                        MainGraph[Point2].Remove(Point1);
                        //Console.WriteLine("Ребро удалено");
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка:");
                    Console.WriteLine("обе вершины должны быть в графе или ребро не существует");
                }
            }
            if (this.type == "t3" || this.type == "t4")
            {
                if (MainGraph.ContainsKey(Point1) && MainGraph.ContainsKey(Point2) && (MainGraph[Point1].ContainsKey(Point2) || MainGraph[Point2].ContainsKey(Point1)))
                {
                    if (MainGraph[Point1].ContainsKey(Point2) && MainGraph[Point2].ContainsKey(Point1))
                    {
                        MainGraph[Point1].Remove(Point2);
                        MainGraph[Point2].Remove(Point1);
                        //Console.WriteLine("Ребро удалено");
                    }
                    else if (!MainGraph[Point1].ContainsKey(Point2) && MainGraph[Point2].ContainsKey(Point1))
                    {
                        MainGraph[Point2].Remove(Point1);
                        //Console.WriteLine("Ребро удалено");
                    }
                    else if (MainGraph[Point1].ContainsKey(Point2) && !MainGraph[Point2].ContainsKey(Point1))
                    {
                        MainGraph[Point1].Remove(Point2);
                        //Console.WriteLine("Ребро удалено");
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка:");
                    Console.WriteLine("обе вершины должны быть в графе или ребро не существует");
                }
            }
        }
        public void WrtiteToFile(string Path)
        {
            using (StreamWriter file = new StreamWriter(Path))
            {
                foreach (var Point in MainGraph)
                {
                    file.WriteLine(Point.Key);
                    if (Point.Value.Count == 0)
                    {
                        file.Write("");
                    }
                    else
                    {
                        foreach (var Points in Point.Value)
                        {
                            file.Write("{0} {1} ", Points.Key, Points.Value);
                        }
                    }
                    file.WriteLine();
                }
            }
            Console.WriteLine("Записано");
        }
        public int GetCountVertex(Dictionary<string, Dictionary<string, double>> MyGraphCop)
        {
            int count = 0;
            foreach (var Point1 in MainGraph)
            {
                int k = MyGraphCop[Point1.Key].Count;
                count += k;
            }
            return count/2;
        }
        public int GetCountComponentSvyznosti()
        {
            int count = 0;

            return count;
        }
        public void Dfs(string v)
        {
            nov[v] = 1;
            foreach (var inv in MainGraph[v])
            {
                if (inv.Value != 0 && nov[inv.Key] == 0)
                {
                    Dfs(inv.Key);
                }
            }
        }
        public int CountComponent()
        {
            int count = 0;
            Dfs(nov.First().Key);
            count++;
            foreach (var v in MainGraph)
            {
                if (nov[v.Key] != 1)
                {
                    Dfs(v.Key);
                    count++;
                }
            }
            ClearNov();
            return count;
        }

        public void MinPuti(string u)
        {
            foreach (var point in MainGraph)
            {
                if (point.Key != u)
                {
                    int count = 0;
                    count = Bfs(point.Key, u);
                    ClearNov();
                    Console.WriteLine("Для вершины {0} минимальным путем до нашей вершины {1} будет: {2}", point.Key, u, count);
                }           
            }
        }


        //////////////////////////////Обход в ширину
        public int Bfs(string v, string u)
        {
            if (MainGraph.ContainsKey(v))
            {
                Dictionary<string, int> dist = new Dictionary<string, int>();
                foreach (string Point in MainGraph.Keys)
                {
                    dist.Add(Point, 0);
                }
                Queue<string> queue = new Queue<string>();
                queue.Enqueue(v);
                nov[v] = 1;
                int flag = 0;
                while (queue.Count != 0)
                {
                    v = queue.Dequeue();
                    foreach (var Points in MainGraph[v])
                    {
                        if (Points.Value != 0 && nov[Points.Key] == 0)
                        {
                            dist[Points.Key] = dist[v] + 1;
                            queue.Enqueue(Points.Key);
                            nov[Points.Key] = 1;
                        }
                    }
                }
                return dist[u];
            }
            else
            {
                return 0;
                Console.WriteLine("Такой точки нет в графе");
            }
        }
        public void NovSet()
        {
            nov = new Dictionary<string, int>();
            if (MainGraph != null)
            {
                foreach (var item in MainGraph)
                {
                    nov.Add(item.Key, 0);
                }
            }
        }
        public Graph Krascal()
        {
            List<(double weight, string a, string b)> ways = new List<(double, string, string)>();
            foreach (var item in MainGraph)
            {
                foreach (var item2 in MainGraph[item.Key])
                {
                    if (!(ways.Contains((MainGraph[item.Key][item2.Key], item.Key, item2.Key)) || 
                        ways.Contains((MainGraph[item.Key][item2.Key], item2.Key, item.Key))))
                        ways.Add((MainGraph[item.Key][item2.Key], item.Key, item2.Key));
                }
            }
            ways.Sort();

            Graph crasc = new Graph(type);

            foreach (var item in MainGraph)
            {
                crasc.Add_Vertex(item.Key);
            }
            crasc.NovSet();

            foreach ((double weight, string a, string b) item in ways)
            {
                crasc.AddWayFromVertexes(item.a, item.b, item.weight);

                if (crasc.hasCycles(item.a, " "))
                {
                    crasc.DeleteWayFromVertexes(item.a, item.b);
                }
                crasc.NovSet();

            }
            return crasc;
        }

        public bool hasCycles(string s, string prev)
        {
            nov[s] = 1;
            foreach (var item in MainGraph[s])
            {
                if (nov[item.Key] == 0)
                {
                    hasCycles(item.Key, s);
                }
                else if (item.Key != prev)
                {
                    return true;
                }
            }
            return false;
        }
        public void Dijkstr(string ver,int N)
        {
            SetSopostavitels();
            int v = DeSopostavitel[ver];
            double[,] array = CreateMatrixMinW();
            bool[] nov2 = new bool[MainGraph.Count];
            for (int i = 0; i < nov2.Length; i++)
            {
                nov2[i] = true;
            }
            double[] d = dijkstr(v, array, nov2);
            for (int i = 0; i < MainGraph.Count; i++)
            {
                if (i != v && d[i] >N && d[i]<int.MaxValue)
                {
                    Console.WriteLine("{0} равна {1}, ", Sopostavitel[i], d[i]);
                }
            }
            ClearSopostavitels();
            ClearNov();
        }

        public double[] dijkstr(int v, double[,] array, bool[] nov2)
        {
            nov2[v] = false;
            double[,] c = new double[MainGraph.Count, MainGraph.Count];
            for (int i = 0; i < MainGraph.Count; i++)
            {
                for (int u = 0; u < MainGraph.Count; u++)
                {
                    if (array[i, u] == 0 || i == u)
                    {
                        c[i, u] = int.MaxValue;
                    }
                    else
                    {
                        c[i, u] = array[i, u];
                    }
                }
            }
            double[] d = new double[MainGraph.Count];
            for (int u = 0; u < MainGraph.Count; u++)
            {
                if (u != v)
                {
                    d[u] = c[v, u];
                }
            }
            for (int i = 0; i < MainGraph.Count - 1; i++)
            {
                double min = int.MaxValue;
                int w = 0;
                for (int u = 0; u < MainGraph.Count; u++)
                {
                    if (nov2[u] && min > d[u])
                    {
                        min = d[u];
                        w = u;
                    }
                }
                nov2[w] = false;
                for (int u = 0; u < MainGraph.Count; u++)
                {
                    double distance = d[w] + c[w, u];
                    if (nov2[u] && d[u] > distance)
                    {
                        d[u] = distance;
                    }
                }
            }
            return d;
        }

        //////////////////////////////Флойд 8 задача

        //public void Floyd(string u, string v1,string v2)
        //{
        //    SetSopostavitels();
        //    double[,] array = CreateMatrixMinW();
        //    int[,] p;
        //    double[,] a = floyd(out p, array);
        //    int i, j;
        //    for (i = 0; i < MainGraph.Count; i++)
        //    {
        //        for (j = 0; j < MainGraph.Count; j++)
        //        {
        //            if (i != j && i == DeSopostavitel[u] && (j == DeSopostavitel[v1] || j == DeSopostavitel[v2]))
        //            {
        //                if (a[i, j] == int.MaxValue)
        //                {
        //                    Console.WriteLine("Пути из вершины {0} в вершину {1} не существует", Sopostavitel[i], Sopostavitel[j]);
        //                }
        //                else
        //                {
        //                        Console.WriteLine("Кратчайший путь от вершины " + Sopostavitel[i] + " до вершины " + Sopostavitel[j] + " равен " + a[i, j].ToString());
        //                }
        //            }
        //        }
        //    }
        //    ClearSopostavitels();
        //}
        //public double[,] floyd(out int[,] p, double[,] array)
        //{
        //    int i, j, k;
        //    double[,] a = new double[MainGraph.Count, MainGraph.Count];
        //    p = new int[MainGraph.Count, MainGraph.Count];
        //    for (i = 0; i < MainGraph.Count; i++)
        //    {
        //        for (j = 0; j < MainGraph.Count; j++)
        //        {
        //            if (i == j)
        //            {
        //                a[i, j] = 0;
        //            }
        //            else
        //            {
        //                if (array[i, j] == 0)
        //                {
        //                    a[i, j] = int.MaxValue;
        //                }
        //                else
        //                {
        //                    a[i, j] = array[i, j];
        //                }
        //            }
        //            p[i, j] = -1;
        //        }
        //    }
        //    for (k = 0; k < MainGraph.Count; k++)
        //    {
        //        for (i = 0; i < MainGraph.Count; i++)
        //        {
        //            for (j = 0; j < MainGraph.Count; j++)
        //            {
        //                double distance = a[i, k] + a[k, j];
        //                if (a[i, j] > distance)
        //                {
        //                    a[i, j] = distance;
        //                    p[i, j] = k;
        //                }
        //            }
        //        }
        //    }
        //    return a;
        //}
        public void BellmanFord(string u, string v1, string v2)
        {
            if (!MainGraph.ContainsKey(u) || !MainGraph.ContainsKey(v1) || !MainGraph.ContainsKey(v2))
            {
                Console.WriteLine("Ошибка: точка не существует");
            }
            else
            {
                Dictionary<string, double> distance = new Dictionary<string, double>();
                Dictionary<string, string> pred = new Dictionary<string, string>();
                foreach (var item in MainGraph.Keys)
                {
                    distance[item] = int.MaxValue;
                    pred[item] = null;
                }

                distance[u] = 0;

                for (int i = 0; i < MainGraph.Count - 1; i++)
                {
                    foreach (var item in MainGraph)
                    {
                        foreach (var item2 in item.Value)
                        {
                            if (distance[item.Key] + item2.Value < distance[item2.Key])
                            {
                                distance[item2.Key] = distance[item.Key] + item2.Value;
                                pred[item2.Key] = item.Key;
                            }
                        }
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("Кратчайший путь от {0} до {1}: {2}", u, v1, GetPath(u, v1, pred));
                Console.WriteLine("Кратчайший путь от {0} до {1}: {2}", u, v2, GetPath(u, v2, pred));
            }
        }
        private string GetPath(string start, string end, Dictionary<string, string> pred)
        {
            if (!pred.ContainsKey(end) || pred[end] == null)
                return start == end ? start : "Путь не существует";

            return GetPath(start, pred[end], pred) + " " + end;
        }

        //////////////////////////////////////////////////////////// Floyd ////////////////////////////////////////////////////////////////////////
        public void Floyd(string u)
        {
            if (!MainGraph.ContainsKey(u))
            {
                Console.WriteLine("Ошибка: точка не существует");
            }
            else
            {
                Dictionary<(string, string), double> distance = new Dictionary<(string, string), double>();
                foreach (var item in MainGraph)
                {
                    foreach (var item2 in MainGraph)
                    {
                        if (item.Key == item2.Key)
                            distance[(item.Key, item2.Key)] = 0;
                        else
                            distance[(item.Key, item2.Key)] = MainGraph.ContainsKey(item.Key) && MainGraph[item.Key].ContainsKey(item2.Key) ? MainGraph[item.Key][item2.Key] : int.MaxValue;
                    }
                }

                foreach (var k in MainGraph)
                {
                    foreach (var item in MainGraph)
                    {
                        foreach (var item2 in MainGraph)
                        {
                            if (distance.ContainsKey((item.Key, k.Key)) && distance.ContainsKey((k.Key, item2.Key)) && (distance.ContainsKey((item.Key, item2.Key)) || item.Key == item2.Key)
                                && distance[(item.Key, k.Key)] != int.MaxValue && distance[(k.Key, item2.Key)] != int.MaxValue && distance[(item.Key, k.Key)] + distance[(k.Key, item2.Key)] < distance[(item.Key, item2.Key)])
                            {
                                distance[(item.Key, item2.Key)] = distance[(item.Key, k.Key)] + distance[(k.Key, item2.Key)];
                            }
                        }
                    }
                }

                //foreach (var item in distance.Keys)
                //{
                //    string i = item.Item1;
                //    string j = item.Item2;
                //    if (distance[(i, j)] != int.MaxValue && distance[(j, j)] < 0 && distance[(j, i)] != int.MaxValue)
                //    {
                //        Console.WriteLine("В графе содержится отрицательный цикл");
                //        return;
                //    }
                //}

                foreach (var item in MainGraph)
                {
                    if (distance.ContainsKey((u, item.Key)) && distance[(u, item.Key)] != int.MaxValue)
                    {
                        if (distance.ContainsKey((u, item.Key)) && distance[(u, item.Key)] < 0)
                        {
                            distance[(u, item.Key)] = int.MinValue;
                            Console.WriteLine("Кратчайший путь из " + u + " в " + item.Key + ": " + distance[(u, item.Key)]);
                        }
                        else
                            Console.WriteLine("Кратчайший путь из " + u + " в " + item.Key + ": " + distance[(u, item.Key)]);
                    }
                    else
                        Console.WriteLine("Пути из " + u + " в " + item.Key + " не существует");
                }
            }
        }
        public bool Bfs(double[,] graph, int start, int end, int[] parent)
        {
            bool[] visited = new bool[parent.Length];
            Queue<int> queue = new Queue<int>();

            visited[start] = true;
            queue.Enqueue(start);
            parent[start] = -1;

            while (queue.Any())
            {
                int u = queue.Dequeue();

                for (int v = 0; v < graph.GetLength(1); v++)
                {
                    if (!visited[v] && graph[u, v] > 0)
                    {
                        visited[v] = true;
                        queue.Enqueue(v);
                        parent[v] = u;
                    }
                }
            }

            return visited[end];
        }
        public void FordFulkersonAlgorithm(string x1, string x2)
        {
            ClearNov();
            SetSopostavitels();
            double[,] graph = CreateMatrix();
            int start = DeSopostavitel[x1];
            int end = DeSopostavitel[x2];
            int V = MainGraph.Keys.Count;
            double[,] residualGraph = new double[V, V];
            Array.Copy(graph, residualGraph, graph.Length);

            int[] parent = new int[V];
            double maxFlow = 0;

            while (Bfs(residualGraph, start, end, parent))
            {
                double pathFlow = double.MaxValue;

                for (int v = end; v != start; v = parent[v])
                {
                    int u = parent[v];
                    pathFlow = Math.Min(pathFlow, residualGraph[u, v]);
                    Console.Write("{0} {1} ", Sopostavitel[v], Sopostavitel[u]);
                }
                Console.Write(pathFlow);
                Console.WriteLine();
                for (int v = end; v != start; v = parent[v])
                {
                    int u = parent[v];
                    residualGraph[u, v] -= pathFlow;
                    residualGraph[v, u] += pathFlow;
                }

                maxFlow += pathFlow;
            }

            Console.WriteLine(maxFlow);
        }
        //public void BellmanFord(string[,] graph, int vertices, string source)
        //{
        //    int INF = int.MaxValue;
        //    int[] distance = new int[vertices];
        //    // Инициализируем расстояние до всех вершин как бесконечность
        //    for (int i = 0; i < vertices; ++i)
        //        distance[i] = INF;
        //    // Расстояние от исходной вершины равно 0
        //    distance[source] = 0;
        //    // Релаксация всех ребер |V| - 1 раз
        //    for (int i = 0; i < vertices - 1; ++i)
        //    {
        //        for (int j = 0; j < vertices; ++j)
        //        {
        //            for (int k = 0; k < vertices; ++k)
        //            {
        //                if (graph[j, k] != 0 && distance[j] != INF && distance[j] + graph[j, k] < distance[k])
        //                    distance[k] = distance[j] + graph[j, k];
        //            }
        //        }
        //    }
        //    // Проверяем наличие циклов отрицательного веса
        //    for (int j = 0; j < vertices; ++j)
        //    {
        //        for (int k = 0; k < vertices; ++k)
        //        {
        //            if (graph[j, k] != 0 && distance[j] != INF && distance[j] + graph[j, k] < distance[k])
        //            {
        //                Console.WriteLine("Граф содержит циклы отрицательного веса");
        //                return;
        //            }
        //        }
        //    }
        //    // Выводим длины кратчайших путей
        //    Console.WriteLine("Вершина \t\t Расстояние от источника");
        //    for (int i = 0; i < vertices; ++i)
        //        Console.WriteLine(i + "\t\t\t" + distance[i]);
        //}
        //public void BellmanFord(string u, string v1, string v2)
        //{
        //        Dictionary<string, double> distance = new Dictionary<string, double>();
        //        Dictionary<string, string> pred = new Dictionary<string, string>();
        //        foreach (var item in MainGraph.Keys)
        //        {
        //            distance[item] = int.MaxValue;
        //            pred[item] = null;
        //        }

        //        distance[u] = 0;

        //        for (int i = 0; i < MainGraph.Count - 1; i++)
        //        {
        //            foreach (var item in MainGraph)
        //            {
        //                foreach (var item2 in item.Value)
        //                {
        //                    if (distance[item.Key] + item2.Value < distance[item2.Key])
        //                    {
        //                        distance[item2.Key] = distance[item.Key] + item2.Value;
        //                        pred[item2.Key] = item.Key;
        //                    }
        //                }
        //            }
        //            Console.WriteLine();
        //        }
        //        Console.WriteLine("Кратчайший путь от {0} до {1}: {2}", u, v1, GetPath(u, v1, pred));
        //        Console.WriteLine("Кратчайший путь от {0} до {1}: {2}", u, v2, GetPath(u, v2, pred));
        //    }
        //private string GetPath(string start, string end, Dictionary<string, string> pred)
        //{
        //    if (!pred.ContainsKey(end) || pred[end] == null)
        //        return start == end ? start : "Путь не существует";

        //    return GetPath(start, pred[end], pred) + " " + end;
        //}


        //Dictionary<string, string> DFS(string start)
        //{
        //    Dictionary<string, bool> used = new Dictionary<string, bool>();
        //    Dictionary<string, string> pred = new Dictionary<string, string>();
        //    foreach (string node in MainGraph.Keys)
        //    {
        //        used[node] = false;
        //    }
        //    Stack<string> stack = new Stack<string>();
        //    stack.Push(start);
        //    while (stack.Count > 0)
        //    {
        //        string cur = stack.Pop();
        //        foreach (string node in MainGraph[cur].Keys)
        //        {
        //            if (!used[node])
        //            {
        //                stack.Push(node);
        //                pred[node] = cur;
        //            }
        //        }
        //        used[cur] = true;
        //    }
        //    return pred;
        //}
        //public long Task10(string s, string t)
        //{
        //    if (!MainGraph.ContainsKey(s))
        //    {
        //        throw new Exception($"Graph doesn't have node {s}.");
        //    }
        //    if (!MainGraph.ContainsKey(t))
        //    {
        //        throw new Exception($"Graph doesn't have node {t}.");
        //    }
        //    if (!DFS(s).ContainsKey(t))
        //    {
        //        throw new Exception($"There is no path from {s} to {t}");
        //    }

        //    Graph tmp = new Graph(this);
        //    long flow = 0;
        //    while (true)
        //    {
        //        Dictionary<string, string> pred = tmp.DFS(s);
        //        if (!pred.ContainsKey(t))
        //        {
        //            return flow;
        //        }
        //        string str = t;
        //        List<string> list = new List<string>();
        //        while (str != s)
        //        {
        //            list.Add(str);
        //            str = pred[str];
        //        }
        //        list.Add(s);
        //        list.Reverse();
        //        long cur_flow = long.MaxValue;
        //        for (int i = 0; i < list.Count - 1; i++)
        //        {
        //            cur_flow = (long)Math.Min(cur_flow, tmp.MainGraph[list[i]][list[i + 1]]);
        //        }
        //        for (int i = 0; i < list.Count - 1; i++)
        //        {
        //            if (tmp.MainGraph[list[i]][list[i + 1]] == cur_flow)
        //            {
        //                tmp.MainGraph[list[i]].Remove(list[i + 1]);
        //            }
        //            else
        //            {
        //                tmp.MainGraph[list[i]][list[i + 1]] -= cur_flow;
        //            }

        //            if (!tmp.MainGraph[list[i + 1]].ContainsKey(list[i]))
        //            {
        //                tmp.MainGraph[list[i + 1]].Add(list[i], cur_flow);
        //            }
        //            else
        //            {
        //                tmp.MainGraph[list[i + 1]][list[i]] += cur_flow;
        //            }
        //        }
        //        flow += cur_flow;
        //    }
        //}
    }

    public class Program
    {
        static void Main()
        {
            string type = "";
            string inway = "";
            //string inway = "test1.txt";
            string outway = "vivod.txt";
            Console.WriteLine("Выберите тип графа");
            Console.WriteLine("Неориентированный невзвешенный граф: t1");
            Console.WriteLine("Неориентированный взвешенный граф: t2");
            Console.WriteLine("Ориентированный невзвешенный граф: t3");
            Console.WriteLine("Ориентированный взвешенный граф: t4");
            string key = Console.ReadLine();
            Graph MyGraph = new Graph(key);
            Graph MyGraphCop = new Graph(key); // копия графа
            if (key == "t1")
            {
                inway = "test1.txt";
            }
            else if (key == "t2")
            {
                inway = "test2.txt";
            }
            else if (key == "t3")
            {
                inway = "test3.txt";
            }
            else if (key == "t4")
            {
                inway = "test4.txt";
            }
            switch (key)
            {
                case "t1":
                    Console.WriteLine("Выход из программы: esc");
                    Console.WriteLine("Показать изначальный граф: 0");
                    Console.WriteLine("Показать копию графа: 1");
                    Console.WriteLine("Очистить граф: 2");
                    Console.WriteLine("Заполнить граф по файлу: 3");
                    Console.WriteLine("Записать граф в файл: 4");
                    Console.WriteLine("Добавить вершину: add vertex");
                    Console.WriteLine("Добавить ребро: add way");
                    Console.WriteLine("Удалить вершину: delete vertex");
                    Console.WriteLine("Удалить ребро: delete way");
                    Console.WriteLine("Проверить висячую вершину: 5");
                    Console.WriteLine("Найти общую вершину между двух вершин: 6");
                    Console.WriteLine("Найти цикломатическое число графа: min");
                    Console.WriteLine("Вывести длины кратчайших путей до вершины от всех вершин: dugi");
                    Console.WriteLine("Вывести N переферию: per");
                    Console.WriteLine("Вывести кратчайший путь от вершины u до v1 и v2: kp");
                    Console.WriteLine("Вывести длины кратчайших путей от u до всех остальных вершин.: kpf");
                    type = "t1";
                    while (true)
                    {
                        string key1 = Console.ReadLine();
                        if (key1 == "esc")
                        {
                            break;
                        }
                        else
                        {
                            switch (key1)
                            {
                                case "0":
                                    MyGraph.ShowMainGraph();
                                    break;
                                case "1":
                                    MyGraphCop.ShowMainGraph();
                                    break;
                                case "2":
                                    MyGraphCop = new Graph(type);
                                    Console.WriteLine("Копия графа обнулена");
                                    break;
                                case "3":
                                    MyGraph = new Graph(inway, type);
                                    MyGraphCop = new Graph(MyGraph);
                                    break;
                                case "4":
                                    MyGraphCop.WrtiteToFile(outway);
                                    break;
                                case "add vertex":
                                    Console.WriteLine("Введите добавляемую вершину");
                                    MyGraphCop.Add_Vertex(Console.ReadLine());
                                    break;
                                case "add way":
                                    Console.WriteLine("Введите первую вершину");
                                    string FirstPointA = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string SecondPointA = Console.ReadLine();
                                    //Console.WriteLine("Введите длину пути");
                                    //int Way = int.Parse(Console.ReadLine());
                                    MyGraphCop.AddWayFromVertexes(FirstPointA, SecondPointA);
                                    break;
                                case "delete vertex":
                                    Console.WriteLine("Введите удаляемую вершину");
                                    MyGraphCop.Delete_Vertex(Console.ReadLine());
                                    break;
                                case "delete way":
                                    Console.WriteLine("Введите первую вершину");
                                    string FirstPointD = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string SecondPointD = Console.ReadLine();
                                    MyGraphCop.DeleteWayFromVertexes(FirstPointD, SecondPointD);
                                    break;
                                case "5":
                                    MyGraph.FindStepen1(MyGraphCop.MainGraph);
                                    break;
                                case "6":
                                    Console.WriteLine("Введите первую вершину");
                                    string FirstPointI = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string SecondPointI = Console.ReadLine();
                                    MyGraph.FindGeneralVertex(MyGraphCop.MainGraph, FirstPointI, SecondPointI);
                                    break;
                                case "min":
                                    int n = MyGraphCop.nov.Count(); //кол - во вершин
                                    int N = MyGraphCop.GetCountVertex(MyGraphCop.MainGraph); //кол - во ребер
                                    int P = MyGraphCop.CountComponent(); //кол-во компонент
                                    Console.WriteLine("{0}, {1}, {2}", n, N, P);
                                    Console.WriteLine(N - n + P);
                                    break;
                                case "dugi":
                                    Console.WriteLine("Введите вершину, пути до которой ищем");
                                    string point = Console.ReadLine();
                                    MyGraphCop.MinPuti(point);
                                    break;
                                case "per":
                                    Console.WriteLine("Введите вершину");
                                    string start = Console.ReadLine();
                                    Console.WriteLine("Введите N");
                                    int NN = int.Parse(Console.ReadLine());
                                    MyGraphCop.Dijkstr(start,NN);
                                    break;
                                case "kp":
                                    Console.WriteLine("Введите вершину u");
                                    string u = Console.ReadLine();
                                    Console.WriteLine("Введите вершину v1");
                                    string v1 = Console.ReadLine();
                                    Console.WriteLine("Введите вершину v2");
                                    string v2 = Console.ReadLine();
                                    MyGraphCop.BellmanFord(u, v1, v2);
                                    break;
                                case "kpf":
                                    Console.WriteLine("Введите вершину u");
                                    string _u = Console.ReadLine();
                                    MyGraphCop.Floyd(_u);
                                    break;
                                default:
                                    Console.WriteLine("Такой команды нет");
                                    break;
                            }
                        }
                    }
                    break;
                case "t2":
                    Console.WriteLine("Выход из программы: esc");
                    Console.WriteLine("Показать изначальный граф: 0");
                    Console.WriteLine("Показать копию графа: 1");
                    Console.WriteLine("Очистить граф: 2");
                    Console.WriteLine("Заполнить граф по файлу: 3");
                    Console.WriteLine("Записать граф в файл: 4");
                    Console.WriteLine("Добавить вершину: add vertex");
                    Console.WriteLine("Добавить ребро: add way");
                    Console.WriteLine("Удалить вершину: delete vertex");
                    Console.WriteLine("Удалить ребро: delete way");
                    Console.WriteLine("Проверить висячую вершину: 5");
                    Console.WriteLine("Найти общую вершину между двух вершин: 6");
                    Console.WriteLine("Найти цикломатическое число графа: min");
                    Console.WriteLine("Вывести длины кратчайших путей до вершины от всех вершин: dugi");
                    Console.WriteLine("Алгоритм Краскала: krascal");
                    Console.WriteLine("Вывести N переферию: per");
                    Console.WriteLine("Вывести кратчайший путь от вершины u до v1 и v2: kp");
                    Console.WriteLine("Вывести длины кратчайших путей от u до всех остальных вершин.: kpf");
                    type = "t2";
                    while (true)
                    {
                        string key2 = Console.ReadLine();
                        if (key2 == "esc")
                        {
                            break;
                        }
                        else
                        {
                            switch (key2)
                            {
                                case "0":
                                    MyGraph.ShowMainGraph();
                                    break;
                                case "1":
                                    MyGraphCop.ShowMainGraph();
                                    break;
                                case "2":
                                    MyGraphCop = new Graph(type);
                                    Console.WriteLine("Копия графа обнулена");
                                    break;
                                case "3":
                                    MyGraph = new Graph(inway, type);
                                    MyGraphCop = new Graph(MyGraph);
                                    break;
                                case "4":
                                    MyGraphCop.WrtiteToFile(outway);
                                    break;
                                case "add vertex":
                                    Console.WriteLine("Введите добавляемую вершину");
                                    MyGraphCop.Add_Vertex(Console.ReadLine());
                                    break;
                                case "add way":
                                    Console.WriteLine("Введите первую вершину");
                                    string FirstPointA = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string SecondPointA = Console.ReadLine();
                                    Console.WriteLine("Введите длину пути");
                                    int Way = int.Parse(Console.ReadLine());
                                    MyGraphCop.AddWayFromVertexes(FirstPointA, SecondPointA, Way);
                                    break;
                                case "delete vertex":
                                    Console.WriteLine("Введите удаляемую вершину");
                                    MyGraphCop.Delete_Vertex(Console.ReadLine());
                                    break;
                                case "delete way":
                                    Console.WriteLine("Введите первую вершину");
                                    string FirstPointD = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string SecondPointD = Console.ReadLine();
                                    MyGraphCop.DeleteWayFromVertexes(FirstPointD, SecondPointD);
                                    break;
                                case "5":
                                    MyGraph.FindStepen1(MyGraphCop.MainGraph);
                                    break;
                                case "6":
                                    Console.WriteLine("Введите первую вершину");
                                    string FirstPointI = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string SecondPointI = Console.ReadLine();
                                    MyGraph.FindGeneralVertex(MyGraphCop.MainGraph, FirstPointI, SecondPointI);
                                    break;
                                case "min":
                                    int n = MyGraphCop.nov.Count(); //кол - во вершин
                                    int N = MyGraphCop.GetCountVertex(MyGraphCop.MainGraph); //кол - во ребер
                                    int P = MyGraphCop.CountComponent(); //кол-во компонент
                                    Console.WriteLine("{0}, {1}, {2}", n, N, P);
                                    Console.WriteLine(N - n + P);
                                    break;
                                case "dugi":
                                    Console.WriteLine("Введите вершину, пути до которой ищем");
                                    string point = Console.ReadLine();
                                    MyGraphCop.MinPuti(point);
                                    break;
                                case "krascal":
                                    if (MyGraphCop.CountComponent() > 1)
                                    {
                                        Console.WriteLine("Граф не связан");
                                    }
                                    else
                                    {
                                        Graph crasc = MyGraphCop.Krascal();
                                        crasc.ShowMainGraph();
                                    }
                                    break;
                                case "per":
                                    Console.WriteLine("Введите вершину");
                                    string start = Console.ReadLine();
                                    Console.WriteLine("Введите N");
                                    int NN = int.Parse(Console.ReadLine());
                                    MyGraphCop.Dijkstr(start, NN);
                                    break;
                                case "kp":
                                    Console.WriteLine("Введите вершину u");
                                    string u = Console.ReadLine();
                                    Console.WriteLine("Введите вершину v1");
                                    string v1 = Console.ReadLine();
                                    Console.WriteLine("Введите вершину v2");
                                    string v2 = Console.ReadLine();
                                    MyGraphCop.BellmanFord(u, v1, v2);
                                    break;
                                case "kpf":
                                    Console.WriteLine("Введите вершину u");
                                    string _u = Console.ReadLine();
                                    MyGraphCop.Floyd(_u);
                                    break;
                                default:
                                    Console.WriteLine("Такой команды нет");
                                    break;
                            }
                        }
                    }
                    break;
                case "t3":
                    Console.WriteLine("Выход из программы: esc");
                    Console.WriteLine("Показать изначальный граф: 0");
                    Console.WriteLine("Показать копию графа: 1");
                    Console.WriteLine("Очистить граф: 2");
                    Console.WriteLine("Заполнить граф по файлу: 3");
                    Console.WriteLine("Записать граф в файл: 4");
                    Console.WriteLine("Добавить вершину: add vertex");
                    Console.WriteLine("Добавить ребро: add way");
                    Console.WriteLine("Удалить вершину: delete vertex");
                    Console.WriteLine("Удалить ребро: delete way");
                    Console.WriteLine("Дополнение орграфа: dop");
                    Console.WriteLine("Вывести длины кратчайших путей до вершины от всех вершин: dugi");
                    Console.WriteLine("Вывести N переферию: per");
                    Console.WriteLine("Вывести кратчайший путь от вершины u до v1 и v2: kp");
                    Console.WriteLine("Вывести длины кратчайших путей от u до всех остальных вершин.: kpf");
                    type = "t3";
                    while (true)
                    {
                        string key3 = Console.ReadLine();
                        if (key3 == "esc")
                        {
                            break;
                        }
                        else
                        {
                            switch (key3)
                            {
                                case "0":
                                    MyGraph.ShowMainGraph();
                                    break;
                                case "1":
                                    MyGraphCop.ShowMainGraph();
                                    break;
                                case "2":
                                    MyGraphCop = new Graph(type);
                                    Console.WriteLine("Копия графа обнулена");
                                    break;
                                case "3":
                                    MyGraph = new Graph(inway, type);
                                    MyGraphCop = new Graph(MyGraph);
                                    break;
                                case "4":
                                    MyGraphCop.WrtiteToFile(outway);
                                    break;
                                case "add vertex":
                                    Console.WriteLine("Введите добавляемую вершину");
                                    MyGraphCop.Add_Vertex(Console.ReadLine());
                                    break;
                                case "add way":
                                    Console.WriteLine("Введите первую вершину");
                                    string FirstPointA = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string SecondPointA = Console.ReadLine();
                                    //Console.WriteLine("Введите длину пути");
                                    //int Way = int.Parse(Console.ReadLine());
                                    MyGraphCop.AddWayFromVertexes(FirstPointA, SecondPointA);
                                    break;
                                case "delete vertex":
                                    Console.WriteLine("Введите удаляемую вершину");
                                    MyGraphCop.Delete_Vertex(Console.ReadLine());
                                    break;
                                case "delete way":
                                    Console.WriteLine("Введите первую вершину");
                                    string FirstPointD = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string SecondPointD = Console.ReadLine();
                                    MyGraphCop.DeleteWayFromVertexes(FirstPointD, SecondPointD);
                                    break;
                                case "dop":
                                    //Graph AditionGraph = new Graph(key);
                                    //AditionGraph = MyGraph.Adition(MyGraphCop.MainGraph);
                                    //AditionGraph.ShowMainGraph();
                                    Graph DopGraph = MyGraph.Adition();
                                    DopGraph.ShowMainGraph();
                                    break;
                                case "dugi":
                                    Console.WriteLine("Введите вершину, пути до которой ищем");
                                    string point = Console.ReadLine();
                                    MyGraphCop.MinPuti(point);
                                    break;
                                case "per":
                                    Console.WriteLine("Введите вершину");
                                    string start = Console.ReadLine();
                                    Console.WriteLine("Введите N");
                                    int NN = int.Parse(Console.ReadLine());
                                    MyGraphCop.Dijkstr(start, NN);
                                    break;
                                case "kp":
                                    Console.WriteLine("Введите вершину u");
                                    string u = Console.ReadLine();
                                    Console.WriteLine("Введите вершину v1");
                                    string v1 = Console.ReadLine();
                                    Console.WriteLine("Введите вершину v2");
                                    string v2 = Console.ReadLine();
                                    MyGraphCop.BellmanFord(u, v1, v2);
                                    break;
                                case "kpf":
                                    Console.WriteLine("Введите вершину u");
                                    string _u = Console.ReadLine();
                                    MyGraphCop.Floyd(_u);
                                    break;
                                default:
                                    Console.WriteLine("Такой команды нет");
                                    break;
                            }
                        }
                    }
                    break;
                case "t4":
                    Console.WriteLine("Выход из программы: esc");
                    Console.WriteLine("Показать изначальный граф: 0");
                    Console.WriteLine("Показать копию графа: 1");
                    Console.WriteLine("Очистить граф: 2");
                    Console.WriteLine("Заполнить граф по файлу: 3");
                    Console.WriteLine("Записать граф в файл: 4");
                    Console.WriteLine("Добавить вершину: add vertex");
                    Console.WriteLine("Добавить ребро: add way");
                    Console.WriteLine("Удалить вершину: delete vertex");
                    Console.WriteLine("Удалить ребро: delete way");
                    Console.WriteLine("Дополнение орграфа: dop");
                    Console.WriteLine("Вывести длины кратчайших путей до вершины от всех вершин: dugi");
                    Console.WriteLine("Вывести N переферию: per");
                    Console.WriteLine("Вывести кратчайший путь от вершины u до v1 и v2: kp");
                    Console.WriteLine("Вывести длины кратчайших путей от u до всех остальных вершин.: kpf");
                    Console.WriteLine("Нахождение максимального потока: potoki");
                    type = "t4";
                    while (true)
                    {
                        string key4 = Console.ReadLine();
                        if (key4 == "esc")
                        {
                            break;
                        }
                        else
                        {
                            switch (key4)
                            {
                                case "0":
                                    MyGraph.ShowMainGraph();
                                    break;
                                case "1":
                                    MyGraphCop.ShowMainGraph();
                                    break;
                                case "2":
                                    MyGraphCop = new Graph(type);
                                    Console.WriteLine("Копия графа обнулена");
                                    break;
                                case "3":
                                    MyGraph = new Graph(inway, type);
                                    MyGraphCop = new Graph(MyGraph);
                                    break;
                                case "4":
                                    MyGraphCop.WrtiteToFile(outway);
                                    break;
                                case "add vertex":
                                    Console.WriteLine("Введите добавляемую вершину");
                                    MyGraphCop.Add_Vertex(Console.ReadLine());
                                    break;
                                case "add way":
                                    Console.WriteLine("Введите первую вершину");
                                    string FirstPointA = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string SecondPointA = Console.ReadLine();
                                    Console.WriteLine("Введите длину пути");
                                    int Way = int.Parse(Console.ReadLine());
                                    MyGraphCop.AddWayFromVertexes(FirstPointA, SecondPointA, Way);
                                    break;
                                case "delete vertex":
                                    Console.WriteLine("Введите удаляемую вершину");
                                    MyGraphCop.Delete_Vertex(Console.ReadLine());
                                    break;
                                case "delete way":
                                    Console.WriteLine("Введите первую вершину");
                                    string FirstPointD = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string SecondPointD = Console.ReadLine();
                                    MyGraphCop.DeleteWayFromVertexes(FirstPointD, SecondPointD);
                                    break;
                                case "dop":
                                    Graph DopGraph = MyGraph.Adition();
                                    DopGraph.ShowMainGraph();
                                    break;
                                case "dugi":
                                    Console.WriteLine("Введите вершину, пути до которой ищем");
                                    string point = Console.ReadLine();
                                    MyGraphCop.MinPuti(point);
                                    break;
                                case "per":
                                    Console.WriteLine("Введите вершину");
                                    string start = Console.ReadLine();
                                    Console.WriteLine("Введите N");
                                    int NN = int.Parse(Console.ReadLine());
                                    MyGraphCop.Dijkstr(start, NN);
                                    break;
                                case "kp":
                                    Console.WriteLine("Введите вершину u");
                                    string u = Console.ReadLine();
                                    Console.WriteLine("Введите вершину v1");
                                    string v1 = Console.ReadLine();
                                    Console.WriteLine("Введите вершину v2");
                                    string v2 = Console.ReadLine();
                                    MyGraphCop.BellmanFord(u, v1, v2);
                                    break;
                                case "kpf":
                                    Console.WriteLine("Введите вершину u");
                                    string _u = Console.ReadLine();
                                    MyGraphCop.Floyd(_u);
                                    break;
                                case "potoki":
                                    Console.WriteLine("Введите первую вершину");
                                    string startov = Console.ReadLine();
                                    Console.WriteLine("Введите вторую вершину");
                                    string end = Console.ReadLine();
                                    MyGraph.FordFulkersonAlgorithm(startov, end);
                                    //MyGraph.Task10(startov, end);
                                    break;
                                default:
                                    Console.WriteLine("Такой команды нет");
                                    break;
                            }
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Неизвестная команда");
                    break;
            }
        }
    }
}