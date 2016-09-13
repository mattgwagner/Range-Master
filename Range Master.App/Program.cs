using iTextSharp.text.pdf;
using RangeMaster.App;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using static RangeMaster.App.FormModel;

namespace Range_Master.App
{
    internal class Program
    {
        private static string field_prefix { get; } = "form1[0].Page1[0]";

        private static void Main(string[] args)
        {
            // Who needs the owner password?

            var model = new FormModel { };

            model.Remarks = "Awesome remarks!";

            model.IdCode = "Test Id Code";

            // TODO Miss and NoFire don't register the right checkboxes

            model.Table1 = Enumerable.Range(1, 20).Select(_ => new Target { Number = _, Result = TargetResult.Hit }).ToList();
            model.Table2 = Enumerable.Range(1, 10).Select(_ => new Target { Number = _, Result = TargetResult.Hit }).ToList();
            model.Table3 = Enumerable.Range(1, 10).Select(_ => new Target { Number = _, Result = TargetResult.Miss }).ToList();

            PdfReader.unethicalreading = true;

            using (var output = new FileStream("Output.pdf", FileMode.Create))
            using (var input = Assembly.GetExecutingAssembly().GetManifestResourceStream("RangeMaster.App.DA_3595-R_Record_Fire_Scorecard.pdf"))
            using (var reader = new PdfReader(input))
            using (var stamper = new PdfStamper(reader, output))
            {
                var form = stamper.AcroFields;

                // Header Info

                form.SetField($"{field_prefix}.IDCODE[0]", model.IdCode);
                form.SetField($"{field_prefix}.UNIT[0]", model.Unit);

                form.SetField($"{field_prefix}.DATE[0]", model.Date.ToString("yyyyMMdd"));

                form.SetField($"{field_prefix}.EVALUATOR[0]", model.Evaluator);

                form.SetField($"{field_prefix}.Remarks[0]", model.Remarks);

                // Individual Targets

                // A, B are Table 1 - Prone Supported

                foreach (var target in model.Table1.Where(_ => _.Number < 11))
                {
                    form.SetField(GetFieldToSet("A", target), FromResult(target.Result));
                }

                form.SetField($"{field_prefix}.Total_Hit_Column1[0]", model.Column1_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Column1[0]", model.Column1_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Column1[0]", model.Column1_Result.TotalNoFire.ToString());

                foreach (var target in model.Table1.Where(_ => _.Number > 10))
                {
                    form.SetField(GetFieldToSet("B", target), FromResult(target.Result));
                }

                form.SetField($"{field_prefix}.Total_Hit_Column2[0]", model.Column2_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Column2[0]", model.Column2_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Column2[0]", model.Column2_Result.TotalNoFire.ToString());

                // C is Table 2 - Prone Unsupported

                foreach (var target in model.Table2)
                {
                    form.SetField(GetFieldToSet("C", target), FromResult(target.Result));
                }

                form.SetField($"{field_prefix}.Total_Hit_Column3[0]", model.Table2_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Column3[0]", model.Table2_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Column3[0]", model.Table2_Result.TotalNoFire.ToString());

                // D is Table 3 - Kneeling Unsupported

                foreach (var target in model.Table3)
                {
                    form.SetField(GetFieldToSet("D", target), FromResult(target.Result));
                }

                form.SetField($"{field_prefix}.Total_Hit_Column4[0]", model.Table3_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Column4[0]", model.Table3_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Column4[0]", model.Table3_Result.TotalNoFire.ToString());

                // Totals for the table

                // {field_prefix}.Hit{A,B,C,D}[0]
                // {field_prefix}.Miss{A,B,C,D}[0]
                // {field_prefix}.NoFire{A,B,C,D}[0]

                form.SetField($"{field_prefix}.Total_Hit_Table_1[0]", model.Table1_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Hit_Table_2[0]", model.Table2_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Hit_Table_3[0]", model.Table3_Result.TotalHit.ToString());

                form.SetField($"{field_prefix}.Total_Miss_Table_1[0]", model.Table1_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Table_2[0]", model.Table2_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Table_3[0]", model.Table3_Result.TotalMiss.ToString());

                form.SetField($"{field_prefix}.Total_NoFire_Table_1[0]", model.Table1_Result.TotalNoFire.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Table_2[0]", model.Table2_Result.TotalNoFire.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Table_3[0]", model.Table3_Result.TotalNoFire.ToString());

                // Totals for the Entire Iteration

                form.SetField($"{field_prefix}.Total_Points[0]", model.TotalScore.ToString());

                form.SetField($"{field_prefix}.Marksman[0]", FromQualification(model.Qualification));
                form.SetField($"{field_prefix}.Expert[0]", FromQualification(model.Qualification));
                form.SetField($"{field_prefix}.ShrpShot[0]", FromQualification(model.Qualification));
                form.SetField($"{field_prefix}.Unqualifd[0]", FromQualification(model.Qualification));

                // What sight did they use?

                //{field_prefix}.AA[0]
                //{field_prefix}.EE[0]
                //{field_prefix}.BB[0]
                //{field_prefix}.GG[0]
                //{field_prefix}.CC[0]
                //{field_prefix}.HH[0]
                //{field_prefix}.DD[0]
                //{field_prefix}.FF[0]

                // Qualified with IBA?

                form.SetField($"{field_prefix}.YES[0]", model.QualifiedWithIba ? "1" : String.Empty);
                form.SetField($"{field_prefix}.NO[0]", model.QualifiedWithIba ? String.Empty : "2");

                // Evaluator Signatures

                //{field_prefix}.Initial1[0]
                //{field_prefix}.Initial2[0]
                //{field_prefix}.DATE1[0]
                //{field_prefix}.DATE2[0]

                //{field_prefix}.signature_BUTTON1[0]
                //{field_prefix}.signature_BUTTON2[0]
            }

            Console.ReadKey();
        }

        private static String GetFieldToSet(String table, Target target)
        {
            String base_name = $"{field_prefix}.{target.Result}{table}";

            if (target.Number > 1)
            {
                if ("B".Equals(table))
                {
                    // Target 11 does not need a _ prefix since it's the first in column 2

                    if (target.Number > 11)
                    {
                        base_name += $"_{--target.Number - 10}";
                    }
                }
                else
                {
                    base_name += $"_{--target.Number}";
                }
            }

            // For target 1, there is no _1

            //form.SetField($"{field_prefix}.HitA_{target.Number}[0]", FromBool(target.Result == TargetResult.Hit));
            //form.SetField($"{field_prefix}.MissA_{target.Number}[0]", FromBool(target.Result == TargetResult.Miss));
            //form.SetField($"{field_prefix}.NoFireA_{target.Number}[0]", FromBool(target.Result == TargetResult.NoFire));

            return $"{base_name}[0]";
        }

        private static String FromBool(Boolean result)
        {
            return result ? "1" : "0";
        }

        private static String FromResult(TargetResult result)
        {
            switch (result)
            {
                case TargetResult.Hit:
                    return "1";

                case TargetResult.Miss:
                    return "2";

                case TargetResult.NoFire:
                default:
                    return "3";
            }
        }

        private static String FromQualification(Qualification qualification)
        {
            switch (qualification)
            {
                case Qualification.Expert:
                    return "1";

                case Qualification.Sharpshooter:
                    return "2";

                case Qualification.Marksman:
                    return "1";

                case Qualification.Unqualified:
                default:
                    return "4";
            }
        }
    }
}