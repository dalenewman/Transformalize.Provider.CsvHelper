using Autofac;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Providers.File;

namespace Transformalize.Providers.CsvHelper.Autofac {
   public class CsvHelperProviderModule : Module {
      private readonly Stream _stream;

      public CsvHelperProviderModule(Stream stream = null) {
         _stream = stream;
      }

      protected override void Load(ContainerBuilder builder) {

         if (!builder.Properties.ContainsKey("Process")) {
            return;
         }

         var p = (Process)builder.Properties["Process"];

         // connections
         foreach (var connection in p.Connections.Where(c => c.Provider == "file")) {

            // Schema Reader
            builder.Register<ISchemaReader>(ctx => {
               // todo
               return new NullSchemaReader();
            }).Named<ISchemaReader>(connection.Key);
         }

         // entity input
         foreach (var entity in p.Entities.Where(e => p.Connections.First(c => c.Name == e.Input).Provider == "file")) {

            // input version detector
            builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

            // input read
            builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));


               //if (input.Connection.Delimiter == string.Empty && input.Entity.Fields.Count(f => f.Input) == 1) {
               //   return new FileReader(input, rowFactory);
               //}
               //return new DelimitedFileReader(input, rowFactory);
               return new NullReader(input, true);
            }).Named<IRead>(entity.Key);

         }

         // Entity Output
         if (p.GetOutputConnection().Provider == "file") {

            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in p.Entities) {

               // ENTITY OUTPUT CONTROLLER
               builder.Register<IOutputController>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  var fileInfo = new FileInfo(Path.Combine(output.Connection.Folder, output.Connection.File ?? output.Entity.OutputTableName(output.Process.Name)));
                  var folder = Path.GetDirectoryName(fileInfo.FullName);
                  var init = p.Mode == "init" || (folder != null && !Directory.Exists(folder));
                  var initializer = init ? (IInitializer)new FileInitializer(output) : new NullInitializer();
                  return new FileOutputController(output, initializer, new NullInputProvider(), new NullOutputProvider());
               }).Named<IOutputController>(entity.Key);

               // ENTITY WRITER
               builder.Register<IWrite>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                  switch (output.Connection.Provider) {
                     case "file":
                        if (output.Connection.Delimiter == string.Empty) {
                           // return new FileStreamWriter(output);
                           return new NullWriter(output, true);
                        } else {
                           if (output.Connection.Stream &&  _stream != null) {
                              return new CsvHelperStreamWriter(output, _stream);
                           } else {
                              var fileInfo = new FileInfo(Path.Combine(output.Connection.Folder, output.Connection.File ?? output.Entity.OutputTableName(output.Process.Name)));
                              var stream = System.IO.File.OpenWrite(fileInfo.FullName);
                              return new CsvHelperStreamWriter(output, stream);
                           }
                           
                        }
                     default:
                        return new NullWriter(output, true);
                  }
               }).Named<IWrite>(entity.Key);

            }
         }
      }
   }
}
