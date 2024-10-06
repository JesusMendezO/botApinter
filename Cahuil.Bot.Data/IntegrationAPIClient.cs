using Cahuil.Bot.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cahuil.Bot.Data
{
	#region IntegrationAPIClient class
	public class IntegrationAPIClient
	{
		#region Const
		private const int PROJECT_ID = 1;
		#endregion

		#region Constructors
		public IntegrationAPIClient()
		{
		}
		#endregion

		#region Public methods
		public async Task<APIResultMessage<APInterCustomer>> GetAPInterCustomer(string customerId, APInterBotState apInterBotState)
		{
			APIResultMessage<APInterCustomer> resultMessage = null;
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			using (HttpResponseMessage result = await httpClient.PostAsync(apInterBotState.APIUrl + "Integration/apinter/customer", new StringContent(JsonConvert.SerializeObject(new { customerId = customerId }), UTF8Encoding.UTF8, "application/json")))
			{
				string resultJson = await result.Content.ReadAsStringAsync();
				resultMessage = JsonConvert.DeserializeObject<APIResultMessage<APInterCustomer>>(resultJson);
			}
			return resultMessage;
		}

		public async Task<APIResultMessage<int>> CreateAPInterTicket(APInterBotState apInterBotState)
		{
			APIResultMessage<int> resultMessage = null;
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			using (HttpResponseMessage result = await httpClient.PostAsync(apInterBotState.APIUrl + "ticket/bot",
				new StringContent(JsonConvert.SerializeObject(new
				{
					customerId = apInterBotState.CustomerId,
					customerName = apInterBotState.Fullname,
					conversationId = apInterBotState.ConversarionId,
					channel = apInterBotState.Channel,
					company = 1,
					from = apInterBotState.From,
					title = apInterBotState.Title,
					description = apInterBotState.Description,
					closed = apInterBotState.Closed,
					departament = apInterBotState.Departament,
					contactNumber = apInterBotState.ContactNumber,
					issueId = apInterBotState.IssueId,
					email = apInterBotState.Email,
					dueDebt = apInterBotState.DueDebt
				}), Encoding.UTF8, "application/json")))
			{
				string resultJson = await result.Content.ReadAsStringAsync();
				resultMessage = JsonConvert.DeserializeObject<APIResultMessage<int>>(resultJson);
			}
			return resultMessage;
		}

		public async Task<int> BotSession(APInterBotState apInterBotState)
		{
			int resultMessage;
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			using (HttpResponseMessage result = await httpClient.PostAsync(apInterBotState.APIUrl + "webhook/botsession",
				new StringContent(JsonConvert.SerializeObject(new
				{
					conversationId = apInterBotState.ConversarionId,
					projectId = 1,
					from = apInterBotState.From,
					to = apInterBotState.To,
					messageSid = apInterBotState.MessageSIDStartSession
				}), Encoding.UTF8, "application/json")))
			{
				string resultJson = await result.Content.ReadAsStringAsync();
				resultMessage = JsonConvert.DeserializeObject<int>(resultJson);
			}
			return resultMessage;
		}

		public async Task<int> RegisterMessage(string url, string from, string to, string body, string sid, string latitude, string longitude, int cantMedia, string status, DateTime created)
		{
			int resultMessage;
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			using (HttpResponseMessage result = await httpClient.PostAsync(url + "webhook/registermessage",
				new StringContent(JsonConvert.SerializeObject(new
				{
					from = from,
					to = to,
					body = body,
					created = created,
					sid = sid,
					latitude = latitude,
					longitude = longitude,
					cantMedia = cantMedia,
					status = status
				}), Encoding.UTF8, "application/json")))
			{
				string resultJson = await result.Content.ReadAsStringAsync();
				resultMessage = JsonConvert.DeserializeObject<int>(resultJson);
			}
			return resultMessage;
		}

		public async Task<bool> IsToAgent(string url, string from, string to)
		{
			bool resultMessage;
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			using (HttpResponseMessage result = await httpClient.PostAsync(url + "webhook/istoagent",
				new StringContent(JsonConvert.SerializeObject(new
				{
					from = from,
					to = to
				}), Encoding.UTF8, "application/json")))
			{
				string resultJson = await result.Content.ReadAsStringAsync();
				resultMessage = JsonConvert.DeserializeObject<bool>(resultJson);
			}
			return resultMessage;
		}

		public async Task<int> HasTicket(string url, string customerId)
		{
			int resultMessage;
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			using (HttpResponseMessage result = await httpClient.PostAsync(url + "ticket/bot/hasticket",
				new StringContent(JsonConvert.SerializeObject(new
				{
					customerId = customerId,
					projectId = PROJECT_ID
				}), Encoding.UTF8, "application/json")))
			{
				string resultJson = await result.Content.ReadAsStringAsync();
				resultMessage = JsonConvert.DeserializeObject<int>(resultJson);
			}
			return resultMessage;
		}

		public async Task<int> HasTicketFrom(string url, string from)
		{
			int resultMessage;
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			using (HttpResponseMessage result = await httpClient.PostAsync(url + "ticket/bot/hasticket/from",
				new StringContent(JsonConvert.SerializeObject(new
				{
					from = from,
					projectId = PROJECT_ID
				}), Encoding.UTF8, "application/json")))
			{
				string resultJson = await result.Content.ReadAsStringAsync();
				resultMessage = JsonConvert.DeserializeObject<int>(resultJson);
			}
			return resultMessage;
		}

		public async Task<Massive> IsEventMassive(string url)
		{
			Massive resultMessage;
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			using (HttpResponseMessage result = await httpClient.PostAsync(url + string.Format("configuration/massive/{0}/bot", PROJECT_ID),
				new StringContent(JsonConvert.SerializeObject(new { }), Encoding.UTF8, "application/json")))
			{
				string resultJson = await result.Content.ReadAsStringAsync();
				resultMessage = JsonConvert.DeserializeObject<Massive>(resultJson);
			}
			return resultMessage;
		}

		public async Task<ResponseMessage<CustomMessage>> GetCustomMessage(string url, string customerId)
		{
			ResponseMessage<CustomMessage> customMessageResult;
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			using (HttpResponseMessage result = await httpClient.GetAsync($"{url}bot/apinter/custom-message?customerId={customerId}"))
			{
				string resultJson = await result.Content.ReadAsStringAsync();
				customMessageResult = JsonConvert.DeserializeObject<ResponseMessage<CustomMessage>>(resultJson);
			}
			return customMessageResult;
		}

		public async Task<IEnumerable<CustomMessage>> GetTemporaryCustomMessage(string url, int fieldType)
		{
			IEnumerable<CustomMessage> customMessageResult;
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			using (HttpResponseMessage result = await httpClient.GetAsync($"{url}bot/custom-message/{PROJECT_ID}?customMessageTypes={fieldType}"))
			{
				string resultJson = await result.Content.ReadAsStringAsync();
				customMessageResult = JsonConvert.DeserializeObject<IEnumerable<CustomMessage>>(resultJson);
			}
			return customMessageResult;
		}
		#endregion
	}
	#endregion
}


