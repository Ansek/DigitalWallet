using System;
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
                    Console.WriteLine("2. Показать транзакции за месяц.");
                    Console.WriteLine("3. Отчет о наибольших тратах за месяц.");
                    Console.WriteLine("4. Вернуться в главное меню.");

                    TryUntilDataRead("Выбрать пункт > ", out int cmd);

                    switch (cmd)
                    {
                        case 1:
                            
                            break;
                        case 2:
                            
                            break;
                        case 3:

                            break;
                        case 4:
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
                    Console.WriteLine("4. Выход");

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

                            break;
                        case 4:
                            return;
                        default:
                            Console.WriteLine("Ошибка выбора.");
                            break;
                    }
                }
            }
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
                TryUntilSelectCurrency(out Currency currency) &&
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
        /// <param name="currency"></param>
        /// <returns><see langword="false"/> - если пользователь отменил ввод пустой строкой.</returns>
        private bool TryUntilSelectCurrency(out Currency currency)
        {
            currency = default;
            while (true)
            {
                Console.WriteLine("Выберите номер валюты: ");
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
    }
}
