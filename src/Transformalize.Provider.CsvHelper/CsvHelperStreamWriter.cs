using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.CsvHelper {

   public class CsvHelperStreamWriter : CsvHelperWriterBase, IWrite, IDisposable {

      private readonly OutputContext _context;
      private readonly Stream _stream;
      private readonly CsvWriter _csv;

      public CsvHelperStreamWriter(OutputContext context, Stream stream) : base(context) {
         _context = context;
         _stream = stream;
         _csv = new CsvWriter(new StreamWriter(_stream), Config);
      }

      public void Write(IEnumerable<IRow> rows) {



         if (_context.Connection.Header == Constants.DefaultSetting) {
            WriteHeader(_csv);
            _csv.NextRecordAsync().ConfigureAwait(false);
         }

         foreach (var row in rows) {
            WriteRow(_csv, row);
            _context.Entity.Inserts++;
            _csv.NextRecordAsync().ConfigureAwait(false);
            _csv.FlushAsync().ConfigureAwait(false);
         }

         _csv.FlushAsync().ConfigureAwait(false);
      }

      public void Dispose() {
         if(_csv != null) {
            _csv.Dispose();
         }
      }
   }
}
