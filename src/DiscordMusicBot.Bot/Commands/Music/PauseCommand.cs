using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Clients;

namespace DiscordMusicBot.Bot.Commands.Music;

public class PauseCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAudioService _audioService;

    public PauseCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("pause", "Pausa a música que está tocando", runMode: Discord.Interactions.RunMode.Async)]
    public async Task PauseAsync()
    {
        var guildUser = Context.Guild.GetUser(Context.User.Id);
        var voiceChannel = guildUser?.VoiceChannel;

        if(voiceChannel is null)
        {
            await RespondAsync("Você precisa estar conectado a um canal de voz para pausar a música.", ephemeral: true);            return;
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
                await FollowupAsync($"Não consegui pausar a música", ephemeral: true);
                return;
            }

            await result.Player.PauseAsync(); //Pausa o player de áudio

            await FollowupAsync($"Música pausada", ephemeral: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao pausar o áudio: {ex.Message}");
            await FollowupAsync("Não consegui pausar a música.", ephemeral: true);
        }
    }

}