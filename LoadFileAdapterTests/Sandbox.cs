using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Instructions;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class Sandbox
    {
        [TestMethod]
        public void SandboxTest()
        {
            string xml = System.IO.File.ReadAllText("X:\\dev\\TestData\\DAT_Edit.xml");
            Job job = Job.Deserialize(xml);
            job.Imports.First().File = new System.IO.FileInfo(@"X:\dev\TestData\SAMPLE.DAT");
            job.Exports.First().File = new System.IO.FileInfo(@"X:\dev\TestData\OUTPUT.DAT");
            Executor e = new Executor();
            e.Execute(job);
        }
    }
}
