using Discord.Interactions;

namespace DiscordMusicBot.Bot.Commands.General;

public class PingCommand : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("ping", "Verifica se o bot está funcionando.")]
	public async Task PingAsync()
    {
        await RespondAsync("Pong!");
    }
}
