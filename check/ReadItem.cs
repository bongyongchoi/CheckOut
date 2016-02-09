using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using FileHelpers.Events;
using System.Text.RegularExpressions;

namespace check
{
    [DelimitedRecord(","), IgnoreFirst]
    class ReadItem : INotifyRead, INotifyWrite
    {
        [FieldConverter(ConverterKind.Date, "yyyy-dd-MM")]
        public DateTime ymd;
        [FieldConverter(ConverterKind.Date, "H:mm:ss")]
        public DateTime hms;
        public int termId;
        public int userId;
        public string name;
        [FieldValueDiscarded]
        int? staffId;
        [FieldValueDiscarded]
        string category;
        [FieldValueDiscarded]
        string mode;
        [FieldValueDiscarded]
        string type;
        [FieldValueDiscarded]
        string result;
        [FieldHidden]
        public int roomNum;

        public override string ToString()
        {
            return termId + " " + userId + " " + name + " " + string.Format("{0:H:mm:ss}", hms);
        }

        public int GetRoomNum()
        {
            return roomNum;
        }

        public void BeforeRead(BeforeReadEventArgs e)
        {
            // throw new NotImplementedException();
        }

        public void AfterRead(AfterReadEventArgs e)
        {
          
            if (name.StartsWith("A"))
            {
                //roomNum = Int32.Parse(Regex.Replace(name, @"\D", ""));
                //int subEndNum = name.IndexOf("(");   
            }
            else
            {
                int subEndNum = name.IndexOf("(");
                roomNum = int.Parse(name.Substring(subEndNum + 1, 3));
                name = Regex.Replace(name.Substring(0, subEndNum), @"\d", "");
            }

        }

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
