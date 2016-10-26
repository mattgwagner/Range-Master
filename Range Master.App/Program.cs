using RangeMaster.App;
using System;

namespace Range_Master.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var model = new FormModel
            {
                SoldierIdentifier = "WAGNER MATTHEW",
                Unit = "A BTRY 2-116 FA"
            };

            model.ToFile();

            Console.ReadKey();
        }
    }
}