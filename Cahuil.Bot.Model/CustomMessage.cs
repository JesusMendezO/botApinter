namespace Cahuil.Bot.Model
{
    public class CustomMessage
    {
        public string Body { get; set; }
        public TypeBodyBotCustomMessage TypeBody { get; set; }
        public string Name { get; set; }
        public int IssueId { get; set; }
        public string TicketDescription { get; set; }
        public TypeFieldBotCustomMessage TypeField { get; set; }
        public bool Enabled { get; set; }
    }

    public enum TypeFieldBotCustomMessage
    {
        Plan = 1, Fibra = 2, Welcome = 3, Goodbye = 4
    }

    public enum TypeBodyBotCustomMessage
    {
        Text = 1, Image = 2
    }
}
