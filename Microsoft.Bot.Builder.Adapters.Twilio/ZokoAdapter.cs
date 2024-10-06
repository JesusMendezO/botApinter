// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder.Adapters.ZokoWhatsapp;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Microsoft.Bot.Builder.Adapters.TwilioWhatsapp
{
    /// <summary>
    /// A <see cref="BotAdapter"/> that can connect to Twilio's SMS service.
    /// </summary>
    public class ZokoAdapter : BotAdapter, IBotFrameworkHttpAdapter
    {
        private readonly ZokoClientWrapper _zokoClient;
        private readonly ILogger _logger;
        private readonly ZokoAdapterOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZokoAdapter"/> class using configuration settings.
        /// </summary>
        /// <param name="configuration">An <see cref="IConfiguration"/> instance.</param>
        /// <remarks>
        /// The configuration keys are:
        /// TwilioNumber: The phone number associated with the Twilio account.
        /// TwilioAccountSid: The string identifier of the account. See https://www.twilio.com/docs/glossary/what-is-a-sid
        /// TwilioAuthToken: The authentication token for the account.
        /// TwilioValidationUrl: The validation URL for incoming requests.
        /// </remarks>
        /// <param name="adapterOptions">Options for the <see cref="ZokoAdapter"/>.</param>
        /// <param name="logger">The ILogger implementation this adapter should use.</param>
        public ZokoAdapter(IConfiguration configuration, ZokoAdapterOptions adapterOptions = null, ILogger logger = null)
            : this(
                new ZokoClientWrapper(new ZokoClientWrapperOptions(configuration["ZokoNumber"], configuration["TwilioAccountSid"], configuration["TwilioAuthToken"], configuration["ZokoRegisterMessageURL"], new Uri(configuration["TwilioValidationUrl"]))), adapterOptions, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZokoAdapter"/> class.
        /// </summary>
        /// <param name="twilioClient">The Twilio client to connect to.</param>
        /// <param name="adapterOptions">Options for the <see cref="ZokoAdapter"/>.</param>
        /// <param name="logger">The ILogger implementation this adapter should use.</param>
        public ZokoAdapter(ZokoClientWrapper twilioClient, ZokoAdapterOptions adapterOptions, ILogger logger = null)
        {
            _zokoClient = twilioClient ?? throw new ArgumentNullException(nameof(twilioClient));
            _logger = logger ?? NullLogger.Instance;
            _options = adapterOptions ?? new ZokoAdapterOptions();
        }

        /// <summary>
        /// Sends activities to the conversation.
        /// </summary>
        /// <param name="turnContext">The context object for the turn.</param>
        /// <param name="activities">The activities to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>If the activities are successfully sent, the task result contains
        /// an array of <see cref="ResourceResponse"/> objects containing the SIDs that
        /// Twilio assigned to the activities.</remarks>
        /// <seealso cref="ITurnContext.OnSendActivities(SendActivitiesHandler)"/>
        public override async Task<ResourceResponse[]> SendActivitiesAsync(ITurnContext turnContext, Activity[] activities, CancellationToken cancellationToken)
        {
            var responses = new List<ResourceResponse>();
            foreach (var activity in activities)
            {
                if (activity.Type != ActivityTypes.Message)
                {
                    _logger.LogTrace(
                        $"Unsupported Activity Type: '{activity.Type}'. Only Activities of type 'Message' are supported.");
                }
                else
                {
                    ZokoMessageOptions messageOptions = ZokoHelper.ActivityToZoko(activity, _zokoClient.Options.ZokoNumber);
                    //var messageOptions = ZokoHelper.ActivityToZoko(activity, _zokoClient.Options.ZokoNumber);
                    ZokoMessageResult messageResult = null;
                    if (activity.Attachments == null || activity.Attachments.Count == 0)
                        messageResult = await _zokoClient.SendMessage("7361e105-b643-48d9-84ac-f69992f53173", messageOptions.To, messageOptions.Body, "", "text");
                    else
                        messageResult = await _zokoClient.SendMessage("7361e105-b643-48d9-84ac-f69992f53173", messageOptions.To, messageOptions.MediaUrl, messageOptions.Body, "image");
                    var response = new ResourceResponse()
                    {
                        Id = messageResult.MessageId,
                    };

                    //ZokoMessageResult messageResult = await _zokoClient.SendMessage("e80dce4e-0387-41b1-8af2-ebc6d2d437cf", messageOptions.To, messageOptions.Body, "text");
                    //var response = new ResourceResponse()
                    //{
                    //    Id = messageResult.MessageId,
                    //};
                    //await ZokoHelper.RegisterMessage(_zokoClient.Options.ZokoRegisterMedssageUrl, "+5492346569251", messageOptions.To, messageOptions.Body, response.Id, "", "", 0, messageResult.StatusText, DateTime.Now, "Mobi","");                   
                    //responses.Add(response);
                    if (activity.Attachments == null || activity.Attachments.Count == 0)
                        await ZokoHelper.RegisterMessage(_zokoClient.Options.ZokoRegisterMedssageUrl, "+5492342481818", messageOptions.To, messageOptions.Body, response.Id, "", "", 0, messageResult.StatusText, DateTime.Now, "ApInter", "");
                    else
                        await ZokoHelper.RegisterMessage(_zokoClient.Options.ZokoRegisterMedssageUrl, "+5492342481818", messageOptions.To, messageOptions.Body, response.Id, "", "", 1, messageResult.StatusText, DateTime.Now, "ApInter", $"image|{messageOptions.MediaUrl}");
                    responses.Add(response);
                }
            }

            return responses.ToArray();
        }

        /// <summary>
        /// Creates a turn context and runs the middleware pipeline for an incoming activity.
        /// </summary>
        /// <param name="httpRequest">The incoming HTTP request.</param>
        /// <param name="httpResponse">When this method completes, the HTTP response to send.</param>
        /// <param name="bot">The bot that will handle the incoming activity.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="httpRequest"/>,
        /// <paramref name="httpResponse"/>, or <paramref name="bot"/> is <c>null</c>.</exception>
        public async Task ProcessAsync(HttpRequest httpRequest, HttpResponse httpResponse, IBot bot, CancellationToken cancellationToken)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            if (httpResponse == null)
            {
                throw new ArgumentNullException(nameof(httpResponse));
            }

            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }
            string body = string.Empty;
            using (StreamReader stream = new StreamReader(httpRequest.Body))
            {
                body = await stream.ReadToEndAsync();
            }
            // TODO: Validación del Token (Token URL AppSettings)
            //if (_options.ValidateIncomingRequests && !_twilioClient.ValidateSignature(httpRequest, bodyDictionary))
            //{
            //    throw new AuthenticationException("WARNING: Webhook received message with invalid signature. Potential malicious behavior!");
            //}
            ZokoMessage zokoMessage = JsonConvert.DeserializeObject<ZokoMessage>(body);
            zokoMessage.PlatformRecieverId = _zokoClient.Options.ZokoNumber;
            var activity = ZokoHelper.PayloadToActivity(zokoMessage);
            switch (zokoMessage.Type)
            {
                case "location":
                    string[] location = zokoMessage.Text.Split(':');
                    await ZokoHelper.RegisterMessage(_zokoClient.Options.ZokoRegisterMedssageUrl, zokoMessage.PlatformSenderId, zokoMessage.PlatformRecieverId, zokoMessage.Text, zokoMessage.Id, location[0], location[1], 0, zokoMessage.DeliveryStatus, zokoMessage.PlatformTimestamp.ToLocalTime(), zokoMessage.SenderName, "location");
                    break;
                case "audio":
                case "document":
                case "sticker":
                case "image":
                    await ZokoHelper.RegisterMessage(_zokoClient.Options.ZokoRegisterMedssageUrl, zokoMessage.PlatformSenderId, zokoMessage.PlatformRecieverId, zokoMessage.FileCaption, zokoMessage.Id, "", "", 1, zokoMessage.DeliveryStatus, zokoMessage.PlatformTimestamp.ToLocalTime(), zokoMessage.SenderName, zokoMessage.Type + "|" + zokoMessage.FileUrl);
                    break;
                default:
                    await ZokoHelper.RegisterMessage(_zokoClient.Options.ZokoRegisterMedssageUrl, zokoMessage.PlatformSenderId, zokoMessage.PlatformRecieverId, zokoMessage.Text, zokoMessage.Id, "", "", 0, zokoMessage.DeliveryStatus, zokoMessage.PlatformTimestamp.ToLocalTime(), zokoMessage.SenderName, "");
                    break;
            }
            // create a conversation reference
            using (var context = new TurnContext(this, activity))
            {
                context.TurnState.Add("httpStatus", HttpStatusCode.OK.ToString("D"));
                await RunPipelineAsync(context, bot.OnTurnAsync, cancellationToken).ConfigureAwait(false);
                var statusCode = Convert.ToInt32(context.TurnState.Get<string>("httpStatus"), CultureInfo.InvariantCulture);
                var text = context.TurnState.Get<object>("httpBody") != null
                    ? context.TurnState.Get<object>("httpBody").ToString()
                    : string.Empty;

                await ZokoHelper.WriteAsync(httpResponse, statusCode, text, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Replaces an existing activity in the conversation.
        /// Twilio SMS does not support this operation.
        /// </summary>
        /// <param name="turnContext">The context object for the turn.</param>
        /// <param name="activity">New replacement activity.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>This method always returns a faulted task.</remarks>
        /// <seealso cref="ITurnContext.OnUpdateActivity(UpdateActivityHandler)"/>
        public override Task<ResourceResponse> UpdateActivityAsync(ITurnContext turnContext, Activity activity, CancellationToken cancellationToken)
        {
            return Task.FromException<ResourceResponse>(
                new NotSupportedException("Twilio SMS does not support updating activities."));
        }

        /// <summary>
        /// Deletes an existing activity in the conversation.
        /// Twilio SMS does not support this operation.
        /// </summary>
        /// <param name="turnContext">The context object for the turn.</param>
        /// <param name="reference">Conversation reference for the activity to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>This method always returns a faulted task.</remarks>
        /// <seealso cref="ITurnContext.OnDeleteActivity(DeleteActivityHandler)"/>
        public override Task DeleteActivityAsync(ITurnContext turnContext, ConversationReference reference, CancellationToken cancellationToken)
        {
            return Task.FromException<ResourceResponse>(
                new NotSupportedException("Twilio SMS does not support deleting activities."));
        }

        /// <summary>
        /// Sends a proactive message to a conversation.
        /// </summary>
        /// <param name="reference">A reference to the conversation to continue.</param>
        /// <param name="logic">The method to call for the resulting bot turn.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>Call this method to proactively send a message to a conversation.
        /// Most channels require a user to initiate a conversation with a bot
        /// before the bot can send activities to the user.</remarks>
        /// <seealso cref="BotAdapter.RunPipelineAsync(ITurnContext, BotCallbackHandler, CancellationToken)"/>
        /// <exception cref="ArgumentNullException"><paramref name="reference"/> or
        /// <paramref name="logic"/> is <c>null</c>.</exception>
        public async Task ContinueConversationAsync(ConversationReference reference, BotCallbackHandler logic, CancellationToken cancellationToken)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            if (logic == null)
            {
                throw new ArgumentNullException(nameof(logic));
            }

            var request = reference.GetContinuationActivity().ApplyConversationReference(reference, true);

            using (var context = new TurnContext(this, request))
            {
                await RunPipelineAsync(context, logic, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sends a proactive message from the bot to a conversation.
        /// </summary>
        /// <param name="claimsIdentity">A <see cref="ClaimsIdentity"/> for the conversation.</param>
        /// <param name="reference">A reference to the conversation to continue.</param>
        /// <param name="callback">The method to call for the resulting bot turn.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>Call this method to proactively send a message to a conversation.
        /// Most _channels require a user to initialize a conversation with a bot
        /// before the bot can send activities to the user.
        /// <para>This method registers the following services for the turn.<list type="bullet">
        /// <item><description><see cref="IIdentity"/> (key = "BotIdentity"), a claims claimsIdentity for the bot.
        /// </description></item>
        /// </list></para>
        /// </remarks>
        /// <seealso cref="BotAdapter.RunPipelineAsync(ITurnContext, BotCallbackHandler, CancellationToken)"/>
        public override async Task ContinueConversationAsync(ClaimsIdentity claimsIdentity, ConversationReference reference, BotCallbackHandler callback, CancellationToken cancellationToken)
        {
            using (var context = new TurnContext(this, reference.GetContinuationActivity()))
            {
                context.TurnState.Add<IIdentity>(BotIdentityKey, claimsIdentity);
                context.TurnState.Add<BotCallbackHandler>(callback);
                await RunPipelineAsync(context, callback, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
