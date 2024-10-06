using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerSuspendWelcomeDialog : ComponentDialog
    {
        #region Constructors
        public APInterCustomerSuspendWelcomeDialog() : base(nameof(APInterCustomerSuspendWelcomeDialog))
        {
            AddDialog(new APInterCustomerAdministrativeDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                APInterCustomerSuspendWelcomeStepAsync,
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> APInterCustomerSuspendWelcomeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            //            string promptMessage = string.Format(@"Hola *{0}*!
            //👤 Estado de su cuenta: *{1}*", apInterBotState.Fullname, apInterBotState.NameStatus);
            //            await stepContext.Context.SendActivityAsync(promptMessage);
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerAdministrativeDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
