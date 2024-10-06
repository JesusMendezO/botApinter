// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

#if SIGNASSEMBLY
[assembly: InternalsVisibleTo("Microsoft.Bot.Builder.Adapters.Twilio.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9")]
#else
[assembly: InternalsVisibleTo("Microsoft.Bot.Builder.Adapters.Twilio.Tests")]
#endif

namespace Microsoft.Bot.Builder.Adapters.TwilioWhatsapp
{
    /// <summary>
    /// A helper class to create Activities and Twilio messages.
    /// </summary>
    internal static class ZokoHelper
    {
        /// <summary>
        /// Creates Twilio SMS message options object from a Bot Framework <see cref="Activity"/>.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="zokoNumber">The Twilio phone number assigned to the bot.</param>
        /// <returns>The Twilio message options object.</returns>
        /// <seealso cref="ZokoAdapter.SendActivitiesAsync(ITurnContext, Activity[], System.Threading.CancellationToken)"/>
        public static ZokoMessageOptions ActivityToZoko(Activity activity, string zokoNumber)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }
            if (string.IsNullOrWhiteSpace(zokoNumber))
            {
                throw new ArgumentNullException(nameof(zokoNumber));
            }
            string mediaUrl = string.Empty;
            if (activity.Attachments != null && activity.Attachments.Count > 0)
            {
                mediaUrl = activity.Attachments.First().ContentUrl;
            }
            var messageOptions = new ZokoMessageOptions()
            {
                To = activity.Conversation.Id,
                ApplicationSid = activity.Conversation.Id,
                From = zokoNumber,
                Body = activity.Text
            };
            messageOptions.MediaUrl = mediaUrl;
            return messageOptions;
        }

        /// <summary>
        /// Writes the HttpResponse.
        /// </summary>
        /// <param name="response">The httpResponse.</param>
        /// <param name="code">The status code to be written.</param>
        /// <param name="text">The text to be written.</param>
        /// <param name="encoding">The encoding for the text.</param>
        /// <param name="cancellationToken">A cancellation token for the task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task WriteAsync(HttpResponse response, int code, string text, Encoding encoding, CancellationToken cancellationToken)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            response.ContentType = "text/plain";
            response.StatusCode = code;

            var data = encoding.GetBytes(text);

            await response.Body.WriteAsync(data, 0, data.Length, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a Bot Framework <see cref="Activity"/> from an HTTP request that contains a Twilio message.
        /// </summary>
        /// <param name="payload">The HTTP request.</param>
        /// <returns>The activity object.</returns>
        public static Activity PayloadToActivity(ZokoMessage zokoMessage)
        {
            if (zokoMessage == null)
            {
                throw new ArgumentNullException(nameof(zokoMessage));
            }
            return new Activity()
            {
                Id = zokoMessage.Id,
                Timestamp = DateTime.UtcNow,
                ChannelId = Channels.Directline,
                Conversation = new ConversationAccount()
                {
                    Id = zokoMessage.PlatformSenderId
                },
                From = new ChannelAccount()
                {
                    Id = zokoMessage.PlatformSenderId
                },
                Recipient = new ChannelAccount()
                {
                    Id = zokoMessage.PlatformRecieverId,
                },
                Text = string.IsNullOrEmpty(zokoMessage.Text) ? "-" : zokoMessage.Text,
                ChannelData = zokoMessage,
                Type = ActivityTypes.Message,
                Attachments = null,
            };
        }

        /// <summary>
        /// Gets attachments from a Twilio message.
        /// </summary>
        /// <param name="numMedia">The number of media items to pull from the message body.</param>
        /// <param name="message">A dictionary containing the Twilio message elements.</param>
        /// <returns>An Attachments array with the converted attachments.</returns>
        public static List<Attachment> GetMessageAttachments(int numMedia, Dictionary<string, string> message)
        {
            var attachments = new List<Attachment>();
            for (var i = 0; i < numMedia; i++)
            {
                // Ensure MediaContentType and MediaUrl are present before adding the attachment
                if (message.ContainsKey($"MediaContentType{i}") && message.ContainsKey($"MediaUrl{i}"))
                {
                    var attachment = new Attachment()
                    {
                        ContentType = message[$"MediaContentType{i}"],
                        ContentUrl = message[$"MediaUrl{i}"],
                    };
                    attachments.Add(attachment);
                }
            }

            return attachments;
        }

        /// <summary>
        /// Converts a query string to a dictionary with key-value pairs.
        /// </summary>
        /// <param name="query">The query string to convert.</param>
        /// <returns>A dictionary with the query values.</returns>
        public static Dictionary<string, string> QueryStringToDictionary(string query)
        {
            var values = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(query))
            {
                return values;
            }

            var pairs = query.Replace("+", "%20").Split('&');

            foreach (var p in pairs)
            {
                var pair = p.Split('=');
                var key = pair[0];
                var value = Uri.UnescapeDataString(pair[1]);

                values.Add(key, value);
            }

            return values;
        }

        public async static Task RegisterMessage(string url, string from, string to, string body, string sid, string latitude, string longitude, int cantMedia, string status, DateTime created, string profileName, string data)
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
                    status = status,
                    profileName = profileName,
                    provider = 2,
                    data = data
                }), Encoding.UTF8, "application/json")))
            {
                string resultJson = await result.Content.ReadAsStringAsync();
                resultMessage = JsonConvert.DeserializeObject<int>(resultJson);
            }
        }
    }
}
