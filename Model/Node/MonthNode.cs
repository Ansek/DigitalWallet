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
            TransactionsBegin = null;
            NextMonth = null;
        }

        /// <summary>
        /// Отслеживаемый месяц.
        /// </summary>
        public int Month { get; }

        /// <summary>
        /// Начало списка транзакций для данного месяца.
        /// </summary>
        public TransactionNode TransactionsBegin { get; set; }

        /// <summary>
        /// Список транзакций для следующего месяца.
        /// </summary>
        public MonthNode NextMonth { get; set; }

        /// <summary>
        /// Проверка следующий условий.<br/>
        /// - t1 является доходом;<br/>
        /// - t1.Amount больше t2.Amount;<br/>
        /// - t1.Date меньше t2.Date.
        /// </summary>
        /// <param name="t1">Транзакция 1.</param>
        /// <param name="t2">Транзакция 2.</param>
        /// <returns><see langword="true"/> - если выполнены все условия.</returns>
        private bool CheckСondition(Transaction t1, Transaction t2)
        {
            if (t1.Type != t2.Type)
                return t1.Type == TransactionType.Income;
            if (t1.Amount != t2.Amount)
                return t1.Amount > t2.Amount;
            return t1.Date < t2.Date;
        }

        /// <summary>
        /// Создает новый узёл транзакциии.
        /// </summary>
        /// <param name="transaction">Добавляемая транщакция.</param>
        public void CreateTransactionNode(Transaction transaction)
        {
            if (TransactionsBegin == null)
            {
                TransactionsBegin = new TransactionNode(transaction);
            }
            else if (CheckСondition(transaction, TransactionsBegin.Transaction))
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
                while (node.Next != null && CheckСondition(node.Next.Transaction, transaction))
                    node = node.Next;
                var temp = node.Next;
                node.Next = new TransactionNode(transaction);
                node = node.Next;
                node.Next = temp;
            }
        }
    }
}
