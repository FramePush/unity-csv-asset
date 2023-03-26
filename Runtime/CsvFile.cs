using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FramePush.Csv
{
    [CreateAssetMenu]
    public class CsvFile : ScriptableObject
    {
        public TextAsset source;
        public bool containsHeaders;

        public Record[] Records { get; private set; }
        public string[] Headers { get; private set; }

        private enum ParseState
        {
            EndRecord,
            Field,
            EnclosedField,
            EndField,
            EnclosedEscape,
            EndingRecord,
        }

        public static List<Record> Parse(string text)
        {
            var records = new List<Record>();
            List<string> fields = null;
            var field = new StringBuilder();
            var futureHeaders = new Dictionary<string, int>();

            var state = ParseState.EndRecord;

            void EndField()
            {
                fields?.Add(field.ToString());
                field.Clear();
                state = ParseState.EndField;
            }

            void EndRecord()
            {
                EndField();
                records.Add(new Record { Fields = fields?.ToArray(), HeaderMap = futureHeaders });
                fields = null;
                state = ParseState.EndRecord;
            }

            void ParseField(char c)
            {
                switch (c)
                {
                    case ',':
                        EndField();
                        break;
                    case '\r':
                        state = ParseState.EndingRecord;
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
                        // Double quote is the escape character inside a field enclosed in double quotes
                        // It could also end the field
                        // Next char is either escaped char or end of field
                        state = ParseState.EnclosedEscape;
                        break;
                    default:
                        field.Append(c);
                        break;
                }
            }

            void ParseEscaped(char c)
            {
                ParseField(c);
                if (c == '"')
                {
                    // Double quote is the only escaped character
                    state = ParseState.EnclosedField;
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
                        fields = new List<string>();
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
                        ParseEscaped(c);
                        break;
                    case ParseState.EndingRecord:
                        if (c != '\n')
                        {
                            throw new FormatException("Unexpected character after carriage return");
                        }
                        EndRecord();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (state != ParseState.EndRecord)
            {
                EndRecord();
            }

            return records;
        }

        public static (Record[] record, string[] headers) Parse(string text, bool containsHeaders)
        {
            var records = Parse(text);

            if (!containsHeaders)
                return (records.ToArray(), null);
            
            var headers = records[0];
            for (var i = 0; i < headers.Fields.Length; ++i)
                headers.HeaderMap[headers[i]] = i;

            records.RemoveAt(0);

            return (records.ToArray(), headers.Fields);
        }

        public void Parse()
        {
            (Records, Headers) = Parse(source.text, containsHeaders);
        }
    }
}
