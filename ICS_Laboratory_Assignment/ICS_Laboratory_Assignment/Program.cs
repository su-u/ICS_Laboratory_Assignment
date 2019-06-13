using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ICS_Laboratory_Assignment
{
    class Student
    {
        public int Number { get; private set; }
        public double Gpa { get; set; }
        public readonly List<Tuple<int,int>> Satisfactions = new List<Tuple<int, int>>();


        public Student(int num, double gpa, List<int> satisfaction)
        {
            this.Number = num;
            this.Gpa = gpa;

            for (int i = 1; i <= satisfaction.Count; i++)
            {
                this.Satisfactions.Add(Tuple.Create(i, satisfaction[i - 1]));
            }
            this.Satisfactions = this.Satisfactions.OrderByDescending(x => x.Item2).ToList();
        }

        public int this[int i] => this.Satisfactions.Single(x => x.Item1 == i).Item2;

    }

    class Laboratory
    {
        private static int AutoNumber { get; set; } = 1;
        public static int MaximumStudent { get; private set; } = 0;
        public string Name { get; private set; }
        public int Number { get; private set; }
        private readonly int MAX;
        private List<Student> Students;

        public int DT { get; set; } = 0;

        public Laboratory(string name, int max)
        {
            this.Number = Laboratory.AutoNumber++;
            this.Name = name;
            this.MAX = max;
            Students = new List<Student>();
            this.DT = 0;
            Laboratory.MaximumStudent += max;
        }

        public int Size => this.Students.Count;

        public bool IsMax()
        {
            return this.Size >= this.MAX;
        }

        public Student this[int i] => this.Students[i];

        public void AddStudent(Student s)
        {
            this.Students.Add(s);
        }

        public void PrintStudents()
        {
            foreach (var student in this.Students)
            {
                Console.Write($"{student.Number} ");
            }
            Console.WriteLine();
        }

        public void WriteStudents()
        {
            foreach (var student in this.Students)
            {
                Writer.Write(@"Output.txt",$"{student.Number} ");
            }
            Writer.Write(@"Output.txt", $"\n");
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            List<Student> unassigned = new List<Student>();
            List<Laboratory> laboratories = new List<Laboratory>();

            //研究室情報の読み取り
            try
            {
                using (StreamReader sr = new StreamReader("Laboratory.txt", System.Text.Encoding.UTF8))
                {
                    var line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        var l = line?.Split().ToArray();
                        if(l.Length > 2) throw new FormatException("入力値のミス");
                        laboratories.Add(new Laboratory(l[0], int.Parse(l[1])));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            //学生情報の読み込み
            try
            {
                using (StreamReader sr = new StreamReader("Students.txt", System.Text.Encoding.UTF8))
                {
                    //var n = int.Parse(Console.ReadLine());
                    var n = int.Parse(sr.ReadLine());

                    if (n >= Laboratory.MaximumStudent)
                        throw new System.ArgumentOutOfRangeException($"入力人数が研究室上限を超えています。");

                    for (int i = 0; i < n; i++)
                    {
                        //var line = Console.ReadLine()?.Split().ToList();
                        var line = sr.ReadLine()?.Split().ToList();

                        if (line?.Count != 12) throw new System.FormatException($"入力ミス:{i + 1}");

                        var gpa = double.Parse(line[1]);
                        if (gpa < 1 && gpa > 4) throw new System.FormatException($"GPAの入力値ミス:{i + 1}");
                        var satisfactionsList = line.Skip(2).Select(int.Parse).ToList();
                        foreach (var s in satisfactionsList)
                            if (s < 1 && s > 10)
                                throw new System.FormatException($"希望の入力値ミス:{i + 1}");

                        unassigned.Add(new Student(int.Parse(line?[0]), gpa, satisfactionsList));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            unassigned = unassigned.OrderByDescending(x => x.Gpa).ToList();

            //研究室ごとの合計希望値の計算
            foreach (var laboratory in laboratories)
            {
                laboratory.DT = unassigned.Sum(x => x[laboratory.Number]);
            }

            Console.WriteLine($"各研究室満足度合計");
            foreach (var laboratory in laboratories)
            {
                Console.WriteLine($"{laboratory.Name,3}:{laboratory.DT}");
            }
            Console.WriteLine();

            //各学生の研究室への割り当て
            foreach (var student in unassigned)
            {

                //学生の希望度順
                var s = student.Satisfactions.GroupBy(x => x.Item2, value => value.Item1).ToList();

                bool j = false;
                //最大規模値の研究室から処理
                foreach (var i in s)
                {
                    var k = i.ToList();
                    var labo = laboratories.Where(x => k.Contains(x.Number)).OrderBy(y => y.DT).ToList();
                    //希望研究室の他学生の最小希望から処理
                    foreach (var l in labo)
                    {
                        //研究室が満員だった場合は次へと回す
                        if (!l.IsMax())
                        {
                            l.AddStudent(student);
                            l.DT -= student[l.Number];
                            j = true;
                            Console.WriteLine($"{student.Number}: 配属先 =>{l.Name}");
                            break;

                        }
                    }
                    if(j)break;
                }
                Console.WriteLine($"各研究室満足度合計");
                foreach (var laboratory in laboratories)
                {
                    Console.Write($"{laboratory.Number}:{laboratory.Name,3}:{laboratory.DT}: {laboratory.Size}:");
                    laboratory.PrintStudents();
                }
                Console.WriteLine();



            }

            System.IO.File.Delete(@"Output.txt");
            Console.WriteLine($"学生配属先研究室");
            Writer.Write(@"Output.txt", $"学生配属先研究室\n");
            foreach (var laboratory in laboratories)
            {
                Console.Write($"{laboratory.Name}: ");
                Writer.Write(@"Output.txt",$"{laboratory.Name}: ");
                //laboratory.PrintStudents();
                laboratory.WriteStudents();
            }

            Console.ReadKey();
        }
    }

    class Writer
    {
        public static void WriteLog(string filename, string text)
        {
            Write(filename, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}, {text}");
        }
        public static void Write(string filename, string text)
        {
            try
            {
                var append = true;
                using (var sw = new System.IO.StreamWriter(filename, append, System.Text.Encoding.UTF8))
                {
                    sw.Write($"{text}");
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
