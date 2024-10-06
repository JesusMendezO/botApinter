using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerAdministrativeDialog : ComponentDialog
    {
        private int _customerStatus;
        #region Constructors
        public APInterCustomerAdministrativeDialog() : base(nameof(APInterCustomerAdministrativeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt), CustomerAdministrativeMenuPromptValidatorAsync));
            AddDialog(new APInterCustomerAdministrativeAccountBalanceDialog());
            AddDialog(new APInterCustomerAdministrativePaymentMethodDialog());
            AddDialog(new APInterCustomerAdministrativeForwardingInvoiceDialog());
            AddDialog(new APInterCustomerAdministrativeClaimAccountDialog());
            AddDialog(new APInterCustomerAdministrativeRequestToMoveDialog());
            AddDialog(new APInterCustomerAdministrativeChangeHardwareDialog());
            AddDialog(new APInterCustomerAdministrativeOtherActionsDialog());
            AddDialog(new APInterCustomerAdministrativeReportPaymentDialog());
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
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            string promptMessage;
            _customerStatus = apInterBotState.Status;
            
            if (apInterBotState.Status == 0)
            {
                promptMessage = string.Format(@"📌 Elija una de las siguientes opciones
1️⃣ Conocer saldo.
2️⃣ Informar un pago realizado.
3️⃣ Medios de pagos habilitados. 
4️⃣ Dudas con su cuenta.
5️⃣ Mudanza del servicio. 
6️⃣ Reenvio de su factura. 
7️⃣ Cambio de antena o router de lugar.
8️⃣ Otras gestiones administrativas

*️⃣ Volver al Menu Anterior");
            }
            else
            {
                promptMessage = string.Format(@"📌 Elija una de las siguientes opciones
1️⃣ Conocer saldo.
2️⃣ Informar un pago realizado.
3️⃣ Medios de pagos habilitados. 
4️⃣ Dudas con su cuenta.
5️⃣ Mudanza del servicio. 
6️⃣ Reenvio de su factura. 
7️⃣ Cambio de antena o router de lugar.
8️⃣ Otras gestiones administrativas");
            }
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> CustomerAdministrativeMenuResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Description = "";
            switch (stepContext.Result.ToString())
            {
                case "1":
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativeAccountBalanceDialog), stepContext.Options, cancellationToken);
                case "2":
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativeReportPaymentDialog), stepContext.Options, cancellationToken);
                case "3":
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativePaymentMethodDialog), stepContext.Options, cancellationToken);
                case "4":
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativeClaimAccountDialog), stepContext.Options, cancellationToken);
                case "5":
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativeRequestToMoveDialog), stepContext.Options, cancellationToken);
                case "6":
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativeForwardingInvoiceDialog), stepContext.Options, cancellationToken);
                case "7":
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativeChangeHardwareDialog), stepContext.Options, cancellationToken);
                case "8":
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativeOtherActionsDialog), stepContext.Options, cancellationToken);
                case "*":
                default:
                    return await stepContext.EndDialogAsync("BACK_MENU", cancellationToken);
            }
        }
        #endregion

        #region Validators
        private Task<bool> CustomerAdministrativeMenuPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            string[] values = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "*" };
            if (_customerStatus == 1)
                values = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };
            return Task.FromResult(promptContext.Recognized.Succeeded && values.Any(v => v.Contains(promptContext.Recognized.Value)));
        }
        #endregion
    }
}
