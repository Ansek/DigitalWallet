namespace DigitalWallet.Model.Node
{
    /// <summary>
    /// Узел для списка транзакций.
    /// </summary>
    internal class TransactionNode
    {
        /// <summary>
        /// Узел для списка транзакций.
        /// </summary>
        /// <param name="transaction">Текущая транзакция.</param>
        public TransactionNode(Transaction transaction)
        {
            Transaction = transaction;
            Next = null;
        }

        /// <summary>
        /// Текущая транзакция.
        /// </summary>
        public Transaction Transaction { get; }

        /// <summary>
        /// Следующая транзакция.
        /// </summary>
        public TransactionNode Next { get; set; }
    }
}
