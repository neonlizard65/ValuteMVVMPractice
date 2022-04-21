using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using ValuteMVVMPractice.ViewModels;

namespace ValuteTesting
{
    [TestClass]
    public class CurrencyViewModelTests
    {
        [TestMethod]
        public void GetConversion()
        {
            //Arrange
            ValCursViewModel vals = new ValCursViewModel();
            var val1 = vals.Valute.Where(x => x.CharCode == "UZS").FirstOrDefault();
            var val2 = vals.Valute.Where(x => x.CharCode == "KRW").FirstOrDefault();

            var expected = (val1.Value / val1.Nominal) * 20 / ((val2.Value / val2.Nominal)); 

            //Act
            var result = vals.GetConversion(20, val1, val2);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
