﻿using Excel;
using RangeMaster.App;
using System;
using System.Collections.Generic;
using System.IO;
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
            using (var input = new FileStream(filename, FileMode.Open))
            {
                var excel = ExcelReaderFactory.CreateOpenXmlReader(input);

                var data = excel.AsDataSet();

                var table = data.Tables[0].Rows;

                for (var row = 2; row < table.Count; row++)
                {
                    int column = 0;

                    var model = new FormModel
                    {
                        SoldierIdentifier = (String)table[row][column++],
                        Unit = (String)table[row][column++]
                    };

                    foreach (var target in Enumerable.Range(1, 20))
                    {
                        model.Table1.Add(new FormModel.Target
                        {
                            Number = target,
                            Result = (int)table[row][column++] == 1 ? TargetResult.Hit : TargetResult.Miss
                        });
                    }

                    foreach (var target in Enumerable.Range(1, 10))
                    {
                        model.Table2.Add(new FormModel.Target
                        {
                            Number = target,
                            Result = (int)table[row][column++] == 1 ? TargetResult.Hit : TargetResult.Miss
                        });
                    }

                    foreach (var target in Enumerable.Range(1, 10))
                    {
                        model.Table3.Add(new FormModel.Target
                        {
                            Number = target,
                            Result = (int)table[row][column++] == 1 ? TargetResult.Hit : TargetResult.Miss
                        });
                    }

                    yield return model;
                }
            }
        }
    }
}