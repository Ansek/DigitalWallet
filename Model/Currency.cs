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
        /// <param name="code">Код согласно ISO 4217.</param>
        /// <param name="name">Название.</param>
        public Currency(string code, string name)
        {
            Code = code;
            Name = name;
        }

        /// <summary>
        /// Код согласно ISO 4217.
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
