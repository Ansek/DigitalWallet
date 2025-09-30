using DigitalWallet.Model.Node;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DigitalWallet.Model
{
    /// <summary>
    /// Кошелек.
    /// </summary>
    public class Wallet
    {
        /// <summary>
        /// Начало списка транзакций, отсортированных по датам.
        /// </summary>
        private TransactionNode _transactionsSortedByDateBegin = null;

        /// <summary>
        /// Начало списка транзакций по годам.
        /// </summary>
        private YearNode _transactionsByYearBegin = null;

        /// <summary>
        /// Кошелек.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="name">Название.</param>
        /// <param name="currency">Валюта.</param>
        /// <param name="openingBalance">Начальный баланс.</param>
        public Wallet(ulong id, string name, Currency currency, double openingBalance)
        {
            if (openingBalance < 0)
                throw new ArgumentException("Баланс должен быть равен или больше 0.");
            ID = id;
            Name = name;
            Currency = currency;
            OpeningBalance = openingBalance;
        }

        /// <summary>
        /// Идентификатор.
        /// </summary>
        public ulong ID { get; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Валюта.
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Начальный баланс.
        /// </summary>
        public double OpeningBalance { get; }


        /// <summary>
        /// Добавляет транзакцию в общий отсортированный список.
        /// Выбрасывает исключение в случае несоответствия данных.
        /// </summary>
        /// <param name="transaction">Запись транзакции.</param>
        /// <exception cref="AddTransactionException"></exception>
        private void CheckAndCreateNode(Transaction transaction)
        {
            if (_transactionsSortedByDateBegin == null)
            {
                _transactionsSortedByDateBegin = new TransactionNode(transaction);
                return;
            }
            TransactionNode prev = null;
            var node = _transactionsSortedByDateBegin;
            double prevAmoutSum = 0;
            double nextAmoutSum = 0;
            while (node != null)
            {
                if (node.Transaction.ID == transaction.ID)
                {
                    var msg = $"Идентифактор транзакции (ID = {transaction.ID}) уже был задан ранее.";
                    throw new AddTransactionException(AddTransactionException.Type.RepeatID, msg);
                }
                // 3
                // 5 4 2 1
                if (node.Transaction.Date > transaction.Date)
                {
                    prev = node;
                    prevAmoutSum += node.Transaction;
                }
                else
                {                    
                    nextAmoutSum += node.Transaction;
                }
                node = node.Next;
            }
            var balance = OpeningBalance + nextAmoutSum;
            if (balance + transaction < 0)
            {
                var date = transaction.Date.ToString();
                var msg = $"К {date} баланс составляет: {balance}, что меньше указанной суммы: {transaction.Amount}.";
                throw new AddTransactionException(AddTransactionException.Type.NextDateSumErr, msg);
            }
            if (balance + transaction + prevAmoutSum < 0)
            {
                var date = transaction.Date.ToString();
                var msg = $"Баланс с текущей транзакцией составит {balance + transaction}" +
                    $", однако были найдены записи после {date} c cуммой {prevAmoutSum}" +
                    ", что приводит к отрицательному результату.";
                throw new AddTransactionException(AddTransactionException.Type.PrevDateSumErr, msg);
            }
            if (prev == null)
            {
                node = new TransactionNode(transaction);
                node.Next = _transactionsSortedByDateBegin;
                _transactionsSortedByDateBegin = node;
            } 
            else
            {
                node = prev.Next;
                prev.Next = new TransactionNode(transaction);
                prev.Next.Next = node;
            }
        }

        /// <summary>
        /// Возращает или создает узел для списка транзакций по годам.
        /// </summary>
        /// <param name="year">Отслеживаемый год.</param>
        /// <returns>Узел для указанного года.</returns>
        private YearNode GetOrCreateYearNode(int year)
        {
            YearNode yNode;
            if (_transactionsByYearBegin == null)
            {
                yNode = new YearNode(year);
                _transactionsByYearBegin = yNode;
            }
            else if (_transactionsByYearBegin.Year < year)
            {
                yNode = new YearNode(year)
                {
                    NextYear = _transactionsByYearBegin
                };
                _transactionsByYearBegin = yNode;
            }
            else
            {
                yNode = _transactionsByYearBegin;
                while (yNode.NextYear != null && year <= yNode.NextYear.Year)
                    yNode = yNode.NextYear;
                if (yNode.Year != year)
                {
                    YearNode yTemp = yNode.NextYear;
                    yNode.NextYear = new YearNode(year);
                    yNode = yNode.NextYear;
                    yNode.NextYear = yTemp;
                }
            }
            return yNode;
        }

        /// <summary>
        /// Добавляет транзакцию к кошельку.
        /// </summary>
        /// <param name="transaction">Запись транзакции.</param>
        public void AddTransaction(Transaction transaction)
        {
            CheckAndCreateNode(transaction);
            YearNode yNode = GetOrCreateYearNode(transaction.Date.Year);
            MonthNode mNode = yNode.GetOrCreateMonthNode(transaction.Date.Month);
            mNode.CreateTransactionNode(transaction);
        }

        /// <summary>
        /// Возращает список год и месяц, по которым доступны списки транзакций.
        /// День равен 1.
        /// </summary>
        /// <returns>Перечисление объектов <see cref="DateTime"/>.</returns>
        public IEnumerable<DateTime> GetYearMonthOfTransactions()
        {
            YearNode yNode = _transactionsByYearBegin;
            while (yNode != null)
            {
                int year = yNode.Year;
                MonthNode mNode = yNode.TransactionsByMonthBegin;
                while (mNode != null)
                {
                    int month = mNode.Month;
                    yield return new DateTime(year, month, 01);
                    mNode = mNode.NextMonth;
                }
                yNode = yNode.NextYear;
            }
        }

        /// <summary>
        /// Возращает список транзакций, отсортированных по датам.
        /// </summary>
        /// <returns>Перечисление объектов <see cref="Transaction"/>.</returns>
        public IEnumerable<Transaction> GetTransactionsSortedByDate()
        {
            var node = _transactionsSortedByDateBegin;
            while (node != null)
            {
                yield return node.Transaction;
                node = node.Next;
            }
        }

        /// <summary>
        /// Возращает список транзакций для конкретного года и месяца.
        /// </summary>
        /// <param name="year">Отслеживаемый год.</param>
        /// <param name="month">Отслеживаемый месяц.</param>
        /// <returns>Перечисление объектов <see cref="Transaction"/>.</returns>
        public IEnumerable<Transaction> GetTransactionsByYearMonth(int year, int month)
        {
            YearNode yNode = _transactionsByYearBegin;
            while (yNode != null)
            {
                MonthNode mNode = yNode.TransactionsByMonthBegin;
                while (mNode != null)
                {
                    var node = mNode.TransactionsBegin;
                    while (node != null)
                    {
                        yield return node.Transaction;
                        node = node.Next;
                    }
                    mNode = mNode.NextMonth;
                }
                yNode = yNode.NextYear;
            }
        }
    }
}
