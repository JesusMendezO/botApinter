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
    public class APInterNoCustomerContractUrbanDialog : ComponentDialog
    {
        #region Fields
        string _domicilio;
        #endregion

        #region Constructors
        public APInterNoCustomerContractUrbanDialog() : base(nameof(APInterNoCustomerContractUrbanDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new APInterNoCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NoCustomerContractUrbanStepAsync,
                NoCustomerContractUrbanResolverStepAsync,
                NoCustomerContractUrbanTelehponeStepAsync,
                NoCustomerContractUrbanTelehponeResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> NoCustomerContractUrbanStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

        private async Task<DialogTurnResult> NoCustomerContractUrbanTelehponeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
        private async Task<DialogTurnResult> NoCustomerContractUrbanResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _domicilio = stepContext.Result.ToString();
            return await stepContext.NextAsync(null, cancellationToken);
        }
        private async Task<DialogTurnResult> NoCustomerContractUrbanTelehponeResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Title = "Solicitud de contrato de servicio urbano.";
            apInterBotState.Description = "Solicitud de contrato de servicio urbano. Datos del domicilio: " + _domicilio;
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            apInterBotState.IssueId = 14;
            apInterBotState.MessageAttentionTicket = true;
            return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
