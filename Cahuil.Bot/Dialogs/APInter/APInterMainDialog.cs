using Cahuil.Bot.Dialogs.APInter;
using Cahuil.Bot.Model;
using Cahuil.Bot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters.TwilioWhatsapp;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterMainDialog : ComponentDialog
    {
        #region Fields
        protected readonly ILogger _logger;
        protected readonly IConfiguration _configuration;
        private readonly IAPInterService _apInterService;
        private bool _backMenu = false;
        #endregion

        #region Constructors
        public APInterMainDialog(ILogger<APInterMainDialog> logger, IConfiguration configuration)
            : base(nameof(APInterMainDialog))
        {
            _logger = logger;
            _configuration = configuration;
            _apInterService = new APInterService();
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), BienvenidaPromptValidatorAsync));

            AddDialog(new APInterCustomerValidatorDialog());
            AddDialog(new APInterNoCustomerDialog());
            AddDialog(new ApInterHasTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                BienvenidaStepAsync,
                BienvenidaResolverStepAsync,
                APInterBackMenuResolverStepAsync,
                FinalStepAsync,
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> BienvenidaStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string messageSID = string.Empty;
            string to = string.Empty;
            ZokoMessage channelData = stepContext.Context.Activity.ChannelData as ZokoMessage;
            //Twilio
            //if (channelData != null)
            //{
            //    messageSID = channelData.MessageSid;
            //    to = channelData.To;
            //}
            if (channelData != null)
            {
                messageSID = channelData.Id;
                to = channelData.PlatformRecieverId;
            }
            APInterBotState apInterBotState = new APInterBotState() { ConversarionId = stepContext.Context.Activity.Id, From = stepContext.Context.Activity.From.Id, Channel = 2, To = to, MessageSIDStartSession = messageSID, APIUrl = _configuration.GetSection("AppSettings").GetSection("APIUrl").Value };
            int resultMessage = await _apInterService.BotSession(apInterBotState);
            stepContext.Values["APInterCustomer"] = apInterBotState;
            string bienvenidaMessage = @"Gracias por comunicarse con APINTER Internet WIFI
Soy un Robot 🤖 y voy a ayudarle a gestionar su pedido o reclamo. 
⛔ Por favor, responda solamente con números (sin espacios y puntos) las siguientes preguntas. 
Ingresar texto o foto SOLO en el caso que se lo requiera. 

📌 *Por favor Ingrese*
0️⃣ Si aún NO es Cliente.
1️⃣ Ingreso Clientes APINTER.";
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(bienvenidaMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> BienvenidaResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            switch ((int)stepContext.Result)
            {
                case 1:
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerValidatorDialog), stepContext.Values["APInterCustomer"], cancellationToken);
                default:
                    string from = stepContext.Context.Activity.From.Id;
                    APInterBotState apInterBotState = new APInterBotState();
                    apInterBotState.HasTicket = await _apInterService.HasTicketFrom(_configuration.GetSection("AppSettings").GetSection("APIUrl").Value, from);
                    if (apInterBotState.HasTicket > 0)
                    {
                        return await stepContext.BeginDialogAsync(nameof(ApInterHasTicketDialog), apInterBotState, cancellationToken);
                    }
                    else
                    {
                        return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerDialog), stepContext.Values["APInterCustomer"], cancellationToken);
                    }
            }
        }

        private async Task<DialogTurnResult> APInterBackMenuResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result != null && stepContext.Result.ToString() == "BACK_MENU")
            {
                _backMenu = true;
                return await stepContext.ReplaceDialogAsync(InitialDialogId, stepContext.Options, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        #endregion

        #region Validators
        private static Task<bool> BienvenidaPromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded && (promptContext.Recognized.Value == 0 || promptContext.Recognized.Value == 1));
        }
        #endregion
    }
}
