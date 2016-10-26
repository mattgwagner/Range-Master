using LinqToExcel;
using RangeMaster.App;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Range_Master.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("Usage: RangeMaster.App.exe {input_file}.xls");
                Environment.Exit(0);
            }

            foreach (var model in Read_From_Excel(args.First()))
            {
                model.ToFile();

                Console.WriteLine($"Saved to {model.FileName}");
            }

            Console.WriteLine("Completed!");

            Console.ReadKey();
        }

        public static IEnumerable<FormModel> Read_From_Excel(String filename)
        {
            var excel = new ExcelQueryFactory(filename);

            foreach (var row in excel.Worksheet("raw").Skip(2))
            {
                int current = 0;

                var model = new FormModel
                {
                    SoldierIdentifier = row[current++],
                    Unit = row[current++],
                };

                foreach (var target in Enumerable.Range(1, 20))
                {
                    model.Table1.Add(new FormModel.Target
                    {
                        Number = target,
                        Result = row[current++].Cast<int>() == 1 ? TargetResult.Hit : TargetResult.Miss
                    });
                }

                foreach (var target in Enumerable.Range(1, 10))
                {
                    model.Table2.Add(new FormModel.Target
                    {
                        Number = target,
                        Result = row[current++].Cast<int>() == 1 ? TargetResult.Hit : TargetResult.Miss
                    });
                }

                foreach (var target in Enumerable.Range(1, 10))
                {
                    model.Table3.Add(new FormModel.Target
                    {
                        Number = target,
                        Result = row[current++].Cast<int>() == 1 ? TargetResult.Hit : TargetResult.Miss
                    });
                }

                yield return model;
            }
        }
    }
}