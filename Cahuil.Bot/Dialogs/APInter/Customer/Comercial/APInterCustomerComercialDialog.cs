using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerComercialDialog : ComponentDialog
    {
        #region Constructors
        public APInterCustomerComercialDialog() : base(nameof(APInterCustomerComercialDialog))
        {
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), CustomerComercialMenuPromptValidatorAsync));
            AddDialog(new APInterCustomerComercialNewServiceDialog());
            AddDialog(new APInterCustomerComercialChangeSuscriptionDialog());
            AddDialog(new APInterCustomerComercialPurchaseHardwareDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CustomerComercialMenuStepAsync,
                CustomerComercialMenuResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> CustomerComercialMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"1️⃣ Solicitar un nuevo servicio.
2️⃣ Solicitar el cambio de su abono. 
3️⃣ Adquirir routers y otros equipos. 
");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> CustomerComercialMenuResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Description = "";
            switch ((int)stepContext.Result)
            {
                case 1:
                    apInterBotState.Title = "Solicitar un nuevo servicio";
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerComercialNewServiceDialog), stepContext.Options, cancellationToken);
                case 2:
                    apInterBotState.Title = "Solicitar el cambio de su abono";
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerComercialChangeSuscriptionDialog), stepContext.Options, cancellationToken);
                case 3:
                default:
                    apInterBotState.Title = "Adquirir routers u otros equipos";
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerComercialPurchaseHardwareDialog), stepContext.Options, cancellationToken);
            }
        }
        #endregion

        #region Validators
        private static Task<bool> CustomerComercialMenuPromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded && (promptContext.Recognized.Value >= 1 && promptContext.Recognized.Value <= 3));
        }
        #endregion
    }
}
