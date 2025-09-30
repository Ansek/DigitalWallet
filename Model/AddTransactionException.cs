using System;

namespace DigitalWallet.Model
{
    /// <summary>
    /// Конкретизирует причину ошибкт при добавлении транзакции.
    /// </summary>
    public class AddTransactionException : Exception
    {
        /// <summary>
        /// Идентификатор типа исключения.
        /// </summary>
        public enum Type
        { 
            /// <summary>
            /// Найден такой же идентификатор.
            /// </summary>
            RepeatID, 
            /// <summary>
            /// Проблема суммирования до текущей даты.
            /// </summary>
            PrevDateSumErr,
            /// <summary>
            /// Проблема суммирования после текущей даты.
            /// </summary>
            NextDateSumErr
        };

        /// <summary>
        /// Конкретизирует причину ошибкт при добавлении транзакции.
        /// </summary>
        /// <param name="code">Код ошибки.</param>
        /// <param name="message">Сообщение об ошибке.</param>
        public AddTransactionException(Type code, string message) : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// Код ошибки.
        /// </summary>
        public Type Code { get; }
    }
}
