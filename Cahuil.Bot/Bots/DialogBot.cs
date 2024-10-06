using Cahuil.Bot.Model;
using Cahuil.Bot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters.TwilioWhatsapp;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Bots
{
    public class DialogBot<T> : ActivityHandler
            where T : Dialog
    {
        private readonly IAPInterService _apInterService;
        protected readonly IConfiguration _configuration;
        protected readonly Dialog _dialog;
        protected readonly BotState _conversationState;
        protected readonly BotState _userState;
        protected readonly ILogger _logger;
        protected readonly IStatePropertyAccessor<DateTime> LastAccessedTimeProperty;
        protected readonly IStatePropertyAccessor<DialogState> DialogStateProperty;
        protected const int EXPIRATION_MINUTES = 15;

        public DialogBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger, IConfiguration configuration)
        {
            _conversationState = conversationState;
            _userState = userState;
            _dialog = dialog;
            _logger = logger;
            _configuration = configuration;
            _apInterService = new APInterService();
            DialogStateProperty = userState.CreateProperty<DialogState>(nameof(DialogState));
            LastAccessedTimeProperty = userState.CreateProperty<DateTime>(nameof(LastAccessedTimeProperty));
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            //TWILIO
            //TwilioMessage message = turnContext.Activity.ChannelData as TwilioMessage;
            ZokoMessage message = turnContext.Activity.ChannelData as ZokoMessage;
            string APIUrl = _configuration.GetSection("AppSettings").GetSection("APIUrl").Value;
            // ControlToAgent

            //bool toAgent = await _apInterService.IsToAgent(APIUrl, message.From, message.To);
            //if (toAgent)
            //    return;
#if !DEBUG
            bool toAgent = await _apInterService.IsToAgent(APIUrl, message.PlatformSenderId, _configuration.GetSection("ZokoNumber").Value);
            if (toAgent)
                return;
#endif
            var lastAccess = await LastAccessedTimeProperty.GetAsync(turnContext, () => DateTime.UtcNow, cancellationToken).ConfigureAwait(false);
            if ((DateTime.UtcNow - lastAccess) >= TimeSpan.FromMinutes(EXPIRATION_MINUTES))
            {
                await _conversationState.ClearStateAsync(turnContext, cancellationToken).ConfigureAwait(false);
            }
            await LastAccessedTimeProperty.SetAsync(turnContext, DateTime.UtcNow, cancellationToken).ConfigureAwait(false);
            await base.OnTurnAsync(turnContext, cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");
            var apInterCustomerAccesor = _userState.CreateProperty<APInterBotState>(nameof(APInterBotState));
            await apInterCustomerAccesor.GetAsync(turnContext, () => new APInterBotState(), cancellationToken);
            await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }
    }
}
