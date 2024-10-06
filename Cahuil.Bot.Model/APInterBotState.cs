namespace Cahuil.Bot.Model
{
    #region APInterCustomer
    public class APInterBotState
    {
        public bool IsCustomer { get; set; }
        public string ConversarionId { get; set; }
        public int Channel { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Title { get; set; }
        public string CustomerId { get; set; }
        public string Fullname { get; set; }
        public string NameStatus { get; set; }
        public int Status { get; set; }
        public string Email { get; set; }
        public string DNI { get; set; }
        public string Description { get; set; }
        public bool Closed { get; set; }
        public int Departament { get; set; }
        public string DueDebt { get; set; }
        public string Debt { get; set; }
        public string APIUrl { get; set; }
        public int IssueId { get; set; }
        public bool BackMenu { get; set; }
        public string ContactNumber { get; set; }
        public bool MessageAttentionTicket { get; set; }
        public string MessageSIDStartSession { get; set; }
        public int HasTicket { get; set; }
        public Massive Massive { get; set; }
        public CustomMessage CustomMessage { get; set; }
    }
    #endregion
}
