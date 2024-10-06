using Cahuil.Bot.Model;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs.APInter
{
    public class ApInterHasTicketDialog : ComponentDialog
    {
        #region Constructors
        public ApInterHasTicketDialog()
            : base(nameof(ApInterHasTicketDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                HasTicketDialog
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> HasTicketDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            await stepContext.Context.SendActivityAsync(string.Format(@"Le informamos que estamos gestionando su ticket número *{0}* . Un asesor se comunicara con usted en nuestro horario de atención.
Muchas gracias.", apInterBotState.HasTicket));
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        #endregion
    }
}
