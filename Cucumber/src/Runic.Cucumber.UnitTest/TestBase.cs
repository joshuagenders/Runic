﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Cucumber.UnitTest
{
    public class TestBase
    {
        protected TestEnvironment TestEnvironment { get; set; }

        [TestInitialize]
        public void Init()
        {
            TestEnvironment = new TestEnvironment();
        }
    }
}
