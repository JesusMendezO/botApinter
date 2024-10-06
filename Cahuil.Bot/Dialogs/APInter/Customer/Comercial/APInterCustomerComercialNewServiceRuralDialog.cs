using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs.APInter.Customer
{
    public class APInterCustomerComercialNewServiceRuralDialog : ComponentDialog
    {
        #region Fields
        string _ubicacion;
        #endregion

        #region Constructors
        public APInterCustomerComercialNewServiceRuralDialog() : base(nameof(APInterCustomerComercialNewServiceRuralDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ComercialNewServiceRuralStepAsync,
                ComercialNewServiceRuralStepAsyncResolverStepAsync,
                ComercialNewServiceRuralTelehponeStepAsync,
                ComercialNewServiceRuralTelehponeSResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> ComercialNewServiceRuralStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"📌 Por favor compártanos su ubicación.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ComercialNewServiceRuralTelehponeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
        private async Task<DialogTurnResult> ComercialNewServiceRuralStepAsyncResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _ubicacion = stepContext.Result.ToString();
            return await stepContext.NextAsync(null, cancellationToken);
        }
        private async Task<DialogTurnResult> ComercialNewServiceRuralTelehponeSResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Title = "Solicitud de nuevo servicio rural.";
            apInterBotState.Description = "Datos del domicilio: " + _ubicacion;
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            apInterBotState.IssueId = 13;
            apInterBotState.MessageAttentionTicket = true;
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
