using System;

namespace DigitalWallet.Model
{
    /// <summary>
    /// Валюта.
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Валюта.
        /// </summary>
        /// <param name="code">Буквенный код ISO 4217.</param>
        /// <param name="name">Название.</param>
        public Currency(string code, string name)
        {
            if (code.Length != 3 || !IsUpperLatin(code))
                throw new ArgumentException("Код валюты должен состоять из 3 заглавных латинских букв.");
            Code = code;
            Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsUpperLatin(string str)
        {
            foreach(char c in str)
                if (c < 'A' || 'Z' < c)
                    return false;
            return true;
        }

        /// <summary>
        /// Буквенный код ISO 4217.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Возвращает значения полей валюты.
        /// </summary>
        /// <returns>Строка с названием и кодом.</returns>
        public override string ToString()
        {
            return $"{Name} ({Code})";
        }
    }
}
