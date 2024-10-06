using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs.APInter.Customer
{
    public class APInterCustomerComercialNewServiceCompanyDialog : ComponentDialog
    {
        #region Fields
        string _domicilio;
        #endregion

        #region Constructors
        public APInterCustomerComercialNewServiceCompanyDialog() : base(nameof(APInterCustomerComercialNewServiceCompanyDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ComercialNewServiceCompanyStepAsync,
                ComercialNewServiceCompanyStepAsyncResolverStepAsync,
                ComercialNewServiceCompanyTelehponeStepAsync,
                ComercialNewServiceCompanyTelehponeSResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> ComercialNewServiceCompanyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"📌 Para verificar la cobertura del servicio necesitamos que nos indique:
-Localidad
-Calle y entre calles");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ComercialNewServiceCompanyTelehponeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
        private async Task<DialogTurnResult> ComercialNewServiceCompanyStepAsyncResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _domicilio = stepContext.Result.ToString();
            return await stepContext.NextAsync(null, cancellationToken);
        }
        private async Task<DialogTurnResult> ComercialNewServiceCompanyTelehponeSResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Title = "Solicitud de nuevo servicio empresa.";
            apInterBotState.Description = "Datos del domicilio: " + _domicilio;
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            apInterBotState.IssueId = 13;
            apInterBotState.MessageAttentionTicket = true;
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
