using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerTechnicalServiceDialog : ComponentDialog
    {
        #region Constructors
        public APInterCustomerTechnicalServiceDialog()
            : base(nameof(APInterCustomerTechnicalServiceDialog))
        {
            AddDialog((new TextPrompt(nameof(TextPrompt), TechnicalServiceMenuPromptValidatorAsync)));
            AddDialog(new APInterCustomerTechnicalServiceReasonDialog());
            AddDialog(new APInterCustomerTechnicalServiceContact());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CustomerTechnicalServiceMenuMessageDialog,
                CustomerTechnicalServiceMenuDialog,
                CustomerTechnicalServiceMenuResolverDialog,
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> CustomerTechnicalServiceMenuMessageDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Recuerde verificar las conexiones de sus equipos y de reiniciar
los mismos por 5 minutos para verificar si el problema persiste
o se ha solucionado");
            await stepContext.Context.SendActivityAsync(promptMessage);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> CustomerTechnicalServiceMenuDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"📌 Elija una de las siguientes opciones
1️⃣ No tiene Internet.
2️⃣ Problemas de servicio.
3️⃣ Problemas con Router WIFI.
4️⃣ Cambio de Clave WIFI.
5️⃣ Otras consultas técnicas marque.

*️⃣ Volver al menú anterior.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolvers
        private async Task<DialogTurnResult> CustomerTechnicalServiceMenuResolverDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            switch (stepContext.Result.ToString())
            {
                case "1":
                    apInterBotState.Title = "No tiene Internet";
                    apInterBotState.IssueId = 1;
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerTechnicalServiceReasonDialog), stepContext.Options, cancellationToken);
                case "2":
                    apInterBotState.Title = "Problemas con el servicio";
                    apInterBotState.IssueId = 1;
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerTechnicalServiceReasonDialog), stepContext.Options, cancellationToken);
                case "3":
                    apInterBotState.Title = "Problemas con el Router WIFI";
                    apInterBotState.IssueId = 1;
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerTechnicalServiceReasonDialog), stepContext.Options, cancellationToken);
                case "5":
                    apInterBotState.Title = "Consultas técnicas";
                    apInterBotState.IssueId = 5;
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerTechnicalServiceReasonDialog), stepContext.Options, cancellationToken);
                case "4":
                    apInterBotState.Title = "Cambio de Clave WIFI";
                    apInterBotState.Description = "Consulta o reclamo: Cambio de clave WIFI";
                    apInterBotState.IssueId = 2;
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerTechnicalServiceContact), stepContext.Options, cancellationToken);
                case "*":
                default:
                    return await stepContext.EndDialogAsync("BACK_MENU", cancellationToken);
            }
        }
        #endregion

        #region Validators
        private static Task<bool> TechnicalServiceMenuPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            string[] values = { "1","2","3","4","5","*"};
            return Task.FromResult(promptContext.Recognized.Succeeded && values.Any(v => v.Contains(promptContext.Recognized.Value)));
        }
        #endregion
    }
}



