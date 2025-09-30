using System.Xml.Linq;

namespace DigitalWallet.Model.Node
{
    /// <summary>
    /// Узел для списка транзакций по годам.
    /// </summary>
    internal class YearNode
    {
        /// <summary>
        /// Узел для списка транзакций по годам.
        /// </summary>
        /// <param name="year">Отслеживаемый год.</param>
        public YearNode(int year)
        {
            Year = year;
            Amount = 0;
            TransactionsByMonthBegin = null;
            NextYear = null;
        }

        /// <summary>
        /// Отслеживаемый год.
        /// </summary>
        public int Year { get; }

        /// <summary>
        /// Годовая сумма транзакций.
        /// </summary>
        public double Amount { get; private set; }

        /// <summary>
        /// Начало списка транзакций по месяцам для данного года.
        /// </summary>
        public MonthNode TransactionsByMonthBegin { get; set; }

        /// <summary>
        /// Список транзакций для следующего года.
        /// </summary>
        public YearNode NextYear { get; set; }

        /// <summary>
        /// Возращает или создает узел для списка транзакций по месяцам.
        /// </summary>
        /// <param name="month">Отслеживаемый месяц.</param>
        /// <returns>Узел для указанного месяца.</returns>
        public MonthNode GetOrCreateMonthNode(int month)
        {
            MonthNode mNode;
            if (TransactionsByMonthBegin == null)
            {
                mNode = new MonthNode(month);
                TransactionsByMonthBegin = mNode;
            }
            else if (TransactionsByMonthBegin.Month < month)
            {
                mNode = new MonthNode(month)
                {
                    NextMonth = TransactionsByMonthBegin
                };
                TransactionsByMonthBegin = mNode;
            }
            else
            {
                mNode = TransactionsByMonthBegin;
                while (mNode.NextMonth != null && month <= mNode.NextMonth.Month)
                    mNode = mNode.NextMonth;
                if (mNode.Month != month)
                {
                    MonthNode yTemp = mNode.NextMonth;
                    mNode.NextMonth = new MonthNode(month);
                    mNode = mNode.NextMonth;
                    mNode.NextMonth = yTemp;
                }
            }
            return mNode;
        }
    }
}
