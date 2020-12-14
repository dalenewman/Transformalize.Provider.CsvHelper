using CsvHelper;
using System.Collections.Generic;
using System.IO;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.CsvHelper {

   public class CsvHelperStreamWriterSync : CsvHelperWriterBase, IWrite {

      private readonly OutputContext _context;
      private readonly Stream _stream;

      public CsvHelperStreamWriterSync(OutputContext context, Stream stream) : base(context) {
         _context = context;
         _stream = stream;
      }

      public void Write(IEnumerable<IRow> rows) {

         var writer = new StreamWriter(_stream);
         var csv = new CsvWriter(writer, Config);

         try {
            if (_context.Connection.Header == Constants.DefaultSetting) {
               WriteHeader(csv);
               csv.NextRecord();
            }

            csv.Context.HasHeaderBeenWritten = true;

            foreach (var row in rows) {
               WriteRow(csv, row);
               _context.Entity.Inserts++;
               csv.NextRecord();
            }

         } finally {
            if (csv != null) {
               csv.Flush();
               csv.Dispose();
            }
         }
      }
   }
}
