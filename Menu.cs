using System;
using System.Collections.Generic;
using System.Linq;
using DigitalWallet.Model;

namespace DigitalWallet
{
    /// <summary>
    /// Выводит пункты меню в консоль.
    /// </summary>
    internal class Menu
    {
        /// <summary>
        /// Набор данных программы.
        /// </summary>
        private readonly DataSet _data;

        /// <summary>
        /// Выбранный кошелёк.
        /// </summary>
        private Wallet _selectedWallet;

        private string _resultMessage;

        /// <summary>
        /// Выводит пункты меню в консоль.
        /// </summary>
        /// <param name="data">Набор данных программы.</param>
        public Menu(DataSet data)
        {
            _data = data;
            _selectedWallet = null;
            _resultMessage = null;
        }

        /// <summary>
        /// Печать главного меню.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                Console.Clear();
                if (_resultMessage != null)
                {
                    Console.WriteLine($"Статус: {_resultMessage}\n");
                    _resultMessage = null;
                }
                if (_selectedWallet != null)
                {
                    Console.WriteLine($"Кошелёк ID{_selectedWallet.ID} {_selectedWallet.Name}.");
                    Console.WriteLine($"Валюта: {_selectedWallet.Currency}.");
                    Console.WriteLine($"Начальный баланс: {_selectedWallet.OpeningBalance}.");
                    Console.WriteLine($"Текущий баланс: {_selectedWallet.Balance}.\n");
                    Console.WriteLine("Меню:");
                    Console.WriteLine("1. Добавить транзакцию.");
                    Console.WriteLine("2. Показать все транзакции.");
                    Console.WriteLine("3. Показать транзакции за месяц.");
                    Console.WriteLine("4. Отчет о наибольших тратах за месяц.");
                    Console.WriteLine("5. Вернуться в главное меню.");

                    TryUntilDataRead("Выбрать пункт > ", out int cmd);

                    switch (cmd)
                    {
                        case 1:
                            AddTransaction();
                            break;
                        case 2:
                            ShowAllTransactions();
                            break;
                        case 3:
                            ShowTransactionsByMonth();
                            break;
                        case 4:
                            //ShowBiggestExpensesInMonth();
                            break;
                        case 5:
                            _selectedWallet = null;
                            break;
                        default:
                            Console.WriteLine("Ошибка выбора.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Главное меню:");
                    Console.WriteLine("1. Создать кошелёк.");
                    Console.WriteLine("2. Выбрать кошелёк.");
                    Console.WriteLine("3. Отчет о наибольших тратах за месяц для всех кошельков.");
                    Console.WriteLine("4. Выход (сохранение данных)");

                    TryUntilDataRead("Выбрать пункт > ", out int cmd);

                    switch (cmd)
                    {
                        case 1:
                            CreateWallet();
                            break;
                        case 2:
                            SelectWallet();
                            break;
                        case 3:
                            //ShowBiggestExpensesInMonthForAllWallets();
                            break;
                        case 4:
                            _data.Save();
                            return;
                        default:
                            Console.WriteLine("Ошибка выбора.");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Заполенение данных и добавление транзакции в текущий кошелёк.
        /// </summary>
        private void AddTransaction()
        {
            Console.Clear();
            Console.WriteLine($"Добавление транзакции в кошелёк ID{_selectedWallet.ID} {_selectedWallet.Name}.");
            Console.WriteLine("Заполните данные (пустая строка - отмена операции):");
            if (TryUntilDataRead("Дата: ", out DateTime date) &&
                TryUntilDataRead("Сумма: ", out double amount) &&
                TryUntilTransactionTypeSelected(out TransactionType type) &&
                TryUntilDataRead("Описание: ", out string description))
            {
                try
                {
                    ulong id = _data.AddTransaction(_selectedWallet, date, amount, type, description);
                    _resultMessage = $"Транзакция с ID = {id} успешно создана.";
                }
                catch (ArgumentException ex)
                {
                    _resultMessage = "Ошибка при создании транзакции. " + ex.Message;
                }
                catch (AddTransactionException ex)
                {
                    _resultMessage = "Ошибка при добавлении транзакции в кошелёк. " + ex.Message;
                }
            }
            else
            {
                _resultMessage = "Отмена операции (добавление транзакции).";
            }
        }

        /// <summary>
        /// Выводит информацию о транзакции в консоль.
        /// </summary>
        /// <param name="transaction"></param>
        private void PrintTransaction(Transaction transaction)
        {
            Console.Write($"ID={transaction.ID, -7}");

            if (transaction.Type == TransactionType.Income)
                Console.Write("Доход  ");
            else if (transaction.Type == TransactionType.Expense)
                Console.Write("Расход ");
            else
                Console.Write("?      ");

            Console.WriteLine($"{transaction.Amount, 12} {_selectedWallet.Currency.Code} {transaction.Date, 12} | {transaction.Description}.");
        }

        /// <summary>
        /// Показать все транзакции.
        /// </summary>
        private void ShowAllTransactions()
        {
            var transactions = _selectedWallet.GetTransactionsSortedByDate();
            if (transactions.Count() > 0)
            {
                Console.Clear();
                Console.WriteLine($"Просмотр всех транзакции кошелька ID{_selectedWallet.ID} {_selectedWallet.Name}.");
                Console.WriteLine($"Начальный баланс: {_selectedWallet.OpeningBalance}.");
                Console.WriteLine($"Текущий баланс: {_selectedWallet.Balance}.\n");

                Console.WriteLine("Результат поиска:");
                foreach (var transaction in transactions)
                    PrintTransaction(transaction);

                Console.Write("\nПродолжить...");
                Console.ReadLine();
            }
            else
            {
                _resultMessage = "Транзакции не найдены. Требуется создание хотя бы одной транзакции.";
            }
        }

        /// <summary>
        /// Показать транзакции за месяц.
        /// </summary>
        private void ShowTransactionsByMonth()
        {
            var dates = new List<DateTime>(_selectedWallet.GetYearMonthOfTransactions());
            if (dates.Count > 0)
            {
                Console.Clear();
                Console.WriteLine($"Просмотр транзакции за месяц для кошелька ID{_selectedWallet.ID} {_selectedWallet.Name}.");

                if (!TryUntilMonthSelected(out DateTime date, dates))
                    return;

                Console.WriteLine("Результат поиска:");
                var transactions = _selectedWallet.GetTransactionsByFilter(date.Year, date.Month);
                foreach (var transaction in transactions)
                    PrintTransaction(transaction);

                Console.Write("\nПродолжить...");
                Console.ReadLine();
            }
            else
            {
                _resultMessage = "Транзакции не найдены. Требуется создание хотя бы одной транзакции.";
            }
        }

        /// <summary>
        /// Отчет о наибольших тратах за месяц для всех кошельков.
        /// </summary>
        private void ShowBiggestExpensesInMonth()
        {

        }

        /// <summary>
        /// Заполенение данных и создание кошелька.
        /// </summary>
        private void CreateWallet()
        {
            Console.Clear();
            Console.WriteLine("Создание кошелька.");
            Console.WriteLine("Заполните данные (пустая строка - отмена операции):");
            if (TryUntilDataRead("Название: ", out string name) &&
                TryUntilCurrencySelected(out Currency currency) &&
                TryUntilDataRead($"Начальный баланс ({currency.Code}): ", out double openingBalance))
            {
                try
                {
                    ulong id = _data.CreateWallet(name, currency, openingBalance);
                    _resultMessage = $"Кошелёк с ID = {id} успешно создан.";
                }
                catch (ArgumentException ex)
                {
                    _resultMessage = "Ошибка при создании кошелька. " + ex.Message;
                }
            }
            else
            {
                _resultMessage = "Отмена операции (создание кошелька).";
            }
        }

        /// <summary>
        /// Выбор кошелька.
        /// </summary>
        private void SelectWallet()
        {
            if (_data.Wallets.Count > 0)
            {
                Console.Clear();
                Console.WriteLine("Список кошельков.");
                foreach (var w in _data.Wallets)
                    Console.WriteLine($"ID={w.ID}. {w.Name}. Баланс: {w.Balance} {w.Currency.Code}.");

                while (_selectedWallet == null)
                {
                    if (!TryUntilDataRead("Выбрать ID > ", out ulong id))
                    {
                        _resultMessage = "Отмена операции (выбор кошелька).";
                        break;
                    }
                    _selectedWallet = _data.GetWalletById(id);
                    if (_selectedWallet == null)
                        Console.WriteLine("ID не найден. Повторите ввод.");
                }
            }
            else
            {
                _resultMessage = "Кошельки не найдены. Требуется создание хотя бы одного кошелька.";
            }
        }

        /// <summary>
        /// Отчет о наибольших тратах за месяц для всех кошельков.
        /// </summary>
        private void ShowBiggestExpensesInMonthForAllWallets()
        {

        }

        /// <summary>
        /// Пытается считать данные из консоли в цикле,
        /// пока не будут получены данные или не будет 
        /// введена пустая строка (отмена операции).
        /// </summary>
        /// <typeparam name="T">Тип значения для считывания.</typeparam>
        /// <param name="parse">Функция для преобразование из в строки в значение.</param>
        /// <param name="msg">Выводимое сообщение.</param>
        /// <param name="data">Данные для считывания.</param>
        /// <returns><see langword="false"/> - если пользователь отменил ввод пустой строкой.</returns>
        private bool TryUntilDataRead<T>(string msg, out T data, Func<string, T> parse)
        {
            string line;
            while (true)
            {
                Console.Write(msg);
                line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    data = default;
                    return false;
                }
                try
                {
                    data = parse(line);
                    return true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Некорректное значение. Повторите ввод.");
                }
            }
        }

        /// <summary>
        /// Пытается считать данные из консоли в цикле,dfsfsd
        /// пока не будут получены данные или не будет 
        /// введена пустая строка (отмена операции).
        /// </summary>
        /// <param name="msg">Выводимое сообщение.</param>
        /// <param name="data">Данные для считывания.</param>
        /// <returns><see langword="false"/> - если пользователь отменил ввод пустой строкой.</returns>
        private bool TryUntilDataRead(string msg, out int data) => TryUntilDataRead(msg, out data, int.Parse);

        /// <inheritdoc cref="TryUntilDataRead"/>
        private bool TryUntilDataRead(string msg, out ulong data) => TryUntilDataRead(msg, out data, ulong.Parse);

        /// <inheritdoc cref="TryUntilDataRead"/>
        private bool TryUntilDataRead(string msg, out double data) => TryUntilDataRead(msg, out data, double.Parse);

        /// <inheritdoc cref="TryUntilDataRead"/>
        private bool TryUntilDataRead(string msg, out DateTime data) => TryUntilDataRead(msg, out data, DateTime.Parse);

        /// <inheritdoc cref="TryUntilDataRead"/>
        private bool TryUntilDataRead(string msg, out string data) => TryUntilDataRead(msg, out data, s => s);

        /// <summary>
        /// Пытается установить значение валюты в в цикле,
        /// пока не будут введен верный номер или не будет 
        /// введена пустая строка (отмена операции).
        /// </summary>
        /// <param name="currency">Валюта.</param>
        /// <returns><see langword="false"/> - если пользователь отменил ввод пустой строкой.</returns>
        private bool TryUntilCurrencySelected(out Currency currency)
        {
            currency = default;
            while (true)
            {
                Console.WriteLine("Выберите номер валюты:");
                for (int i = 0; i < _data.Currencies.Count; i++)
                    Console.WriteLine($"{i + 1}. {_data.Currencies[i]}");

                if (!TryUntilDataRead("Выбрать пункт > ", out int choice))
                    return false;

                if (choice < 1 || _data.Currencies.Count < choice)
                {
                    Console.WriteLine("Выход за диапазон. Повторите ввод.");
                }
                else
                {
                    currency = _data.Currencies[choice - 1];
                    return true;
                }
            }
        }

        /// <summary>
        /// Пытается установить значение типа транзакции в в цикле,
        /// пока не будут введен верный номер или не будет 
        /// введена пустая строка (отмена операции).
        /// </summary>
        /// <param name="type">Тип транзакции.</param>
        /// <returns><see langword="false"/> - если пользователь отменил ввод пустой строкой.</returns>
        private bool TryUntilTransactionTypeSelected(out TransactionType type)
        {
            type = default;
            while (true)
            {
                Console.WriteLine("Выберите тип транзакции:");
                Console.WriteLine("1. Доход.");
                Console.WriteLine("2. Расход.");

                if (!TryUntilDataRead("Выбрать пункт > ", out int choice))
                    return false;

                if (choice == 1)
                {
                    type = TransactionType.Income;
                    return true;
                }
                if (choice == 2)
                {
                    type = TransactionType.Expense;
                    return true;
                }
                Console.WriteLine("Выход за диапазон. Повторите ввод.");
            }
        }

        /// <summary>
        /// Предоставляет пользователю выбор месяца (определенного года),
        /// пока не будут введен верный номер или не будет 
        /// введена пустая строка (отмена операции).
        /// </summary>
        /// <param name="date">Дата с выбранным месяцем.</param>
        /// <param name="dates">Список доступных дат.</param>
        /// <returns><see langword="false"/> - если пользователь отменил ввод пустой строкой.</returns>
        private bool TryUntilMonthSelected(out DateTime date, List<DateTime> dates)
        {
            Console.WriteLine("Возможные даты для просмотра.");
            for (int i = 0; i < dates.Count; i++)
                Console.WriteLine($"{i + 1}. {dates[i]:MMMM yyyy}");

            while (true)
            {
                if (!TryUntilDataRead("Выбрать пункт > ", out int choice))
                {
                    _resultMessage = "Отмена операции (просмотр транзакции за месяц).";
                    date = default;
                    return false;
                }
                if (choice < 1 || dates.Count < choice)
                {
                    Console.WriteLine("Выход за диапазон. Повторите ввод.");
                }
                else
                {
                    date = dates[choice - 1];
                    return true;
                }
            }
        }
    }
}
