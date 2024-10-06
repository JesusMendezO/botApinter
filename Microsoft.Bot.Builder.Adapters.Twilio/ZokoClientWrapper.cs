// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder.Adapters.ZokoWhatsapp;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Security;

namespace Microsoft.Bot.Builder.Adapters.TwilioWhatsapp
{
    /// <summary>
    /// Wrapper class for the Twilio API.
    /// </summary>
    public class ZokoClientWrapper
    {
        private const string TwilioSignature = "x-twilio-signature";
        private const string TwilioHeader = "x-forwarded-proto";

        /// <summary>
        /// Initializes a new instance of the <see cref="ZokoClientWrapper"/> class.
        /// </summary>
        /// <param name="options">An object containing API credentials, a webhook verification token and other options.</param>
        public ZokoClientWrapper(ZokoClientWrapperOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            
            TwilioClient.Init(Options.TwilioAccountSid, Options.TwilioAuthToken);
        }

        /// <summary>
        /// Gets the <see cref="ZokoClientWrapperOptions"/> for the wrapper. 
        /// </summary>
        /// <value>
        /// The <see cref="ZokoClientWrapperOptions"/> for the wrapper.
        /// </value>
        public ZokoClientWrapperOptions Options { get; }

        /// <summary>
        /// Sends a Twilio SMS message.
        /// </summary>
        /// <param name="messageOptions">An object containing the parameters for the message to send.</param>
        /// <param name="cancellationToken">A cancellation token for the task.</param>
        /// <returns>The SID of the Twilio message sent.</returns>
        
        public async Task<ZokoMessageResult> SendMessage(string apikey, string recipient, string message, string type)
        {
            try
            {
                ZokoMessageResult zokoMessageResult;
                string resultJson = "";
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("apikey", apikey);
                using (HttpResponseMessage result = await httpClient.PostAsync("https://chat.zoko.io/v2/message", new StringContent(JsonConvert.SerializeObject(new { recipient = recipient.Replace("+", ""), message = message, type = type, channel = "whatsapp" }), UTF8Encoding.UTF8, "application/json")))
                {
                    resultJson = await result.Content.ReadAsStringAsync();
                    zokoMessageResult = JsonConvert.DeserializeObject<ZokoMessageResult>(resultJson);
                }
                return zokoMessageResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ZokoMessageResult> SendMessage(string apikey, string recipient, string body, string caption, string type)
        {
            try
            {
                ZokoMessageResult zokoMessageResult;
                string resultJson = "";
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("apikey", apikey);
                using (HttpResponseMessage result = await httpClient.PostAsync("https://chat.zoko.io/v2/message",
                    new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            recipient = recipient.Replace("+", ""),
                            message = body,
                            type,
                            caption,
                            channel = "whatsapp"
                        }), UTF8Encoding.UTF8, "application/json")))
                {
                    resultJson = await result.Content.ReadAsStringAsync();
                    zokoMessageResult = JsonConvert.DeserializeObject<ZokoMessageResult>(resultJson);
                }
                return zokoMessageResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Validates an HTTP request as coming from Twilio.
        /// </summary>
        /// <param name="httpRequest">The request to validate.</param>
        /// <param name="body">The request payload, as key-value pairs.</param>
        /// <returns>The result of the comparison between the signature in the request and the hashed body.</returns>
        public virtual bool ValidateSignature(HttpRequest httpRequest, Dictionary<string, string> body)
        {
            var urlString = Options.TwilioValidationUrl?.ToString();
            
            var twilioSignature = httpRequest.Headers.ContainsKey(TwilioSignature)
                ? httpRequest.Headers[TwilioSignature].ToString()
                : throw new ArgumentNullException($"HttpRequest is missing \"{TwilioSignature}\"");

            if (string.IsNullOrWhiteSpace(urlString))
            {
                urlString = httpRequest.Headers[TwilioHeader][0];
                if (string.IsNullOrWhiteSpace(urlString))
                {
                    urlString = $"{httpRequest.Protocol}://{httpRequest.Host + httpRequest.Path}";
                }
            }

            var requestValidator = new RequestValidator(Options.TwilioAuthToken);

            return requestValidator.Validate(urlString, body, twilioSignature);
        }
    }
}
