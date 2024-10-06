using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerWelcomeDialog : ComponentDialog
    {
        #region Fields
        private bool _backMenu = false;
        #endregion

        #region Constructors
        public APInterCustomerWelcomeDialog() : base(nameof(APInterCustomerWelcomeDialog))
        {
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), CustumerWelcomeMenuPromptValidatorAsync));
            AddDialog(new APInterCustomerTechnicalServiceDialog());
            AddDialog(new APInterCustomerAdministrativeDialog());
            AddDialog(new APInterCustomerComercialDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                APInterCustomerWelcomeStepAsync,
                APInterCustomerWelcomeMenuResolverStepAsync,
                APInterBackMenuResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> APInterCustomerWelcomeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            string promptMessage = string.Format(@"1️⃣ Servicio Técnico
2️⃣ Consulta Administrativas 
3️⃣ Consultas Comerciales");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> APInterCustomerWelcomeMenuResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            switch ((int)stepContext.Result)
            {
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
            return Task.FromResult(promptContext.Recognized.Succeeded && (promptContext.Recognized.Value >= 1 && promptContext.Recognized.Value <= 3));
        }
        #endregion
    }
}
