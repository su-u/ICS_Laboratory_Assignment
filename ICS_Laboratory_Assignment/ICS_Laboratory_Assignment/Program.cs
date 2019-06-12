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
        public readonly List<Tuple<int,int>> Satisfaction = new List<Tuple<int, int>>();


        public Student(int num, double gpa, List<int> satisfaction)
        {
            this.Number = num;
            this.Gpa = gpa;

            for (int i = 1; i <= satisfaction.Count; i++)
            {
                this.Satisfaction.Add(Tuple.Create(i, satisfaction[i - 1]));
            }
            this.Satisfaction = this.Satisfaction.OrderByDescending(x => x.Item2).ToList();
        }

        public IReadOnlyList<int> GetMaximumSatisfaction()
        {
             return Satisfaction.GroupBy(x => x)
                .Where(z => z.Key.Item2 == this.Satisfaction.Max(y => y.Item2))
                .Select(zz => zz.Key.Item2).ToList<int>();
        }
    }

    class Laboratory
    {
        public string Name { get; private set; }
        public int Number { get; private set; }
        private readonly int MAX;
        private List<Student> Students;

        public Laboratory(int num, string name, int max)
        {
            this.Name = name;
            this.Number = num;
            this.MAX = max;
            Students = new List<Student>();
        }

        public int Size => this.Students.Count;

        public Student this[int i] => this.Students[i];
    }


    class Program
    {
        static void Main(string[] args)
        {
            List<Student> unassigned = new List<Student>();

            List<Laboratory> laboratory = new List<Laboratory>
            {
                new Laboratory(1, "清水研", 14),
                new Laboratory(2, "水津研", 14),
                new Laboratory(3, "菅原研", 14),
                new Laboratory(4, "中静研", 14),
                new Laboratory(5, "枚田研", 14),
                new Laboratory(6, "中林研", 14),
                new Laboratory(7, "藤原研", 14),
                new Laboratory(8, "木下研", 8),
                new Laboratory(9, "糸井研", 8),
                new Laboratory(10, "長研", 14),
            };

            var n = int.Parse(Console.ReadLine());

            for (int i = 0; i < n; i++)
            {
                var line = Console.ReadLine()?.Split().ToList();
                unassigned.Add(new Student(int.Parse(line?[0]), double.Parse(line[1]), line.Skip(2).Select(int.Parse).ToList()));
            }

            unassigned = unassigned.OrderByDescending(x => x.Gpa).ToList();

            foreach (var student in unassigned)
            {
                var s = student.Satisfaction.GroupBy(x => x)
                    .Where(z => z.Key.Item2 == student.Satisfaction.Max(y => y.Item2))
                    .Select(zz => zz.Key.Item2).ToList<int>();
                foreach (var i in s)
                {
                    Console.WriteLine(i);
                }
            }
        }
    }
}
