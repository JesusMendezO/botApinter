using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterNoCustomerReinstallDialog : ComponentDialog
    {
        #region Fields
        string _telephoneNumber = string.Empty;
        string _ticketNumber = string.Empty;
        #endregion

        #region Constructors
        public APInterNoCustomerReinstallDialog() : base(nameof(APInterNoCustomerReinstallDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new APInterNoCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                APInterNoCustomerReinstallTelephoneStepAsync,
                APInterNoCustomerReinstallResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> APInterNoCustomerReinstallTelephoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = @"Ingrese Teléfono de Contacto";
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> APInterNoCustomerReinstallResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.IssueId = 19;
            apInterBotState.Title = "Fue Cliente y desea reinstalar la antena";
            apInterBotState.Description = "Fue Cliente y desea reinstalar la antena.";
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
