using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text.NumberWithUnit;
using static Microsoft.Recognizers.Text.Culture;

namespace Cahuil.Bot.Apinter.Prompt
{
    public class NumberPromptExtend<T> : Prompt<T> where T : struct
    {
        public string DefaultLocale { get; set; }
        public NumberPromptExtend(string dialogId, PromptValidator<T> validator = null, string defaultLocale = null)
            : base(dialogId, validator)
        {
            DefaultLocale = defaultLocale;
            // Check wheter the number type is supported when the prompt is created.
            var type = typeof(T);
            if (!(type == typeof(float)
                  || type == typeof(int)
                  || type == typeof(long)
                  || type == typeof(double)
                  || type == typeof(decimal)))
            {
                throw new NotSupportedException($"NumberPrompt: type argument T of type 'typeof(T)' is not supported");
            }
        }

        protected override async Task OnPromptAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, bool isRetry, CancellationToken cancellationToken = default(CancellationToken))
        {
            PromptOptionsExtension promptOptionsExtension = options as PromptOptionsExtension;
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (isRetry && options.RetryPrompt != null)
            {
                await turnContext.SendActivityAsync(options.RetryPrompt, cancellationToken).ConfigureAwait(false);
            }
            else if (options.Prompt != null)
            {
                Activity activity = new Activity();
                activity.Attachments = new List<Attachment>() { new Attachment(null, promptOptionsExtension.MediaUrl) };
                activity.Text = options.Prompt.Text;
                activity.Type = "message";
                await turnContext.SendActivityAsync(activity, cancellationToken).ConfigureAwait(false);
            }
        }

        protected override Task<PromptRecognizerResult<T>> OnRecognizeAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            var result = new PromptRecognizerResult<T>();
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var utterance = turnContext.Activity.AsMessageActivity().Text;
                if (string.IsNullOrEmpty(utterance))
                {
                    return Task.FromResult(result);
                }

                var culture = turnContext.Activity.Locale ?? DefaultLocale ?? English;
                var results = RecognizeNumberWithUnit(utterance, culture);
                if (results.Count > 0)
                {
                    // Try to parse value based on type
                    string text = string.Empty;

                    // Try to parse value based on type
                    var valueResolution = results[0].Resolution["value"];
                    if (valueResolution != null)
                    {
                        text = valueResolution.ToString();
                    }

                    if (typeof(T) == typeof(float))
                    {
                        if (float.TryParse(text, NumberStyles.Any, new CultureInfo(culture), out var value))
                        {
                            result.Succeeded = true;
                            result.Value = (T)(object)value;
                        }
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        if (int.TryParse(text, NumberStyles.Any, new CultureInfo(culture), out var value))
                        {
                            result.Succeeded = true;
                            result.Value = (T)(object)value;
                        }
                    }
                    else if (typeof(T) == typeof(long))
                    {
                        if (long.TryParse(text, NumberStyles.Any, new CultureInfo(culture), out var value))
                        {
                            result.Succeeded = true;
                            result.Value = (T)(object)value;
                        }
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        if (double.TryParse(text, NumberStyles.Any, new CultureInfo(culture), out var value))
                        {
                            result.Succeeded = true;
                            result.Value = (T)(object)value;
                        }
                    }
                    else if (typeof(T) == typeof(decimal))
                    {
                        if (decimal.TryParse(text, NumberStyles.Any, new CultureInfo(culture), out var value))
                        {
                            result.Succeeded = true;
                            result.Value = (T)(object)value;
                        }
                    }
                }
            }

            return Task.FromResult(result);
        }

        private static List<ModelResult> RecognizeNumberWithUnit(string utterance, string culture)
        {
            var number = NumberRecognizer.RecognizeNumber(utterance, culture);

            if (number.Any())
            {
                // Result when it matches with a number recognizer
                return number;
            }
            else
            {
                // Analyze every option for numberWithUnit
                var results = new List<List<ModelResult>>();
                results.Add(NumberWithUnitRecognizer.RecognizeCurrency(utterance, culture));
                results.Add(NumberWithUnitRecognizer.RecognizeAge(utterance, culture));
                results.Add(NumberWithUnitRecognizer.RecognizeTemperature(utterance, culture));
                results.Add(NumberWithUnitRecognizer.RecognizeDimension(utterance, culture));

                // Filter the options that returned nothing and return the one that matched
                return results.FirstOrDefault(r => r.Any()) ?? new List<ModelResult>();
            }
        }
    }
}
