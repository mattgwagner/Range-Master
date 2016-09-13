using System;
using System.Collections.Generic;
using System.Linq;

namespace RangeMaster.App
{
    public class FormModel
    {
        public String IdCode { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;

        public String Unit { get; set; }

        public String Evaluator { get; set; }

        public String Remarks { get; set; }

        public Boolean QualifiedWithIba { get; set; } = false;

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
}