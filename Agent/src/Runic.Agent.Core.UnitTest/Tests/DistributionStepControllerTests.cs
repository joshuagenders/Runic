using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class DistributionStepControllerTests
    {
        [TestMethod]
        public void WhenSingleStepIsUsed_NextStepAlwaysReturnsStep()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void WhenStepsHaveNoCommonFactor_ThenQueueSizeIsSumOfDistributionAllocation()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void WhenQueueIsEmptied_QueueIsRefreshed()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void WhenStepsHaveCommonFactor_ThenQueueSizeIsSumOfDitributionAllocationOverGCF()
        {
            throw new NotImplementedException();
        }
    }
}
