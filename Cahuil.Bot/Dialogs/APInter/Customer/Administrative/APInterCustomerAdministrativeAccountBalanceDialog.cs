using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerAdministrativeAccountBalanceDialog : ComponentDialog
    {
        public APInterCustomerAdministrativeAccountBalanceDialog()
        {
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AdministrativeAccountBalanceStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }

        #region Steps
        private async Task<DialogTurnResult> AdministrativeAccountBalanceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Title = "Consulta de Saldo.";
            apInterBotState.IssueId = 3;
            apInterBotState.MessageAttentionTicket = false;
            apInterBotState.Closed = true;
            string message = string.Format(@"El saldo vencido de su cuenta es de *${0}*
El saldo total de su cuenta es de *${1}*

✅ Recuerde que debe cancelar su factura mensual antes del Ultimo día hábil de cada mes para evitar ser SUSPENDIDO", apInterBotState.DueDebt, apInterBotState.Debt);
            await stepContext.Context.SendActivityAsync(message);
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
