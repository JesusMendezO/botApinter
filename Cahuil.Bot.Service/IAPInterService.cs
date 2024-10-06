using Cahuil.Bot.Model;
using System;
using System.Threading.Tasks;

namespace Cahuil.Bot.Service
{
    #region IAPInterService
    public interface IAPInterService
    {
        Task<int> BotSession(APInterBotState apInterBotState);
        Task<APIResultMessage<int>> CreateTicket(APInterBotState apInterBotState);
        Task<APIResultMessage<APInterCustomer>> GetCustomer(string customerId, APInterBotState apInterBotState);
        Task<ResponseMessage<CustomMessage>> GetCustomMessage(string url, string customerId);
        Task<CustomMessage> GetTemporaryCustomMessage(string url, int fieldType);
        Task<int> HasTicket(string url, string customerId);
        Task<int> HasTicketFrom(string url, string from);
        Task<Massive> IsMassive(string url);
        Task<bool> IsToAgent(string url, string from, string to);
        Task<int> RegisterMessage(string url, string from, string to, string body, string sid, string latitude, string longitude, int cantMedia, string status,DateTime created);
    }
    #endregion
}