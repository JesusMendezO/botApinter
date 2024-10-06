namespace Cahuil.Bot.Model
{
    #region ResponseMessage
    public class ResponseMessage<T>
    {
        /// <summary>
        /// Inicializa el objeto con Success en True
        /// </summary>
        public ResponseMessage(T entity)
        {
            Success = true;
            Message = string.Empty;
            Entity = entity;
        }

        public ResponseMessage()
        { }

        /// <summary>
        /// Inicializa el objecto con Success en False
        /// </summary>
        /// <param name="Mensaje del error ó validación fallida."></param>
        public ResponseMessage(string message)
        {
            Success = false;
            Message = message;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Entity { get; set; }
    }
    #endregion

    #region ResponseMessage
    public class ResponseMessage
    {
        /// <summary>
        /// Inicializa el objeto con Success en True
        /// </summary>
        public ResponseMessage()
        {
            Success = true;
            Message = string.Empty;
        }

        /// <summary>
        /// Inicializa el objecto con Success en False
        /// </summary>
        /// <param name="Mensaje del error ó validación fallida."></param>
        public ResponseMessage(string message)
        {
            Success = false;
            Message = message;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
    }
    #endregion
}
