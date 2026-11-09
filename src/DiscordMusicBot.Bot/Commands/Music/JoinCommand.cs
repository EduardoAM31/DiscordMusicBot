using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;

namespace DiscordMusicBot.Bot.Commands.Music;

public class JoinCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IAudioService _audioService;
    
    public JoinCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("join", "Conecta o Sonora ao seu canal de voz.", runMode: Discord.Interactions.RunMode.Async)]
    public async Task JoinAsync()
    {
        var guildUser = Context.Guild.GetUser(Context.User.Id);
        var voiceChannel = guildUser?.VoiceChannel;

        if (voiceChannel is null)
        {
            await RespondAsync("Você precisa estar em um canal de voz para que eu possa entrar.");
            return;
        }

        var botVoiceChannel = Context.Guild.CurrentUser.VoiceChannel; 

        if (botVoiceChannel is not null)
        {
            if (botVoiceChannel.Id == voiceChannel.Id)
            {
                await RespondAsync("Já estou na call", ephemeral: true);
                return;
            }

            await RespondAsync("Estou em outra call no momento", ephemeral: true);
            return;
        }

        await DeferAsync(ephemeral: true);

        try
        {
            var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: PlayerChannelBehavior.Join); // Define as opções de recuperação do player, especificando que o bot deve entrar no canal de voz do usuário
            var result = await _audioService.Players.RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions); // Tenta recuperar ou criar um player de áudio para o contexto atual, usando a fábrica de players "Queued" e as opções de recuperação definidas

            if (!result.IsSuccess)
            {
                await FollowupAsync($"Não consegui conectar ao canal de voz. Status: {result.Status}", ephemeral: true);
                return;
            }
            await FollowupAsync($"Entrei no canal de voz **{voiceChannel.Name}**!", ephemeral: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao conectar ao áudio: {ex.Message}");
            await FollowupAsync("Não consegui estabelecer a conexão de áudio.", ephemeral: true);
        }
    }
}
