using Cahuil.Bot.Data;
using Cahuil.Bot.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cahuil.Bot.Service
{
    #region APInterService class
    public class APInterService : IAPInterService
    {
        #region Fields
        public readonly IntegrationAPIClient _integrationAPIClient;
        #endregion

        #region Constructors
        public APInterService()
        {
            _integrationAPIClient = new IntegrationAPIClient();
        }
        #endregion

        public async Task<APIResultMessage<APInterCustomer>> GetCustomer(string customerId, APInterBotState apInterBotState)
        {
            return await _integrationAPIClient.GetAPInterCustomer(customerId, apInterBotState);
        }

        public async Task<APIResultMessage<int>> CreateTicket(APInterBotState apInterBotState)
        {
            apInterBotState.Channel = 2;
            return await _integrationAPIClient.CreateAPInterTicket(apInterBotState);
        }

        public async Task<int> BotSession(APInterBotState apInterBotState)
        {
            return await _integrationAPIClient.BotSession(apInterBotState);
        }

        public async Task<int> RegisterMessage(string url, string from, string to, string body, string sid, string latitude, string longitude, int cantMendia, string status, DateTime created)
        {
            return await _integrationAPIClient.RegisterMessage(url, from, to, body, sid, latitude, longitude, cantMendia, status, created);
        }

        public async Task<bool> IsToAgent(string url, string from, string to)
        {
            return await _integrationAPIClient.IsToAgent(url, from, to);
        }

        public async Task<int> HasTicket(string url, string customerId)
        {
            return await _integrationAPIClient.HasTicket(url, customerId);
        }

        public async Task<int> HasTicketFrom(string url, string from)
        {
            return await _integrationAPIClient.HasTicketFrom(url, from);
        }

        public async Task<Massive> IsMassive(string url)
        {
            return await _integrationAPIClient.IsEventMassive(url);
        }

        public async Task<ResponseMessage<CustomMessage>> GetCustomMessage(string url, string customerId)
        {
            return await _integrationAPIClient.GetCustomMessage(url, customerId);
        }

        public async Task<CustomMessage> GetTemporaryCustomMessage(string url, int fieldType)
        {
            return (await _integrationAPIClient.GetTemporaryCustomMessage(url, fieldType)).FirstOrDefault();
        }
    }
    #endregion
}
