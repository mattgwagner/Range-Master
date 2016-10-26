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
            // TODO actually read the file

            yield return new FormModel
            {
                SoldierIdentifier = "WAGNER MATTHEW",
                Unit = "A BTRY 2-116 FA"
            };
        }
    }
}