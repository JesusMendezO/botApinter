using Cahuil.Bot.Model;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerAdministrativePaymentMethodCashDialog : ComponentDialog
    {
        #region Constructors
        public APInterCustomerAdministrativePaymentMethodCashDialog() : base(nameof(APInterCustomerAdministrativePaymentMethodCashDialog))
        {
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AdministrativePaymentMethodCashStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> AdministrativePaymentMethodCashStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Closed = true;
            apInterBotState.Title = "Medios de pagos habilitados - Efectivo";
            apInterBotState.IssueId = 7;
            apInterBotState.MessageAttentionTicket = false;
            apInterBotState.Closed = true;
            string message = string.Format(@"Puede abonar su servicio en efectivo en:
PAGO SIN FACTURA
- Oficina 'APINTER'
HipolitoYrigoyen 233
- Polirubros 'Luna'
Hnos Islas 1043 esq.Libertad
- Drugstore 'Galápagos'
Elizondo esq.entre ríos
* Puede abonar solo con Apellido y Nombre de
Titular o DNI
PAGO CON FACTURA
Red Rapipagos, Pago Facil, Pronto
pago, Ripsa y Provincia Net.Presentando
factura de nuestros servicios.");
            await stepContext.Context.SendActivityAsync(message);
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
