using System;

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
        public Transaction(int id, DateTime date, float amount, TransactionType type, string description)
        {
            ID = id;
            Date = date;
            Amount = amount;
            Type = type;
            Description = description;
        }

        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Дата и время выполнения.
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Сумма.
        /// </summary>
        public float Amount { get; }

        /// <summary>
        /// Тип.
        /// </summary>
        public TransactionType Type { get; }

        /// <summary>
        /// Описание.
        /// </summary>
        public string Description { get; }
    }
}
