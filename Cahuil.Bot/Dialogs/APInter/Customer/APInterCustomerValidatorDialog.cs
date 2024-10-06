using Cahuil.Bot.Apinter.Dialogs.APInter.Customer;
using Cahuil.Bot.Dialogs.APInter;
using Cahuil.Bot.Model;
using Cahuil.Bot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    #region APInterCustomerValidatorDialog
    public class APInterCustomerValidatorDialog : ComponentDialog
    {
        #region Fields
        private APInterCustomer _apInterCustomer;
        private APInterBotState _apInterBotState;
        private readonly IAPInterService _apInterService;
        #endregion

        #region Constructors
        public APInterCustomerValidatorDialog() : base(nameof(APInterCustomerValidatorDialog))
        {
            _apInterService = new APInterService();
            _apInterCustomer = new APInterCustomer();
            AddDialog(new TextPrompt(nameof(TextPrompt), TextPromptValidatorAsync));
            AddDialog(new APInterCustomerNoIdOwner());
            AddDialog(new APInterCustomerWelcomeDialog());
            AddDialog(new APInterCustomerSuspendWelcomeDialog());
            AddDialog(new APInterCustomerNoServiceDialog());
            AddDialog(new ApInterHasTicketDialog());
            AddDialog(new APInterCustomerEmailDialog());
            AddDialog(new APInterNoCustomerGenerateTicketDialog());
            AddDialog(new APInterCustomerWelcomeCustomMessageDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CustumerValidationStepAsync,
                CustomerValidationResolverStep1Async,
                CustomerValidationResolverStep2Async
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> CustumerValidationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.IsCustomer = true;
            _apInterBotState = apInterBotState;
            string promptMessage = @"Estimado Cliente, Su número de Cliente APINTER es requerido para  cualquier gestión que desee efectuar en nuestro Centro de Atención al  Cliente. 
🧾 El mismo se encuentra en su factura o recibo, a continuación del titular del servicio.  

🔢 Ingrese su Número de Cliente de 6 dígitos 
0️⃣ Si desea consultar su número de cliente con un operador.

*️⃣ Volver al Menu Anterior";

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolvers
        private async Task<DialogTurnResult> CustomerValidationResolverStep1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            if (_apInterCustomer.CustomerId == "-1")
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            else if (_apInterCustomer.CustomerId == "0")
            {
                apInterBotState.Title = "Consulta por número de cliente.";
                apInterBotState.IssueId = 17;
                apInterBotState.MessageAttentionTicket = true;
                return await stepContext.BeginDialogAsync(nameof(APInterCustomerNoIdOwner), apInterBotState, cancellationToken);
            }
            else if (_apInterCustomer.CustomerId == "*")
            {
                return await stepContext.EndDialogAsync("BACK_MENU", cancellationToken);
            }
            else
            {
                apInterBotState.CustomerId = _apInterCustomer.CustomerId;
                apInterBotState.Fullname = _apInterCustomer.Fullname;
                apInterBotState.NameStatus = _apInterCustomer.NameStatus;
                apInterBotState.Email = _apInterCustomer.Mail;
                apInterBotState.DNI = _apInterCustomer.DNI;
                apInterBotState.DueDebt = _apInterCustomer.DueDebt;
                apInterBotState.Debt = _apInterCustomer.Debt;
                switch (_apInterCustomer.Status)
                {
                    case "1":
                        apInterBotState.NameStatus = "Suspendido por falta de pago";
                        apInterBotState.Status = 1;
                        await stepContext.Context.SendActivityAsync(@"Estimado Cliente, recuerde que debe estar al día con el servicio, abonando del 1 al 10 de cada mes por los medios habilitados");
                        await stepContext.Context.SendActivityAsync(string.Format(@"Hola *{0}*! 
👤 Estado de su cuenta: *{1}*", apInterBotState.Fullname, apInterBotState.NameStatus));
                        break;
                    //apInterBotState.HasTicket = await _apInterService.HasTicket(_apInterBotState.APIUrl, apInterBotState.CustomerId);
                    //if (apInterBotState.HasTicket > 0)
                    //{
                    //    return await stepContext.BeginDialogAsync(nameof(ApInterHasTicketDialog), apInterBotState, cancellationToken);
                    //}
                    //else
                    //{
                    //    return await stepContext.BeginDialogAsync(nameof(APInterCustomerSuspendWelcomeDialog), apInterBotState, cancellationToken);
                    //}
                    default:
                        apInterBotState.Status = 0;
                        apInterBotState.NameStatus = "Habilitado";
                        await stepContext.Context.SendActivityAsync(string.Format(@"Hola *{0}*! 
👤 Estado de su cuenta: *{1}*", apInterBotState.Fullname, apInterBotState.NameStatus));
                        break;

                }
                Thread.Sleep(3000);
                await GetWelcomeMessage(stepContext, apInterBotState);
                // Check Email
                if (string.IsNullOrEmpty(apInterBotState.Email))
                {
                    return await stepContext.BeginDialogAsync(nameof(APInterCustomerEmailDialog), apInterBotState, cancellationToken);
                }
                else
                {
                    return await stepContext.NextAsync(new CustomDialogResult() { Success = false, Origin = "Email" });
                }
            }
        }

        private async Task<DialogTurnResult> CustomerValidationResolverStep2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            CustomDialogResult customDialogResult = stepContext.Result as CustomDialogResult;
            // Check NoId
            if (customDialogResult.Success && customDialogResult.Origin == "NoId")
                return await stepContext.EndDialogAsync();
            // Update Mail
            if (customDialogResult.Success && customDialogResult.Origin == "Email")
                apInterBotState.Email = $"{customDialogResult.Value} (!)";
            // Has Ticket
            apInterBotState.HasTicket = await _apInterService.HasTicket(apInterBotState.APIUrl, _apInterCustomer.CustomerId);
            if (apInterBotState.HasTicket > 0)
            {
                return await stepContext.BeginDialogAsync(nameof(ApInterHasTicketDialog), apInterBotState, cancellationToken);
            }
            else // Is Massive
            {
                Massive massive = await _apInterService.IsMassive(apInterBotState.APIUrl);
                if (massive.Enabled)
                {
                    apInterBotState.Massive = massive;
                    return await stepContext.BeginDialogAsync(nameof(ApInterIsMassiveDialog), apInterBotState, cancellationToken);
                }
                else
                {
                    if (apInterBotState.Status == 1)
                    {
                        return await stepContext.BeginDialogAsync(nameof(APInterCustomerSuspendWelcomeDialog), apInterBotState, cancellationToken);
                    }
                    else
                    {
                        ResponseMessage<CustomMessage> customMessageResult = await _apInterService.GetCustomMessage(apInterBotState.APIUrl, apInterBotState.CustomerId);

                        if (customMessageResult.Success)
                        {
                            apInterBotState.CustomMessage = customMessageResult.Entity;
                            return await stepContext.BeginDialogAsync(nameof(APInterCustomerWelcomeCustomMessageDialog), apInterBotState, cancellationToken);
                        }
                        else
                        {
                            return await stepContext.BeginDialogAsync(nameof(APInterCustomerWelcomeDialog), apInterBotState, cancellationToken);
                        }
                    }
                }
            }
        }
        #endregion

        #region Validators
        private async Task<bool> TextPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (promptContext.Recognized.Value == "0")
            {
                _apInterCustomer.CustomerId = "0";
                return await Task.FromResult(true);
            }
            else if (promptContext.Recognized.Value == "*")
            {
                _apInterCustomer.CustomerId = "*";
                return await Task.FromResult(true);
            }
            else if (promptContext.Recognized.Value.ToString().Length == 6)
            {
                APIResultMessage<APInterCustomer> resultMessage = await _apInterService.GetCustomer(promptContext.Recognized.Value.ToString(), _apInterBotState);
                if (resultMessage.Success)
                {
                    _apInterCustomer = resultMessage.Entity;
                    return await Task.FromResult(true);
                }
                else
                {
                    await promptContext.Context.SendActivityAsync("Número de cliente incorrecto.");
                    if (promptContext.AttemptCount >= 3)
                    {
                        await promptContext.Context.SendActivityAsync("Gracias por comunicarse con nosotros.");
                        _apInterCustomer.CustomerId = "-1";
                        return await Task.FromResult(true);
                    }
                    return await Task.FromResult(false);
                }
            }
            else if (promptContext.AttemptCount >= 3)
            {
                await promptContext.Context.SendActivityAsync("⛔ Ingreso incorrecto.");
                await promptContext.Context.SendActivityAsync("Gracias por comunicarse con nosotros.");
                _apInterCustomer.CustomerId = "-1";
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);

        }
        #endregion

        #region Private methods
        public async Task GetWelcomeMessage(WaterfallStepContext stepContext, APInterBotState aPInterBotState)
        {
            CustomMessage customMessage = await _apInterService.GetTemporaryCustomMessage(aPInterBotState.APIUrl, 3);
            if (customMessage != null && customMessage.Enabled)
                await stepContext.Context.SendActivityAsync(customMessage.Body);
        }
        #endregion

    }
    #endregion
}
