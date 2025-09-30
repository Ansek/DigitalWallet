using System;
using System.Reflection;
using System.Xml.Linq;

namespace DigitalWallet.Model
{
    /// <summary>
    /// Транзакция.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Транзакция.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="date">Дата и время выполнения.</param>
        /// <param name="amount">Сумма.</param>
        /// <param name="type">Тип.</param>
        /// <param name="description">Описание.</param>
        public Transaction(ulong id, DateTime date, double amount, TransactionType type, string description)
        {
            if (amount <= 0)
                throw new ArgumentException("Сумма должна быть положительным значением.");
            ID = id;
            Date = date;
            Amount = amount;
            Type = type;
            Description = description;
        }

        /// <summary>
        /// Идентификатор.
        /// </summary>
        public ulong ID { get; }

        /// <summary>
        /// Дата и время выполнения.
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Сумма.
        /// </summary>
        public double Amount { get; }

        /// <summary>
        /// Тип.
        /// </summary>
        public TransactionType Type { get; }

        /// <summary>
        /// Описание.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Переопределение оператора сложения для суммирования
        /// поля Amount с учетом поля Type.
        /// </summary>
        /// <param name="d">Первое слагаемое.</param>
        /// <param name="t">Транзакция с полем Amount.</param>
        /// <returns>Сумма первого слагаемого и значения поля Amount.</returns>
        public static double operator +(double d, Transaction t)
        {
            if (t.Type == TransactionType.Income)
                return d + t.Amount;
            else
                return d - t.Amount;
        }
    }
}
