using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterNoCustomerContractDialog : ComponentDialog
    {
        #region Constructors
        public APInterNoCustomerContractDialog() : base(nameof(APInterNoCustomerContractDialog))
        {
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), APInterNoCustomerContractPromptValidatorAsync));
            AddDialog(new APInterNoCustomerContractUrbanDialog());
            AddDialog(new APInterNoCustomerContractRuralDialog());
            AddDialog(new APInterNoCustomerContractCompanyDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                APInterNoCustomerContractStepAsync,
                APInterNoCustomerContractResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> APInterNoCustomerContractStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"📌 Tipo de servicio
1️⃣ Urbana
2️⃣ Rural 
3️⃣ Empresa
");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> APInterNoCustomerContractResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Description = "";
            switch ((int)stepContext.Result)
            {
                case 1:
                    return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerContractUrbanDialog), stepContext.Options, cancellationToken);
                case 2:
                    return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerContractRuralDialog), stepContext.Options, cancellationToken);
                case 3:
                default:
                    return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerContractCompanyDialog), stepContext.Options, cancellationToken);
            }
            
        }
        #endregion

        #region Validators
        private static Task<bool> APInterNoCustomerContractPromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded && (promptContext.Recognized.Value >= 1 && promptContext.Recognized.Value <= 3));
        }
        #endregion
    }
}
