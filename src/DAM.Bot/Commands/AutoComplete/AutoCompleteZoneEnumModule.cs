using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands.AutoComplete
{
    public class AutoCompleteZoneEnumModule : AutocompleteHandler
    {
        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            var userInput = ((context.Interaction as SocketAutocompleteInteraction)?.Data?.Current?.Value?.ToString() ?? string.Empty).ToLowerInvariant();

            var values = GetEnumValues();

            var filteredValues = values.Where(e => e.Name.ToLowerInvariant().Contains(userInput)).ToList();

          

            // max - 25 suggestions at a time (API limit)
            return AutocompletionResult.FromSuccess(filteredValues.Take(25));

        }
        private IEnumerable<AutocompleteResult> GetEnumValues()
        {
            var results = new List<AutocompleteResult>();

            var enumType = typeof(ZoneAvA);
            var fields = enumType.GetFields().Where(f => f.FieldType.Name == enumType.Name);
            foreach (var memberInfo in fields)
            {
                var value = (int)(memberInfo.GetValue(null));
                var valueAttributes =
                memberInfo.GetCustomAttributes(typeof(ChoiceDisplayAttribute), false);

                var description = valueAttributes.Length > 0 ? ((ChoiceDisplayAttribute)valueAttributes[0]).Name : memberInfo.Name;
                results.Add(new AutocompleteResult(description, value.ToString()));
            }
            return results;



        }
    }
}
