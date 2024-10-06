using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterNoCustomerPendingRequestDialog : ComponentDialog
    {
        #region Fields
        string _ticketNumber = string.Empty;
        string _telephoneNumber = string.Empty;
        #endregion

        #region Constructors
        public APInterNoCustomerPendingRequestDialog() : base(nameof(APInterNoCustomerPendingRequestDialog))
        {
            AddDialog(new APInterNoCustomerGenerateTicketDialog());
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                APInterNoCustomerPendingRequestNumberStepAsync,
                APInterNoCustomerPendingRequestTelephoneStepAsync,
                APInterNoCustomerPendingRequestQureyStepAsync,
                APInterNoCustomerPendingRequestFinalStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> APInterNoCustomerPendingRequestNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = @"📌 Por favor introduzca el número de Solicitud de Servicio";
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> APInterNoCustomerPendingRequestTelephoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _ticketNumber = stepContext.Result.ToString();
            string promptMessage = @"Por favor introduzca un numero de telefono de contacto";
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> APInterNoCustomerPendingRequestQureyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _telephoneNumber = stepContext.Result.ToString();
            string promptMessage = @"📌 Ingrese su consulta Ingrese solo con TEXTO (No audio ni imagen) su consulta";
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> APInterNoCustomerPendingRequestFinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Title = "Consulta por solicitud pendiente de instalación";
            apInterBotState.Description = "Consulta por solicitud pendiente de instalación. solicitud número: " + _ticketNumber + ", consulta: " + stepContext.Result.ToString();
            apInterBotState.ContactNumber = _telephoneNumber;
            apInterBotState.IssueId = 18;
            return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
