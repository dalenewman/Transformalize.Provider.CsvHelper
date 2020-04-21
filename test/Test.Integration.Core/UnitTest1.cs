using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.CsvHelper.Autofac;

namespace Test.Integration.Core {
   [TestClass]
   public class UnitTest1 {
      [TestMethod]
      public void Write() {

         const string xml = @"<add name='file' mode='init' read-only='true'>
  <parameters>
    <add name='Size' type='int' value='1000' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='file' delimiter=',' file='c:\temp\bogus.csv' />
  </connections>
  <entities>
    <add name='Contact' size='@[Size]'>
      <fields>
        <add name='Identity' type='int' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
    </add>
  </entities>
</add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new CsvHelperProviderModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               Assert.AreEqual((uint)1000, process.Entities.First().Inserts);
            }
         }

      }

      [TestMethod]
      public void WriteWithSomeLineBreaks() {
         const string xml = @"<add name='file' mode='init' read-only='true'>
  <connections>
    <add name='input' provider='internal' />
    <add name='output' provider='file' delimiter=',' file='c:\temp\data-with-line-breaks-and-commas.csv' text-qualifier='""' />
  </connections>
  <entities>
    <add name='Contact'>
      <rows>
        <add Identity='1' FirstName='Dale' LastName='Newman' Stars='1' Reviewers='1' />
        <add Identity='2' FirstName='Dale
 Jr' LastName='Newman,s' Stars='2' Reviewers='2' />
      </rows>
      <fields>
        <add name='Identity' type='int' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' />
        <add name='Reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new CsvHelperProviderModule()).CreateScope(process, new ConsoleLogger(LogLevel.Debug))) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }

   }
}
