using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FramePush.Csv
{
    [CreateAssetMenu]
    public class CsvFile : ScriptableObject
    {
        public TextAsset source;

        public string[][] Records
        {
            get;
            private set;
        }

        private enum ParseState
        {
            EndRecord,
            Field,
            EnclosedField,
            EndField,
            EnclosedEscape,
        }

        public static List<string[]> Parse(string text)
        {
            List<string[]> records = new List<string[]>();
            List<string> record = null;
            StringBuilder field = new StringBuilder();

            ParseState state = ParseState.EndRecord;

            void EndField()
            {
                record.Add(field.ToString());
                field.Clear();
                state = ParseState.EndField;
            }

            void EndRecord()
            {
                EndField();
                records.Add(record.ToArray());
                record = null;
                state = ParseState.EndRecord;
            }

            void ParseField(char c)
            {
                switch (c)
                {
                    case ',':
                        EndField();
                        break;
                    case '\n':
                        EndRecord();
                        break;
                    default:
                        field.Append(c);
                        break;
                }
            }

            void ParseEnclosedField(char c)
            {
                switch (c)
                {
                    case '"':
                        state = ParseState.EnclosedEscape;
                        break;
                    default:
                        field.Append(c);
                        break;
                }
            }

            void ParseEnclosedEscape(char c)
            {
                switch (c)
                {
                    case ',':
                        EndField();
                        break;
                    case '\n':
                        EndRecord();
                        break;
                    case '"':
                        field.Append(c);
                        state = ParseState.EnclosedField;
                        break;
                }
            }

            void CheckFieldStart(char c)
            {
                switch (c)
                {
                    case '"':
                        state = ParseState.EnclosedField;
                        break;
                    default:
                        ParseField(c);
                        state = ParseState.Field;
                        break;
                }
            }

            foreach (var c in text)
            {
                switch (state)
                {
                    case ParseState.EndRecord:
                        record = new List<string>();
                        CheckFieldStart(c);
                        break;
                    case ParseState.EndField:
                        CheckFieldStart(c);
                        break;
                    case ParseState.Field:
                        ParseField(c);
                        break;
                    case ParseState.EnclosedField:
                        ParseEnclosedField(c);
                        break;
                    case ParseState.EnclosedEscape:
                        ParseEnclosedEscape(c);
                        break;
                }
            }

            if (state != ParseState.EndRecord)
            {
                EndRecord();
            }

            return records;
        }

        public void Parse()
        {
            Records = Parse(source.text).ToArray();
        }
    }
}
