using DigitalWallet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DigitalWallet
{
    /// <summary>
    /// Выполняет взаимодействие с json-файлом для сохранения информации.
    /// </summary>
    internal class JSONManager
    {
        /// <summary>
        /// Выполняет взаимодействие с json-файлом для сохранения информации.
        /// </summary>
        /// <param name="fileName">Название файла.</param>
        public JSONManager(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Название файла.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Выполняет сохранение данных в файл.
        /// </summary>
        /// <param name="wallets">Список кошельков.</param>
        public void Save(IReadOnlyList<Wallet> wallets)
        {
            var root = new JsonObject();
            var walletsList = new JsonArray();
            root.Add("wallets", walletsList);
            foreach (var wallet in wallets)
            {
                var transactions = new JsonArray();
                foreach (var transaction in wallet.GetTransactionsSortedByDate())
                {
                    var jsonTransaction = new JsonObject
                    {
                        { "id", transaction.ID },
                        { "date", transaction.Date },
                        { "amount", transaction.Amount },
                        { "isIncome", transaction.Type == TransactionType.Income },
                        { "description", transaction.Description }
                    };
                    transactions.Add(jsonTransaction);
                }
                var jsonWallet = new JsonObject
                {
                    { "id", wallet.ID },
                    { "name", wallet.Name },
                    { "currency", wallet.Currency.Code },
                    { "openingBalance", wallet.OpeningBalance },
                    { "transactions", transactions }
                };
                walletsList.Add(jsonWallet);

                File.WriteAllText(FileName, root.ToJsonString(new JsonSerializerOptions() { WriteIndented = true }));
            }
        }

        /// <summary>
        /// Выполняет загрузку из файла.
        /// </summary>
        /// <param name="currencies">Список доступных валют.</param>
        /// <returns>Список кошельков.</returns>
        public List<Wallet> Load(IReadOnlyList<Currency> currencies)
        {
            List<Wallet> wallets = new List<Wallet>();
            if (File.Exists(FileName))
            {
                string contents = File.ReadAllText(FileName);
                JsonNode root = JsonNode.Parse(contents);
                foreach (var jsonWallet in root["wallets"].AsArray())
                {
                    ulong id = jsonWallet["id"].GetValue<ulong>();
                    string name = jsonWallet["name"].GetValue<string>();
                    string code = jsonWallet["currency"].GetValue<string>();
                    double openingBalance = jsonWallet["openingBalance"].GetValue<double>();
                    JsonArray transactions = jsonWallet["transactions"].AsArray();

                    Currency currency = null;
                    foreach (var curr in currencies)
                        if (curr.Code == code)
                            currency = curr;
                    if (currency == null)
                        currency = new Currency(code, "Error!");

                    var wallet = new Wallet(id, name, currency, openingBalance);
                    foreach (var jsonTransaction in transactions.Reverse())
                    {
                        id = jsonTransaction["id"].GetValue<ulong>();
                        DateTime date = jsonTransaction["date"].GetValue<DateTime>();
                        double amount = jsonTransaction["amount"].GetValue<double>();
                        bool isIncome = jsonTransaction["isIncome"].GetValue<bool>();
                        string description = jsonTransaction["description"].GetValue<string>();

                        var type = isIncome ? TransactionType.Income : TransactionType.Expense;

                        var transaction = new Transaction(id, date, amount, type, description);
                        wallet.AddTransaction(transaction);
                    }
                    wallets.Add(wallet);
                }
            }
            return wallets;
        }
    }
}
