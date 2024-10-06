using Cahuil.Bot.Model;
using Cahuil.Bot.Service;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterNoCustomerGenerateTicketDialog : ComponentDialog
    {
        #region Fields
        private readonly IAPInterService _apInterService;
        #endregion

        #region Constructors
        public APInterNoCustomerGenerateTicketDialog()
            : base(nameof(APInterNoCustomerGenerateTicketDialog))
        {
            _apInterService = new APInterService();
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NoCustomerGenerateTicket
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> NoCustomerGenerateTicket(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.CustomerId = "000000";
            apInterBotState.Fullname = "No cliente";
            APIResultMessage<int> resultMessage = await _apInterService.CreateTicket(apInterBotState);
            string message = string.Format(@"Un operador se contactara a la brevedad con usted, en nuestro horario de atención de lunes a viernes de 8 a 17 hs y los días sábados de 8 a 12hs
Se ha registrado su solicitud con el siguiente número de ticket: *{0}*
Gracias por contactarse con nosotros!", resultMessage.Entity);
            await stepContext.Context.SendActivityAsync(message);
            await GetGoodbyeMessage(stepContext, apInterBotState);
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
