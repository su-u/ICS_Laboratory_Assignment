using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
        private static int autoNumber { get; set; } = 1;
        public string Name { get; private set; }
        public int Number { get; private set; }
        private readonly int MAX;
        private List<Student> Students;

        public int DT { get; set; } = 0;

        public Laboratory(string name, int max)
        {
            this.Number = autoNumber++;
            this.Name = name;
            this.MAX = max;
            Students = new List<Student>();
            this.DT = 0;
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
    }


    class Program
    {
        static void Main(string[] args)
        {
            List<Student> unassigned = new List<Student>();

            List<Laboratory> laboratories = new List<Laboratory>
            {
                new Laboratory("清水研", 14),
                new Laboratory("水津研", 14),
                new Laboratory("菅原研", 14),
                new Laboratory("中静研", 14),
                new Laboratory("枚田研", 14),
                new Laboratory("中林研", 14),
                new Laboratory("藤原研", 14),
                new Laboratory("木下研", 8),
                new Laboratory("糸井研", 8),
                new Laboratory("長研", 14),
            };

            var n = int.Parse(Console.ReadLine());

            for (int i = 0; i < n; i++)
            {
                var line = Console.ReadLine()?.Split().ToList();

                if(line?.Count != 12)throw new Exception($"入力ミス:{i + 1}");

                unassigned.Add(new Student(int.Parse(line?[0]), double.Parse(line[1]), line.Skip(2).Select(int.Parse).ToList()));
            }

            unassigned = unassigned.OrderByDescending(x => x.Gpa).ToList();

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

            foreach (var student in unassigned)
            {

                var s = student.Satisfactions.GroupBy(x => x.Item2, value => value.Item1).ToList();

                bool j = false;
                foreach (var i in s)
                {
                    var k = i.ToList();
                    var labo = laboratories.Where(x => k.Contains(x.Number)).OrderBy(y => y.DT).ToList();
                    foreach (var l in labo)
                    {
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

            //Console.WriteLine($"学生配属先研究室");
            //foreach (var laboratory in laboratories)
            //{
            //    Console.Write($"{laboratory.Name}: ");
            //    laboratory.PrintStudents();
            //}
        }
    }
}
