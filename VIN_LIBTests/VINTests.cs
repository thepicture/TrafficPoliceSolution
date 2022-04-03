using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VIN_LIB.Tests
{
    [TestClass()]
    public class VINTests
    {
        private readonly VIN vin = new VIN();
        [TestMethod()]
        public void CheckVIN_InputIsJHMCM56557C404453_ReturnsTrue()
        {
            // Arrange.
            bool expected = true;
            // Act.
            bool actual = vin.CheckVIN("JHMCM56557C404453");
            // Assert.
            Assert.AreEqual(expected, actual);
        }
    }
}