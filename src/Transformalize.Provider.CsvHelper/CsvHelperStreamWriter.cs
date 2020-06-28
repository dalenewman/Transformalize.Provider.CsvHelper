using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.CsvHelper {

   /// <summary>
   /// </summary>
   public class CsvHelperStreamWriter : IWrite {

      private readonly OutputContext _context;
      private readonly Stream _stream;
      private readonly CsvConfiguration _config;

      public CsvHelperStreamWriter(OutputContext context, Stream stream) {
         _context = context;
         _stream = stream;

         _config = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture) {
            Delimiter = string.IsNullOrEmpty(context.Connection.Delimiter) ? "," : context.Connection.Delimiter,
            IgnoreBlankLines = true,
            SanitizeForInjection = false
         };

         if (!string.IsNullOrEmpty(context.Connection.TextQualifier)) {
            _config.Quote = context.Connection.TextQualifier[0];
         }
         _config.Encoding = Encoding.GetEncoding(_context.Connection.Encoding);
         _config.TrimOptions = TrimOptions.None;
      }

      public void Write(IEnumerable<IRow> rows) {

         var writer = new StreamWriter(_stream);
         var csv = new CsvWriter(writer, _config);

         try {
            if (_context.Connection.Header == Constants.DefaultSetting) {
               foreach (var field in _context.OutputFields) {
                  csv.WriteField(field.Label == string.Empty ? field.Alias : field.Label);
               }
               csv.NextRecordAsync();
            }

            csv.Context.HasHeaderBeenWritten = true;

            foreach (var row in rows) {
               foreach (var field in _context.OutputFields) {

                  string strVal;

                  switch (field.Type) {
                     case "byte[]":
                        strVal = Convert.ToBase64String((byte[])row[field]);
                        break;
                     case "string":
                        strVal = row[field] is string ? (string)row[field] : row[field].ToString();
                        break;
                     case "datetime":
                        var format = field.Format == string.Empty ? "o" : field.Format.Replace("AM/PM", "tt");
                        strVal = row[field] is DateTime ? ((DateTime)row[field]).ToString(format) : Convert.ToDateTime(row[field]).ToString(format);
                        break;
                     case "float":
                     case "decimal":
                     case "single":
                     case "double":
                        if (field.Format == string.Empty) {
                           strVal = row[field].ToString();
                        } else {
                           switch (field.Type) {
                              case "single":
                              case "float":
                                 strVal = row[field] is float ? ((float)row[field]).ToString(field.Format) : Convert.ToSingle(row[field]).ToString(field.Format);
                                 break;
                              case "decimal":
                                 strVal = row[field] is decimal ? ((decimal)row[field]).ToString(field.Format) : Convert.ToDecimal(row[field]).ToString(field.Format);
                                 break;
                              case "double":
                                 strVal = row[field] is double ? ((double)row[field]).ToString(field.Format) : Convert.ToDouble(row[field]).ToString(field.Format);
                                 break;
                              default:
                                 strVal = row[field].ToString();
                                 break;
                           }
                        }
                        break;
                     default:
                        strVal = row[field].ToString();
                        break;
                  }
                  csv.WriteField(strVal);
                  
               }
               
               _context.Entity.Inserts++;
               csv.NextRecordAsync();
            }

         } finally {
            if (csv != null) {
               csv.FlushAsync();
               csv.DisposeAsync();
            }
         }
      }
   }
}
