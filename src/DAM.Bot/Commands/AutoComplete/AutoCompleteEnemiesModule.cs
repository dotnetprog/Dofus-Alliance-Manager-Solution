using Alachisoft.NCache.Caching;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands.AutoComplete
{
    public class AutoCompleteEnemiesModule : AutocompleteHandler
    {
        protected readonly IAllianceManagementServiceAsync _allianceManagementService;
        protected readonly ILogger _logger;
        protected readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _defaultDelay;
        public AutoCompleteEnemiesModule(ILogger<AutoCompleteEnemiesModule> logger, IAllianceManagementServiceAsync allianceService, IMemoryCache cacheService)
        {
            _logger = logger;
            _allianceManagementService = allianceService;
            _memoryCache = cacheService;
            _defaultDelay = TimeSpan.FromHours(12);
        }

        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            // Create a collection with suggestions for autocomplete
            var key = $"allianceid{context.Guild.Id}";
            var allianceid = await _memoryCache.GetOrCreate(key, async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = _defaultDelay;
                var alliance = await _allianceManagementService.GetAlliance(context.Guild.Id.ToString());
                return alliance.Id;
            });
            var keyEnemy = $"allianceenemies{context.Guild.Id}";
            var ennemies = await _memoryCache.GetOrCreate(keyEnemy, async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _allianceManagementService.GetEnemies(allianceid);

            });

            var userInput = ((context.Interaction as SocketAutocompleteInteraction)?.Data?.Current?.Value?.ToString() ?? string.Empty).ToLowerInvariant();
            var filteredEnemies = ennemies.Where(e => e.Name.ToLowerInvariant().Contains(userInput)).ToList();

            var results = filteredEnemies.Select(e => new AutocompleteResult(e.Name, e.Id.ToString())).ToArray();

            // max - 25 suggestions at a time (API limit)
            return AutocompletionResult.FromSuccess(results.Take(25));
        }
    }
}
