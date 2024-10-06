using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerAdministrativePaymentMethodBankingDialog : ComponentDialog
    {
        #region Constructors
        public APInterCustomerAdministrativePaymentMethodBankingDialog() : base(nameof(APInterCustomerAdministrativePaymentMethodBankingDialog))
        {
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AdministrativePaymentMethodBankingStepAsync,
                AdministrativePaymentMethodBankingResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> AdministrativePaymentMethodBankingStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Estimado cliente:
Le informamos que contamos con el funcionamiento DEBITO DIRECTO CBU, con este método evita el pago en forma presencial o de recordar realizar la transferencia.
La fecha en la cual se realiza el débito es el quinto día hábil del mes, debitaremos el importe de la factura correspondiente, de la caja
de ahorro o cuenta corriente que usted nos informe.
Para adherirse, debe enviarnos a continuación Imagen del comprobante del CBU ( foto de la constancia que emite el cajero o constancia descargada desde el homebanking).
Recuerde que es debito en cuenta por lo tanto no aceptamos tarjeta de crédito.
Recuerde que en el caso de que su cuenta no tenga fondos se le cobraran $10 adicionales.
📌 Por favor, enviarnos Imagen del comprobante del CBU.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AdministrativePaymentMethodBankingResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.MessageAttentionTicket = true;
            apInterBotState.Title = "Medios de pagos habilitados - Debito Automático.";
            apInterBotState.IssueId = 7;
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
