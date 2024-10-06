using Cahuil.Bot.Model;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs.APInter
{
    public class ApInterIsMassiveDialog: ComponentDialog
    {
        #region Constructors
        public ApInterIsMassiveDialog()
            : base(nameof(ApInterIsMassiveDialog))
        {
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IsMassiveDialog
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> IsMassiveDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            await stepContext.Context.SendActivityAsync(string.Format(apInterBotState.Massive.Message));
            apInterBotState.MessageAttentionTicket = true;
            apInterBotState.IssueId = 24;
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion

    }
}
