namespace DigitalWallet.Model.Node
{
    /// <summary>
    /// Узел для списка транзакций по месяцам.
    /// </summary>
    internal class MonthNode
    {
        /// <summary>
        /// Узел для списка транзакций по месяцам.
        /// </summary>
        /// <param name="month">Отслеживаемый месяц.</param>
        public MonthNode(int month)
        {
            Month = month;
            Amount = 0;
            TransactionsBegin = null;
            NextMonth = null;
        }

        /// <summary>
        /// Отслеживаемый месяц.
        /// </summary>
        public int Month { get; }

        /// <summary>
        /// Месячная сумма транзакций.
        /// </summary>
        public double Amount { get; private set; }

        /// <summary>
        /// Начало списка транзакций для данного месяца.
        /// </summary>
        public TransactionNode TransactionsBegin { get; set; }

        /// <summary>
        /// Список транзакций для следующего месяца.
        /// </summary>
        public MonthNode NextMonth { get; set; }

        /// <summary>
        /// Создает новый узёл транзакциии 
        /// </summary>
        /// <param name="transaction">Добавляемая транщакция</param>
        public void CreateTransactionNode(Transaction transaction)
        {
            if (TransactionsBegin == null)
            {
                TransactionsBegin = new TransactionNode(transaction);
            }
            else if (TransactionsBegin.Transaction.Date < transaction.Date)
            {
                var node = new TransactionNode(transaction)
                {
                    Next = TransactionsBegin
                };
                TransactionsBegin = node;
            }
            else
            {
                var node = TransactionsBegin;
                while (node.Transaction.Date < transaction.Date && node.Next != null)
                    node = node.Next;
                var temp = node.Next;
                node.Next = new TransactionNode(transaction)
                {
                    Next = temp
                };
            }
        }
    }
}
