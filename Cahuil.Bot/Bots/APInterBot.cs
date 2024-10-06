using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cahuil.Bot.Bots
{
    public class APInterBot<T> : DialogBot<T> where T : Dialog
    {
        public APInterBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger, IConfiguration configuration)
            : base(conversationState, userState, dialog, logger, configuration)
        {

        }


    }
}
