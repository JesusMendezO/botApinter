using Cahuil.Bot.Apinter.Prompt;
using Cahuil.Bot.Dialogs;
using Cahuil.Bot.Model;
using Cahuil.Bot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Apinter.Dialogs.APInter.Customer
{
    public class APInterCustomerWelcomeCustomMessageDialog : ComponentDialog
    {
        #region Fields
        private bool _backMenu = false;
        private readonly IAPInterService _aPInterService;
        #endregion

        #region Constructors
        public APInterCustomerWelcomeCustomMessageDialog() : base(nameof(APInterCustomerWelcomeCustomMessageDialog))
        {
            _aPInterService = new APInterService();
            AddDialog(new NumberPromptExtend<int>(nameof(NumberPromptExtend<int>), CustumerWelcomeMenuPromptValidatorAsync));
            AddDialog(new APInterCustomerWelcomeDialog());
            AddDialog(new APInterCustomerTechnicalServiceDialog());
            AddDialog(new APInterCustomerAdministrativeDialog());
            AddDialog(new APInterCustomerComercialDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
{
                APInterCustomerWelcomeMenuStepAsync,
                APInterCustomerWelcomeMenuResolverStepAsync,
                APInterBackMenuResolverStepAsync
}));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Public methods
        private async Task<DialogTurnResult> APInterCustomerWelcomeMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            if (_backMenu)
            {
                apInterBotState.BackMenu = false;
            }
            string promptMessage = string.Format(@"0️⃣ Conocer mas sobre *{0}*

1️⃣ Servicio Técnico
2️⃣ Consulta Administrativas
3️⃣ Consultas Comerciales", apInterBotState.CustomMessage.Name);
            var promptOptions = new PromptOptionsExtension
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
                MediaUrl = apInterBotState.CustomMessage.Body
            };
            return await stepContext.PromptAsync(nameof(NumberPromptExtend<int>), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> APInterCustomerWelcomeMenuResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            switch ((int)stepContext.Result)
            {
                case 0:
                    apInterBotState.IssueId = apInterBotState.CustomMessage.IssueId;
                    apInterBotState.Description = apInterBotState.CustomMessage.TicketDescription;
                    apInterBotState.MessageAttentionTicket = true;
                    APIResultMessage<int> resultMessage = await _aPInterService.CreateTicket(apInterBotState);
                    await stepContext.Context.SendActivityAsync(string.Format(@"Un asesor comercial se comunicara a la brevedad, para brindarte información sobre *{0}*
¡Muchas gracias!", apInterBotState.CustomMessage.Name));
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerWelcomeDialog), stepContext.Options, cancellationToken);
                case 1:
                    apInterBotState.Departament = 1;
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerTechnicalServiceDialog), stepContext.Options, cancellationToken);
                case 2:
                    apInterBotState.Departament = 2;
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativeDialog), stepContext.Options, cancellationToken);
                case 3:
                default:
                    apInterBotState.Departament = 3;
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerComercialDialog), stepContext.Options, cancellationToken);
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
        #endregion

        #region Validators
        private static Task<bool> CustumerWelcomeMenuPromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded && (promptContext.Recognized.Value >= 0 && promptContext.Recognized.Value <= 3));
        }
        #endregion
    }
}
