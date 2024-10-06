namespace Cahuil.Bot.Model
{
    public class APIResultMessage<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Entity { get; set; }
    }
}
