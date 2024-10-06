using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterNoCustomerDialog : ComponentDialog
    {
        #region Constructors
        public APInterNoCustomerDialog() : base(nameof(APInterNoCustomerDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt), NoCustomerMenuPromptValidatorAsync));
            AddDialog(new APInterNoCustomerContractDialog());
            AddDialog(new APInterNoCustomerPendingRequestDialog());
            AddDialog(new APInterNoCustomerReinstallDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NoCustomerMenuStepAsync,
                NoCustomerMenuResultStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region public methods
        private static async Task<DialogTurnResult> NoCustomerMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.IsCustomer = false;
            apInterBotState.CustomerId = "0";
            apInterBotState.Departament = 3;
            var promptOptions = new PromptOptions { 
                Prompt = MessageFactory.Text(@"📌 Indique el motivo de su contacto
1️⃣ Si desea contratar nuestros servicios.
2️⃣ Si desea consultar por su Solicitud pendiente de instalación.
3️⃣ Si fue Cliente y desea reinstalar la antena.

*️⃣ Volver al Menu Anterior."),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private static async Task<DialogTurnResult> NoCustomerMenuResultStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            switch (stepContext.Result.ToString())
            {
                case "1":
                    apInterBotState.Title = "Contratar servicios";
                    return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerContractDialog), stepContext.Options, cancellationToken);
                case "2":
                    apInterBotState.Title = "Consulta por solicitud pendiente";
                    return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerPendingRequestDialog), stepContext.Options, cancellationToken);
                case "3":
                    apInterBotState.Title = "Reinstalación de antena";
                    return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerReinstallDialog), stepContext.Options, cancellationToken);
                case "*":
                default:
                    return await stepContext.EndDialogAsync("BACK_MENU", cancellationToken);
            }
        }
        #endregion

        #region Validators
        private static Task<bool> NoCustomerMenuPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            string[] values = { "1", "2", "3", "*" };
            return Task.FromResult(promptContext.Recognized.Succeeded && values.Any(v => v.Contains(promptContext.Recognized.Value)));
        }
        #endregion
    }
}
