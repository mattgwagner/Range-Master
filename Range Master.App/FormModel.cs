using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RangeMaster.App
{
    public class FormModel
    {
        private static string field_prefix { get; } = "form1[0].Page1[0]";

        static FormModel()
        {
            // Who needs the owner password?

            PdfReader.unethicalreading = true;
        }

        public virtual String FileName
        {
            get
            {
                return $"{Unit}_{SoldierIdentifier}_{TotalScore}.pdf".Replace(' ', '_');
            }
        }

        public String SoldierIdentifier { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;

        public String Unit { get; set; }

        public String Evaluator { get; set; }

        public String Remarks { get; set; }

        public Boolean QualifiedWithIba { get; set; } = false;

        public Sight Sight { get; set; } = Sight.IronSight;

        public ICollection<Target> Table1 { get; set; } = new List<Target>(capacity: 20);

        public ICollection<Target> Table2 { get; set; } = new List<Target>(capacity: 10);

        public ICollection<Target> Table3 { get; set; } = new List<Target>(capacity: 10);

        public TableResult Column1_Result
        {
            get
            {
                return new TableResult(Table1.Where(_ => _.Number < 11));
            }
        }

        public TableResult Column2_Result
        {
            get
            {
                return new TableResult(Table1.Where(_ => _.Number > 10));
            }
        }

        public TableResult Table1_Result { get { return new TableResult(Table1); } }

        // Table2_Result is also for Column3

        public TableResult Table2_Result { get { return new TableResult(Table2); } }

        // Table3_Result is also for Column 4

        public TableResult Table3_Result { get { return new TableResult(Table3); } }

        public int TotalScore
        {
            get
            {
                return new[] { Table1_Result, Table2_Result, Table3_Result }.Sum(_ => _.TotalHit);
            }
        }

        public Qualification Qualification
        {
            get
            {
                if (TotalScore >= (int)Qualification.Expert) return Qualification.Expert;

                if (TotalScore >= (int)Qualification.Sharpshooter) return Qualification.Sharpshooter;

                if (TotalScore >= (int)Qualification.Marksman) return Qualification.Marksman;

                return Qualification.Unqualified;
            }
        }

        public class Target
        {
            public int Number { get; set; }

            public TargetResult Result { get; set; } = TargetResult.NoFire;
        }

        public class TableResult
        {
            public int TotalHit { get; }

            public int TotalMiss { get; }

            public int TotalNoFire { get; }

            public TableResult(IEnumerable<Target> results)
            {
                TotalHit = results.Where(_ => _.Result == TargetResult.Hit).Count();
                TotalMiss = results.Where(_ => _.Result == TargetResult.Miss).Count();
                TotalNoFire = results.Where(_ => _.Result == TargetResult.NoFire).Count();
            }
        }

        public virtual void ToFile()
        {
            using (var output = new FileStream(FileName, FileMode.Create))
            using (var input = Assembly.GetExecutingAssembly().GetManifestResourceStream("RangeMaster.App.DA_3595-R_Record_Fire_Scorecard.pdf"))
            using (var reader = new PdfReader(input))
            using (var stamper = new PdfStamper(reader, output))
            {
                var form = stamper.AcroFields;

                // Header Info

                form.SetField($"{field_prefix}.IDCODE[0]", SoldierIdentifier);
                form.SetField($"{field_prefix}.UNIT[0]", Unit);

                form.SetField($"{field_prefix}.DATE[0]", Date.ToString("yyyyMMdd"));

                form.SetField($"{field_prefix}.EVALUATOR[0]", Evaluator);

                form.SetField($"{field_prefix}.Remarks[0]", Remarks);

                // Individual Targets

                // A, B are Table 1 - Prone Supported

                foreach (var target in Table1.Where(_ => _.Number < 11))
                {
                    form.SetField(GetFieldToSet("A", target), FromResult(target.Result));
                }

                form.SetField($"{field_prefix}.Total_Hit_Column1[0]", Column1_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Column1[0]", Column1_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Column1[0]", Column1_Result.TotalNoFire.ToString());

                foreach (var target in Table1.Where(_ => _.Number > 10))
                {
                    form.SetField(GetFieldToSet("B", target), FromResult(target.Result));
                }

                form.SetField($"{field_prefix}.Total_Hit_Column2[0]", Column2_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Column2[0]", Column2_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Column2[0]", Column2_Result.TotalNoFire.ToString());

                // C is Table 2 - Prone Unsupported

                foreach (var target in Table2)
                {
                    form.SetField(GetFieldToSet("C", target), FromResult(target.Result));
                }

                form.SetField($"{field_prefix}.Total_Hit_Column3[0]", Table2_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Column3[0]", Table2_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Column3[0]", Table2_Result.TotalNoFire.ToString());

                // D is Table 3 - Kneeling Unsupported

                foreach (var target in Table3)
                {
                    form.SetField(GetFieldToSet("D", target), FromResult(target.Result));
                }

                form.SetField($"{field_prefix}.Total_Hit_Column4[0]", Table3_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Column4[0]", Table3_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Column4[0]", Table3_Result.TotalNoFire.ToString());

                // Totals for the table

                // {field_prefix}.Hit{A,B,C,D}[0]
                // {field_prefix}.Miss{A,B,C,D}[0]
                // {field_prefix}.NoFire{A,B,C,D}[0]

                form.SetField($"{field_prefix}.Total_Hit_Table_1[0]", Table1_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Hit_Table_2[0]", Table2_Result.TotalHit.ToString());
                form.SetField($"{field_prefix}.Total_Hit_Table_3[0]", Table3_Result.TotalHit.ToString());

                form.SetField($"{field_prefix}.Total_Miss_Table_1[0]", Table1_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Table_2[0]", Table2_Result.TotalMiss.ToString());
                form.SetField($"{field_prefix}.Total_Miss_Table_3[0]", Table3_Result.TotalMiss.ToString());

                form.SetField($"{field_prefix}.Total_NoFire_Table_1[0]", Table1_Result.TotalNoFire.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Table_2[0]", Table2_Result.TotalNoFire.ToString());
                form.SetField($"{field_prefix}.Total_NoFire_Table_3[0]", Table3_Result.TotalNoFire.ToString());

                // Totals for the Entire Iteration

                form.SetField($"{field_prefix}.Total_Points[0]", TotalScore.ToString());

                form.SetField($"{field_prefix}.Marksman[0]", FromQualification(Qualification));
                form.SetField($"{field_prefix}.Expert[0]", FromQualification(Qualification));
                form.SetField($"{field_prefix}.ShrpShot[0]", FromQualification(Qualification));
                form.SetField($"{field_prefix}.Unqualifd[0]", FromQualification(Qualification));

                // What sight did they use?

                form.SetField($"{field_prefix}.AA[0]", FromSight(Sight)); // Iron Sight
                form.SetField($"{field_prefix}.BB[0]", FromSight(Sight)); // Backup Iron Sight
                form.SetField($"{field_prefix}.CC[0]", FromSight(Sight)); // M68, CCO
                form.SetField($"{field_prefix}.DD[0]", FromSight(Sight)); // ACOG

                // Qualified with IBA?

                form.SetField($"{field_prefix}.YES[0]", QualifiedWithIba ? "1" : String.Empty);
                form.SetField($"{field_prefix}.NO[0]", QualifiedWithIba ? String.Empty : "2");

                // Evaluator Signatures

                //{field_prefix}.Initial1[0]
                //{field_prefix}.Initial2[0]
                //{field_prefix}.DATE1[0]
                //{field_prefix}.DATE2[0]

                //{field_prefix}.signature_BUTTON1[0]
                //{field_prefix}.signature_BUTTON2[0]
            }
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
                    return "3";

                case Qualification.Unqualified:
                default:
                    return "4";
            }
        }

        private static String FromSight(Sight sight)
        {
            switch (sight)
            {
                case Sight.ACOG:
                    return "4";

                case Sight.CCO:
                    return "3";

                case Sight.BackupIronSight:
                    return "2";

                case Sight.IronSight:
                default:
                    return "1";
            }
        }

        private static void PrintOptions(AcroFields form, String fieldName)
        {
            foreach (var state in form.GetAppearanceStates(fieldName))
            {
                Console.WriteLine($"State for {fieldName}: " + state);
            }
        }
    }

    public enum TargetResult
    {
        NoFire,

        Hit,

        Miss
    }

    public enum Qualification
    {
        Unqualified,

        Marksman = 23,

        Sharpshooter = 30,

        Expert = 36
    }

    public enum Sight
    {
        IronSight = 1,

        BackupIronSight = 2,

        CCO = 3,

        ACOG = 4
    }
}