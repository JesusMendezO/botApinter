using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerAdministrativeRequestToMoveDialog : ComponentDialog
    {
        #region Fields
        string _domicilio = "";
        #endregion

        #region Constructors
        public APInterCustomerAdministrativeRequestToMoveDialog() : base(nameof(APInterCustomerAdministrativeRequestToMoveDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                RequestToMoveReasonStepAsync,
                RequestToMoveReasonResolverStepAsync,
                RequestToMoveTelephoneStepAsync,
                RequestToMoveTelephoneResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> RequestToMoveReasonStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Ingrese solo con TEXTO (No audio ni imagen) la dirección del nuevo domicilio, indicando:
📌 Calle
📌 Altura
📌 Entre calles
📌 Localidad");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> RequestToMoveTelephoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Por favor introduzca un teléfono de contacto.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> RequestToMoveReasonResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _domicilio = stepContext.Result.ToString();
            return await stepContext.NextAsync(null, cancellationToken);
        }
        private async Task<DialogTurnResult> RequestToMoveTelephoneResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Description = $"<b>Domicilio de mudanza: </b> {_domicilio}";
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            apInterBotState.IssueId = 9;
            apInterBotState.MessageAttentionTicket = true;
            await stepContext.Context.SendActivityAsync(@"Le informamos que el tramite de cambio de cambio de domicilio tiene un costo de $2000");
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
