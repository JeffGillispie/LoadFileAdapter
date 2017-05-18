using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Instructions;
using LoadFileAdapter;
using LoadFileAdapter.Transformers;

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

        interface IA { }
        interface IB { }
        abstract class A : IA { }
        abstract class B : IB { }
        class AX : A, IA { public AX(BX bx) { } }
        class AY : A, IA { public AY(BY by) { } }
        class BX : B, IB { }
        class BY : B, IB { }


        A test(B b)
        {
            A a = null; // new A(b);


            
            return a;
        }
    }
}
