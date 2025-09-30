using DigitalWallet.Model;

namespace DigitalWallet.Tests
{
    public class UnitTest
    {
        [Fact]
        public void AddTransaction_CorrectYearMonthOrder()
        {
            // Arrange
            List<DateTime> dates1 =
            [
                new DateTime(2020, 05, 01),
                new DateTime(2022, 05, 01),
                new DateTime(2016, 05, 01),
                new DateTime(2018, 05, 01),
                new DateTime(2020, 01, 01),
                new DateTime(2020, 10, 01),
            ];
            var wallet = new Wallet(0, "Teстовый", new Currency("RUB", "Рубли"), 0);
            for (int i = 0; i < dates1.Count; i++)
                wallet.AddTransaction(new Transaction((ulong)i, dates1[i], 100, TransactionType.Income, "Поступление"));

            // Act
            List<DateTime> dates2 = [.. wallet.GetYearMonthOfTransactions()];

            // Assert
            List<DateTime> dates3 =
            [
                new DateTime(2022, 05, 01),
                new DateTime(2020, 10, 01),
                new DateTime(2020, 05, 01),
                new DateTime(2020, 01, 01),
                new DateTime(2018, 05, 01),
                new DateTime(2016, 05, 01)
            ];
            Assert.Equal(dates3, dates2);
        }
    }
}