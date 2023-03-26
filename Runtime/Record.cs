using System;
using System.Collections;
using System.Collections.Generic;

namespace FramePush.Csv
{
    [Serializable]
    public class Record : IEnumerable<string>
    {
        public string[] Fields { get; set; }
        
        public IDictionary<string, int> HeaderMap { get; set; }

        public string this[int index]
        {
            get => Fields[index];
            set => Fields[index] = value;
        }
        
        public string this[string header]
        {
            get
            {
                if (HeaderMap is null)
                    throw new InvalidOperationException("No headers specified");

                return Fields[HeaderMap[header]];
            }
            set
            {
                if (HeaderMap is null)
                    throw new InvalidOperationException("No headers specified");

                Fields[HeaderMap[header]] = value;
            }
        }

        public int Length => Fields.Length;

        public IEnumerator GetEnumerator()
        {
            return Fields.GetEnumerator();
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return ((IEnumerable<string>)Fields).GetEnumerator();
        }

        public static implicit operator string[](Record record)
        {
            return record.Fields;
        }
    }
}
