using Microsoft.VisualStudio.TestTools.UnitTesting;
using PraksaProjektBackend.Services;

namespace UnitTests
{
    [TestClass]
    public class CurrencyConverterTest
    {
        [TestMethod]
        public void TestUSD_To_BAM()
        {
            double usd = 1;
            double expected = 1.81;

            double actual = CurrencyConverter.USD_To_BAM(usd);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void TestBAM_To_USD()
        {
            double bam = 1;
            double expected = 0.55;

            double actual = CurrencyConverter.BAM_To_USD(bam);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void TestBAM_To_HRK()
        {
            double bam = 1;
            double expected = 3.86;

            double actual = CurrencyConverter.BAM_To_HRK(bam);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void TestHRK_To_BAM()
        {
            double hrk = 1;
            double expected = 0.26;

            double actual = CurrencyConverter.HRK_To_BAM(hrk);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void TestHRK_To_USD()
        {
            double hrk = 1;
            double expected = 0.14;

            double actual = CurrencyConverter.HRK_To_USD(hrk);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void TestUSD_To_HRK()
        {
            double usd = 1;
            double expected = 6.99;

            double actual = CurrencyConverter.USD_To_HRK(usd);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void TestEUR_To_HRK()
        {
            double eur = 1;
            double expected = 7.56;

            double actual = CurrencyConverter.EUR_To_HRK(eur);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void TestHRK_To_EUR()
        {
            double hrk = 1;
            double expected = 0.13;

            double actual = CurrencyConverter.HRK_To_EUR(hrk);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void TestBAM_To_EUR()
        {
            double bam = 1;
            double expected = 0.51;

            double actual = CurrencyConverter.BAM_To_EUR(bam);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void TestEUR_To_BAM()
        {
            double eur = 1;
            double expected = 1.96;

            double actual = CurrencyConverter.EUR_To_BAM(eur);

            Assert.AreEqual(expected, actual);

        }
    }
}