using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerAdministrativePaymentMethodDialog : ComponentDialog
    {
        #region Constructors
        public APInterCustomerAdministrativePaymentMethodDialog() : base(nameof(APInterCustomerAdministrativePaymentMethodDialog))
        {
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), CustomerAdministrativeMenuPromptValidatorAsync));
            AddDialog(new APInterCustomerAdministrativePaymentMethodCashDialog());
            AddDialog(new APInterCustomerAdministrativePaymentMethodBankingDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CustomerAdministrativeMenuStepAsync,
                CustomerAdministrativeMenuResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> CustomerAdministrativeMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"1️⃣ Para conocer los puntos de cobranza habilitados para pago en efectivo.
2️⃣ Para abonar por medio de débito automático.
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
        private async Task<DialogTurnResult> CustomerAdministrativeMenuResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            switch ((int)stepContext.Result)
            {
                case 1:
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativePaymentMethodCashDialog), stepContext.Options, cancellationToken);
                default:
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativePaymentMethodBankingDialog), stepContext.Options, cancellationToken);
            }
        }
        #endregion

        #region Validator
        private static Task<bool> CustomerAdministrativeMenuPromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded && (promptContext.Recognized.Value == 1 || promptContext.Recognized.Value == 2));
        }
        #endregion

    }
}
