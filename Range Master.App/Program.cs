using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Reflection;

namespace Range_Master.App
{
    internal class Program
    {
        private static string field_prefix { get; } = "form1[0].Page1[0]";

        private static void Main(string[] args)
        {
            // Who needs the owner password?

            PdfReader.unethicalreading = true;

            using (var output = new MemoryStream())
            using (var input = Assembly.GetExecutingAssembly().GetManifestResourceStream("RangeMaster.App.DA_3595-R_Record_Fire_Scorecard.pdf"))
            using (var reader = new PdfReader(input))
            using (var stamper = new PdfStamper(reader, output))
            {
                var form = stamper.AcroFields;

                // Header Info

                // form1[0].Page1[0].DATE[0]

                // form1[0].Page1[0].IDCODE[0]
                // form1[0].Page1[0].UNIT[0]
                // form1[0].Page1[0].EVALUATOR[0]

                // form1[0].Page1[0].Remarks[0]

                // A, B are Table 1 - Prone Supported
                // C is Table 2 - Prone Unsupported
                // D is Table 3 - Kneeling Unsupported

                // Indivual Targets

                // {field_prefix}.Hit{A,B,C,D}_{1-10}[0]
                // {field_prefix}.Miss{A,B,C,D}_{1-10}[0]
                // {field_prefix}.NoFire{A,B,C,D}_{1-10}[0]

                // Totals for each column (A/B/C/D)

                // {field_prefix}.Total_Hit_Column1[0]
                // {field_prefix}.Total_Miss_Column1[0]
                // {field_prefix}.Total_NoFire_Column1[0]
                // {field_prefix}.Total_Hit_Column2[0]
                // {field_prefix}.Total_Miss_Column2[0]
                // {field_prefix}.Total_NoFire_Column2[0]
                // {field_prefix}.Total_Hit_Column3[0]
                // {field_prefix}.Total_Miss_Column3[0]
                // {field_prefix}.Total_NoFire_Column3[0]
                // {field_prefix}.Total_Hit_Column4[0]
                // {field_prefix}.Total_Miss_Column4[0]
                // {field_prefix}.Total_NoFire_Column4[0]

                // Totals for the table

                // {field_prefix}.Hit{A,B,C,D}[0]
                // {field_prefix}.Miss{A,B,C,D}[0]
                // {field_prefix}.NoFire{A,B,C,D}[0]

                // {field_prefix}.Total_Hit_Table_1[0]
                // {field_prefix}.Total_Hit_Table_2[0]
                // {field_prefix}.Total_Hit_Table_3[0]

                //{field_prefix}.Total_Miss_Table_1[0]
                //{field_prefix}.Total_Miss_Table_2[0]
                //{field_prefix}.Total_Miss_Table_3[0]

                // {field_prefix}.Total_NoFire_Table_1[0]
                // {field_prefix}.Total_NoFire_Table_2[0]
                // {field_prefix}.Total_NoFire_Table_3[0]

                foreach (var field in form.Fields)
                {
                    Console.WriteLine($"{field.Key}");
                }

                form.SetField($"{field_prefix}.MissA_6[0]", "Test");
            }

            // Totals for the Entire Iterations

            // {field_prefix}.Total_Points[0]

            // {field_prefix}.Marksman[0]
            // {field_prefix}.Expert[0]
            // {field_prefix}.ShrpShot[0]
            // {field_prefix}.Unqualifd[0]

            // What site did they use?

            //form1[0].Page1[0].AA[0]
            //form1[0].Page1[0].EE[0]
            //form1[0].Page1[0].BB[0]
            //form1[0].Page1[0].GG[0]
            //form1[0].Page1[0].CC[0]
            //form1[0].Page1[0].HH[0]
            //form1[0].Page1[0].DD[0]
            //form1[0].Page1[0].FF[0]

            // Qualified with IBA?

            //form1[0].Page1[0].YES[0]
            //form1[0].Page1[0].NO[0]

            // Evaluator Signatures

            //form1[0].Page1[0].Initial1[0]
            //form1[0].Page1[0].Initial2[0]
            //form1[0].Page1[0].DATE1[0]
            //form1[0].Page1[0].DATE2[0]

            //form1[0].Page1[0].signature_BUTTON1[0]
            //form1[0].Page1[0].signature_BUTTON2[0]

            Console.ReadKey();
        }
    }
}