using Discord;
using Discord.WebSocket;

namespace DiscordMusicBot.Infrastructure.Discord;

public class DiscordBotClient
{
    private readonly DiscordSocketClient _client;// Instância do cliente DiscordSocketClient, responsável por gerenciar a conexão com o Discord e interagir com a API do Discord.
    public DiscordSocketClient Client => _client;// Expõe o cliente DiscordSocketClient para acesso externo, se necessário.
    public DiscordBotClient()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildVoiceStates, // Define os intents do gateway para o bot, permitindo que ele receba eventos relacionados a guildas e estados de voz.
            LogLevel = LogSeverity.Info,
            EnableVoiceDaveEncryption = true
        });

        _client.Log += log =>
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        };
        _client.Ready += OnReadyAsync;
    }

    private Task OnReadyAsync()
    {
        Console.WriteLine($"Bot Conectado: {_client.CurrentUser.Username}");
        return Task.CompletedTask;
    }

    public async Task ConnectAsync(string token)
    {
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    public async Task DisconnectAsync()
    {
        await _client.StopAsync();
        await _client.LogoutAsync();
    }


}
