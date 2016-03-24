using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using FileHelpers.Events;

namespace check
{
    [DelimitedRecord(","), IgnoreFirst]
    class WriteItem : INotifyWrite
    {
        //[FieldConverter(ConverterKind.Date, "yyyy-dd-MM")]
        public string ymd;
        //[FieldConverter(ConverterKind.Date, "H:mm:ss")]
        public string hms;
        public string name;
        public int roomNum;
        public int termId;
        public void BeforeWrite(BeforeWriteEventArgs e)
        {
            if (name.StartsWith("A"))
            {
                e.SkipThisRecord = true;
            }
        }

        public void AfterWrite(AfterWriteEventArgs e)
        {

        }
    }
}
