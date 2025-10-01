using System;
using System.Collections.Generic;
using DigitalWallet.Model;

namespace DigitalWallet
{
    /// <summary>
    /// Хранит данные программы.
    /// </summary>
    internal class DataSet
    {
        /// <summary>
        /// Максимальное значение идентификатора получаемого случайных образом.
        /// </summary>
        private const int MAX_RANDOM_ID = 1000;

        /// <summary>
        /// Список кошельков.
        /// </summary>
        private readonly List<Wallet> _wallets;

        /// <summary>
        /// Список кошельков.
        /// </summary>
        public IReadOnlyList<Wallet> Wallets => _wallets;

        /// <summary>
        /// Список валют.
        /// </summary>
        private readonly List<Currency> _currencies;

        /// <summary>
        /// Список валют.
        /// </summary>
        public IReadOnlyList<Currency> Currencies => _currencies;

        /// <summary>
        /// Значение последнего идентифекатора транзакции.
        /// </summary>
        public ulong _lastTransactionID = 0;

        /// <summary>
        /// Хранит данные программы.
        /// </summary>
        public DataSet()
        {
            _wallets = new List<Wallet>();
            _currencies = new List<Currency>()
            {
                new Currency("RUB", "Рубль"),
                new Currency("USD", "Доллар"),
                new Currency("EUR", "Евро")
            };
        }

        /// <summary>
        /// Создает и добавляет кошелёк в список.
        /// </summary>
        /// <param name="name">Название.</param>
        /// <param name="currency">Валюта.</param>
        /// <param name="openingBalance">Начальный баланс.</param>
        /// <returns>Идентификатор кошелька.</returns>
        public ulong CreateWallet(string name, Currency currency, double openingBalance)
        {
            Random random = new Random();
            ulong id = (ulong)random.Next(1, MAX_RANDOM_ID);
            for (int i = 0; i < _wallets.Count; i++)
                if (_wallets[i].ID == id)
                    id = (ulong)random.Next(1, MAX_RANDOM_ID);
            var wallet = new Wallet(id, name, currency, openingBalance);
            _wallets.Add(wallet);
            return id;
        }

        /// <summary>
        /// Получает кошелёк по Идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <returns>Объект кошелька или <see langword="null"/> если такой id не найден.</returns>
        public Wallet GetWalletById(ulong id)
        {
            foreach(var wallet in _wallets)
                if (wallet.ID == id)
                    return wallet;
            return null;
        }


        /// <summary>
        /// Создает и добавляет транзакцию в кошелёк.
        /// </summary>
        /// <param name="wallet">Кошелёк для добавления транзакции.</param>
        /// <param name="date">Дата и время выполнения транзакции.</param>
        /// <param name="amount">Сумма транзакции.</param>
        /// <param name="type">Тип транзакции.</param>
        /// <param name="description">Описание транзакции.</param>
        /// <returns>Идентификатор транзакции.</returns>
        public ulong AddTransaction(Wallet wallet, DateTime date, double amount, TransactionType type, string description)
        {
            _lastTransactionID++;
            var transaction = new Transaction(_lastTransactionID, date, amount, type, description);
            wallet.AddTransaction(transaction);
            return transaction.ID;
        }
    }
}
