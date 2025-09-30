namespace DigitalWallet.Model
{
    /// <summary>
    /// Кошелек.
    /// </summary>
    public class Wallet
    {
        /// <summary>
        /// Кошелек.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="name">Название.</param>
        /// <param name="currency">Валюта.</param>
        /// <param name="openingBalance">Начальный баланс.</param>
        public Wallet(int id, string name, Currency currency, double openingBalance)
        {
            ID = id;
            Name = name;
            Currency = currency;
            OpeningBalance = openingBalance;
        }

        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int ID { get; }

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
    }
}