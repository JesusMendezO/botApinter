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
    public class APInterCustomerTechnicalServiceContact : ComponentDialog
    {
        #region Constructors
        public APInterCustomerTechnicalServiceContact() : base(nameof(APInterCustomerTechnicalServiceContact))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CustomerTechnicalServiceContactDialog,
                CustomerTechnicalServiceContactResolverDialog
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> CustomerTechnicalServiceContactDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Ingrese teléfono de contacto.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion


        #region Resolver()
        private async Task<DialogTurnResult> CustomerTechnicalServiceContactResolverDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            apInterBotState.MessageAttentionTicket = true;
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion

    }
}
