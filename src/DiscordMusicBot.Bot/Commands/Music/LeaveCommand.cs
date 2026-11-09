using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Clients;

namespace DiscordMusicBot.Bot.Commands.Music;

public class LeaveCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAudioService _audioService;

    public LeaveCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("leave", "Desconecta o Sonora do canal de voz.", runMode: Discord.Interactions.RunMode.Async)]

    public async Task LeaveAsync()
    {
        var guildUser = Context.Guild.GetUser(Context.User.Id);
        var voiceChannel = guildUser?.VoiceChannel;

        if(voiceChannel is null)
        {
            await RespondAsync("Você precisa estar conectado a um canal de voz para que eu possa sair.", ephemeral: true);
            return;
        }

        await DeferAsync(ephemeral: true); // Indica que a resposta será enviada posteriormente, permitindo que o bot processe a solicitação sem bloquear a interação.

        try
        {
            var retrieveOptions = new PlayerRetrieveOptions(
                    ChannelBehavior: PlayerChannelBehavior.None,
                    VoiceStateBehavior: MemberVoiceStateBehavior.RequireSame); // Define as opções de recuperação do player, especificando que o bot não deve entrar em nenhum canal de voz ao recuperar o player.
            var result = await _audioService.Players.RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions); // Tenta recuperar ou criar um player de áudio para o contexto atual, usando a fábrica de players "Queued" e as opções de recuperação definidas.

            if(!result.IsSuccess)
            {
                await FollowupAsync("O Sonora não está conectado a um canal de voz.", ephemeral: true);
                return;
            }

            await result.Player.DisconnectAsync(); // Desconecta o player de áudio do canal de voz.

            await FollowupAsync("Desconectei do canal de voz!", ephemeral: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao desconectar do áudio: {ex.Message}");
            await FollowupAsync("Não consegui desconectar do canal de voz.", ephemeral: true);
        }
    }
}