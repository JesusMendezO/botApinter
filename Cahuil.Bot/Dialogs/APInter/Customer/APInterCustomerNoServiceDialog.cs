using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerNoServiceDialog : ComponentDialog
    {
        #region Constructors
        public APInterCustomerNoServiceDialog() : base(nameof(APInterCustomerNoServiceDialog))
        {
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NoServiceStepAsync,
                NoServiceResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> NoServiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Por favor introduzca un teléfono de contacto.");
            await stepContext.Context.SendActivityAsync("Estimado Cliente, Le informamos que por la deuda que mantiene, su servicio se encuentra Inhabilitado y hemos emitido la orden para el retiro de la antena.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> NoServiceResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Title = "Servicio se encuentra Inhabilitado y hemos emitido la orden para el retiro de la antena";
            apInterBotState.Departament = 2;
            apInterBotState.Description = "Servicio se encuentra Inhabilitado y hemos emitido la orden para el retiro de la antena";
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            apInterBotState.IssueId = 1;
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
