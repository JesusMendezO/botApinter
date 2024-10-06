using Cahuil.Bot.Model;
using Cahuil.Bot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerGenerateTicketDialog : ComponentDialog
    {
        #region Fields
        private readonly IAPInterService _apInterService;
        #endregion

        #region Constructors
        public APInterCustomerGenerateTicketDialog()
            : base(nameof(APInterCustomerGenerateTicketDialog))
        {
            _apInterService = new APInterService();
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CustomerGenerateTicket
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> CustomerGenerateTicket(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            APIResultMessage<int> resultMessage = await _apInterService.CreateTicket(apInterBotState);
            if (apInterBotState.MessageAttentionTicket)
            {
                await stepContext.Context.SendActivityAsync(string.Format(@"Un operador se contactara a la brevedad con usted, en nuestro horario de atención de lunes a viernes de 8 a 17hs y los días sábados de 8 a 12hs. 
Se ha registrado su solicitud con el siguiente número de ticket: *{0}* 
Gracias {1} por contactarse con nosotros!", resultMessage.Entity, apInterBotState.Fullname));
                await GetGoodbyeMessage(stepContext, apInterBotState);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(string.Format(@"Se ha registrado su solicitud con el siguiente número de ticket: *{0}*
Gracias {1} por contactarse con nosotros!", resultMessage.Entity, apInterBotState.Fullname));
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        #endregion

        #region Private methods
        public async Task GetGoodbyeMessage(WaterfallStepContext stepContext, APInterBotState aPInterBotState)
        {
            CustomMessage customMessage = await _apInterService.GetTemporaryCustomMessage(aPInterBotState.APIUrl, 4);
            if (customMessage != null && customMessage.Enabled)
                await stepContext.Context.SendActivityAsync(customMessage.Body);
        }
        #endregion
    }
}
