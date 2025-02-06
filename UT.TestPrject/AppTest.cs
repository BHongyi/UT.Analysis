using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT.TestPrject
{
    [TestClass]
    public class AppTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            UT.Project.App app = new UT.Project.App();
            app.Execute(1,"1","13");
        }

        [TestMethod]
        public void TestMethod2()
        {
            UT.Project.App app = new UT.Project.App();
            app.Execute(2, "1", "13");
        }

        [TestMethod]
        public void TestMethod3()
        {
            UT.Project.App app = new UT.Project.App();
            app.Execute(1, "2", "13");
        }

        [TestMethod]
        public void TestMethod4()
        {
            UT.Project.App app = new UT.Project.App();
            app.Execute(1, "1", "1");
        }

        [TestMethod]
        public void TestMethod5()
        {
            UT.Project.App app = new UT.Project.App();
            app.Execute(1, "1", "14");
        }
    }
}
