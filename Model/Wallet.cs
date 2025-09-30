using System;
using System.Collections.Generic;
using DigitalWallet.Model.Node;

namespace DigitalWallet.Model
{
    /// <summary>
    /// Кошелек.
    /// </summary>
    public class Wallet
    {
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
