using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Clients;

namespace DiscordMusicBot.Bot.Commands.Music;

public class SkipCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAudioService _audioService;

    public SkipCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("skip", "Pule para a próxima música", runMode: Discord.Interactions.RunMode.Async)]
    public async Task SkipAsync()
    {
        var guildUser = Context.Guild.GetUser(Context.User.Id);
        var voiceChannel = guildUser?.VoiceChannel;

        if (voiceChannel is null)
        {
            await RespondAsync("Você precisa estar conectado a um canal de voz para pular a música.", ephemeral: true);
            return;
        }

        var botVoiceChannel = Context.Guild.CurrentUser.VoiceChannel; 

        if (botVoiceChannel is not null)
        {
            if (botVoiceChannel.Id != voiceChannel.Id)
            {
                await RespondAsync("Estou em outra call no momento", ephemeral: true);
                return;
            }
        }
        else
        {
            await RespondAsync("Não estou em um canal de voz", ephemeral: true);
            return;
        }

        await DeferAsync(ephemeral: true); // Indica que a resposta será enviada posteriormente, permitindo que o bot processe a solicitação sem bloquear a interação.

        try
        {
            var retrieveOptions = new PlayerRetrieveOptions(
                    ChannelBehavior: PlayerChannelBehavior.None,
                    VoiceStateBehavior: MemberVoiceStateBehavior.RequireSame); // Define as opções de recuperação do player, especificando que o bot não deve entrar em nenhum canal de voz ao recuperar o player.
            var result = await _audioService.Players.RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions); // Tenta recuperar ou criar um player de áudio para o contexto atual, usando a fábrica de players "Queued" e as opções de recuperação definidas

            if (!result.IsSuccess)
            {
                await FollowupAsync($"Não consegui pular a música", ephemeral: true);
                return;
            }

            if (result.Player.CurrentTrack is null)
            {
                await FollowupAsync("Não há nenhuma música tocando no momento.", ephemeral: true);
                return;
            }

            await result.Player.SkipAsync(); // Pula para a próxima música na fila.

            await FollowupAsync($"Música pulada com sucesso!", ephemeral: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao pular a música: {ex.Message}");
            await FollowupAsync("Não consegui pular a música.", ephemeral: true);
        }
    }
}