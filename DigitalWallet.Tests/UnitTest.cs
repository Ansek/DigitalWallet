using DigitalWallet.Model;

namespace DigitalWallet.Tests
{
    public class UnitTest
    {
        /// <summary>
        /// ����������� ���������� ����������� ����� ���������� �� �������,
        /// ��������������� �� �������� ���� � ������.<br/>
        /// ������ ������������ ��� �������������� ������������ ����������  
        /// � ���, ����� ����� � ��� ����� ������ ��� ��������� ������ �
        /// ����������� �� ����������� ������.
        /// </summary>
        [Fact]
        public void AddTransaction_CorrectYearMonthOrder()
        {
            // Arrange
            List<DateTime> dates1 =
            [
                new DateTime(2020, 05, 01),
                new DateTime(2022, 05, 02),
                new DateTime(2016, 05, 03),
                new DateTime(2018, 05, 04),
                new DateTime(2020, 01, 05),
                new DateTime(2020, 10, 06),
            ];
            var wallet = new Wallet(0, "Te������", new Currency("RUB", "�����"), 0);
            for (int i = 0; i < dates1.Count; i++)
                wallet.AddTransaction(new Transaction((ulong)i, dates1[i], 100, TransactionType.Income, "�����������"));

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

        /// <summary>
        /// ����������� ���������� ����������� ����������, 
        /// ��������������� �� ���� Date.<br/>
        /// ������ ������������ ��� ������� ����� �� ���� Amount.<br/>
        /// ��������������, ��� �������� � ���������� ����� ���������, 
        /// ��������, ������ �������� � ���������� ��������, ������� 
        /// ���������� ����� ���� ��������� � ��������.
        /// </summary>
        [Fact]
        public void AddTransaction_CorrectTransactionsOrder()
        {
            // Arrange
            List<DateTime> dates1 =
            [
                new DateTime(2022, 05, 01),
                new DateTime(2018, 05, 01),
                new DateTime(2020, 01, 01),
                new DateTime(2020, 10, 01),
                new DateTime(2020, 10, 05),
                new DateTime(2020, 10, 10),
                new DateTime(2020, 10, 02),
                new DateTime(2020, 10, 02, 15, 0, 0),
                new DateTime(2020, 10, 02, 10, 0, 0),
            ];
            var wallet = new Wallet(0, "Te������", new Currency("RUB", "�����"), 0);
            for (int i = 0; i < dates1.Count; i++)
                wallet.AddTransaction(new Transaction((ulong)i, dates1[i], 100, TransactionType.Income, "�����������"));

            // Act
            List<DateTime> dates2 = [];
            foreach (var transaction in wallet.GetTransactionsSortedByDate())
                dates2.Add(transaction.Date);

            // Assert
            List<DateTime> dates3 =
            [
                new DateTime(2022, 05, 01),
                new DateTime(2020, 10, 10),
                new DateTime(2020, 10, 05),
                new DateTime(2020, 10, 02, 15, 0, 0),
                new DateTime(2020, 10, 02, 10, 0, 0),
                new DateTime(2020, 10, 02),
                new DateTime(2020, 10, 01),
                new DateTime(2020, 01, 01),
                new DateTime(2018, 05, 01)
            ];
            Assert.Equal(dates3, dates2);
        }

        /// <summary>
        /// ����������� ���������� �������� �������:<br/>
        /// ��������� ������: 200 ���.<br/>
        /// 01.09.2025 ��������� 300 ���.<br/>
        /// 02.09.2025 ������� 400 ���.<br/>
        /// ������� ������: 100 ���.
        /// </summary>
        [Fact]
        public void AddTransaction_CorrectSum()
        {
            // Arrange
            var wallet = new Wallet(0, "Te������", new Currency("RUB", "�����"), 200);
            wallet.AddTransaction(new Transaction(1, new DateTime(2025, 09, 01), 300, TransactionType.Income, "���������"));
            wallet.AddTransaction(new Transaction(2, new DateTime(2025, 09, 02), 400, TransactionType.Expense, "�������"));

            // Act
            double balance = wallet.Balance;

            // Assert
            Assert.Equal(100, balance);
        }

        /// <summary>
        /// ����������� ������ �� ������������ �������������� ����������.
        /// </summary>
        [Fact]
        public void AddTransactionException_RepeatID()
        {
            // Arrange
            uint id = 1;
            var wallet = new Wallet(0, "Te������", new Currency("RUB", "�����"), 0);
            wallet.AddTransaction(new Transaction(id, new DateTime(), 100, TransactionType.Income, "�����������"));

            // Act & assert
            var exc = Assert.Throws<AddTransactionException>(() =>
            {
                wallet.AddTransaction(new Transaction(id, new DateTime(), 100, TransactionType.Income, "�����������"));
            });

            // Assert
            Assert.Equal(AddTransactionException.Type.RepeatID, exc.Code);
        }

        /// <summary>
        /// ��������� ������� �����������, ��� ���������� ����� ���� ��������� � �������� 
        /// (������ � ����������� �� ������ �� ������ ����������), �� ����������� ������:<br/>
        /// 01.09.2025 ��������� 200 ���.<br/>
        /// 03.09.2025 ������� 150 ���.<br/>
        /// ������� �� �������: 50 ���.<br/>
        /// � ����������� ������ ����������:<br/>
        /// 02.09.2025 ������� 100 ���.<br/>
        /// ����� ������� ���������: -50 ���.<br/>
        /// ��� �����������, ������� ����� ��������� ����������.
        /// </summary>
        [Fact]
        public void AddTransactionException_PrevDateSumErr()
        {
            // Arrange
            var wallet = new Wallet(0, "Te������", new Currency("RUB", "�����"), 0);
            wallet.AddTransaction(new Transaction(1, new DateTime(2025, 09, 01), 200, TransactionType.Income, "�����"));
            wallet.AddTransaction(new Transaction(3, new DateTime(2025, 09, 03), 150, TransactionType.Expense, "������"));

            // Act & assert
            var exc = Assert.Throws<AddTransactionException>(() =>
            {
                wallet.AddTransaction(new Transaction(2, new DateTime(2025, 09, 01), 100, TransactionType.Expense, "������"));
            });
            
            // Assert
            Assert.Equal(AddTransactionException.Type.PrevDateSumErr, exc.Code);
        }

        /// <summary>
        /// ��������, ��� ����� �� ����� ��������� ������� ������:<br/>
        /// 01.09.2025 ��������� 100 ���.<br/>
        /// 02.09.2025 ������� 200 ���.<br/>
        /// ������ ����������.
        /// </summary>
        [Fact]
        public void AddTransactionException_NextDateSumErr()
        {
            // Arrange
            var wallet = new Wallet(0, "Te������", new Currency("RUB", "�����"), 0);
            wallet.AddTransaction(new Transaction(0, new DateTime(2025, 09, 01), 100, TransactionType.Income, "�����"));

            // Act & assert
            var exc = Assert.Throws<AddTransactionException>(() =>
            {
                wallet.AddTransaction(new Transaction(1, new DateTime(2025, 09, 02), 200, TransactionType.Expense, "������"));
            });

            // Assert
            Assert.Equal(AddTransactionException.Type.NextDateSumErr, exc.Code);
        }

        /// <summary>
        /// ����������� ���������� ����������� ����������, �������� �������� ��� ���������� ������:<br/>
        /// - ������������� ��� ���������� �� ���� (Income/Expense);<br/>
        /// - ������������� ������ �� ����� ����� (�� ��������);<br/>
        /// - � ������ ������ ������������� ���������� �� ���� (�� ����� ������ � ����� �����).
        /// </summary>
        [Fact]
        public void GetTransactionsByYearMonth_CorrectTransactionsOrder()
        {
            // Arrange
            var data1 = new List<Tuple<ulong, TransactionType, double, DateTime>>()
            {
                new(1, TransactionType.Income, 200, new DateTime(2025, 01, 01)),
                new(2, TransactionType.Expense, 100, new DateTime(2025, 01, 01)),
                new(3, TransactionType.Income, 200, new DateTime(2025, 01, 01)),
                new(4, TransactionType.Expense, 300, new DateTime(2025, 01, 03)),
                new(5, TransactionType.Income, 400, new DateTime(2025, 01, 01)),
                new(6, TransactionType.Expense, 300, new DateTime(2025, 01, 02)),
                new(7, TransactionType.Income, 400, new DateTime(2025, 01, 01)),
                new(8, TransactionType.Expense, 300, new DateTime(2025, 01, 01)),
            };
            var wallet = new Wallet(0, "Te������", new Currency("RUB", "�����"), 0);
            foreach (var (id, type, amount, date) in data1)
            {
                wallet.AddTransaction(new Transaction(id, date, amount, type, ""));
            }

            // Act
            var data2 = new List<Tuple<ulong, TransactionType, double, DateTime>>();
            foreach (var transaction in wallet.GetTransactionsByYearMonth(2025, 01))
                data2.Add(new(transaction.ID, transaction.Type, transaction.Amount, transaction.Date));

            // Assert
            var data3 = new List<Tuple<ulong, TransactionType, double, DateTime>>()
            {
                new(5, TransactionType.Income, 400, new DateTime(2025, 01, 01)),
                new(7, TransactionType.Income, 400, new DateTime(2025, 01, 01)),
                new(1, TransactionType.Income, 200, new DateTime(2025, 01, 01)),
                new(3, TransactionType.Income, 200, new DateTime(2025, 01, 01)),
                new(8, TransactionType.Expense, 300, new DateTime(2025, 01, 01)),
                new(6, TransactionType.Expense, 300, new DateTime(2025, 01, 02)),
                new(4, TransactionType.Expense, 300, new DateTime(2025, 01, 03)),
                new(2, TransactionType.Expense, 100, new DateTime(2025, 01, 01))               
            };
            Assert.Equal(data3, data2);
        }
    }
}