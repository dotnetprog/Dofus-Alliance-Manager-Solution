using Discord;

namespace DAM.Core.Abstractions.Services
{
    public interface IDiscordBotService
    {
        Task<IGuildChannel> GetChannel(ulong guildid, ulong channelid);
        Task<IGuildUser> GetCurrentGuildUser(ulong guildid);
        Task<IGuild> GetGuild(ulong guildid);
        Task<IReadOnlyCollection<IGuildChannel?>> GetChannels(ulong id);
        Task<IReadOnlyCollection<IRole?>> GetRoles(ulong guildid);
        Task<IReadOnlyCollection<IGuildChannel?>> GetChannels(ulong id, params ChannelType[] types);
        Task<IReadOnlyCollection<IGuild?>> GetGuilds();

        Task<IGuildUser> GetGuildUser(ulong guildid, ulong id);

        Task<IUserMessage> SendMessage(ITextChannel channel, string content = null, Embed embed = null);
        Task React(ITextChannel channel, ulong MessageId, IEmote emote);

        Task LeaveServer(ulong GuildId);
    }
}
