using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands.Module
{
    public abstract class BaseModule : InteractionModuleBase<SocketInteractionContext>
    {
        protected Embed BuildErrorMessage(string message)
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Une erreur est survenue.")
                .WithDescription(message);

            builder.WithColor(Color.Red);
            return builder.Build();
        }
        protected Embed GetInProgressMessage()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Traitement en cours..");


            builder.WithColor(Color.Blue);
            return builder.Build();
        }
    }
}
