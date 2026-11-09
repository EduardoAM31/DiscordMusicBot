using Microsoft.Extensions.Configuration;
using DiscordMusicBot.Infrastructure.Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;
using DiscordMusicBot.Bot.Commands.General;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Bot;
internal class Program
{
    static async Task Main(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        string? token = config["Discord:Token"];
        string? guildIdValue = config["Discord:GuildId"];
        string? lavalinkPassword = config["Lavalink:Password"];

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Token Discord não encontrado.");
        }
        if (!ulong.TryParse(guildIdValue, out ulong guildId))
        {
            throw new InvalidOperationException("ID do servidor Discord inválido ou não encontrado.");
        }
        if (string.IsNullOrWhiteSpace(lavalinkPassword))
        {
            throw new InvalidOperationException("Senha do Lavalink não encontrada.");
        }

        var discordBotClient = new DiscordBotClient(); // Cria uma instância do cliente do bot Discord, que gerencia a conexão e eventos do bot
        var builder = Host.CreateApplicationBuilder(args); // Cria um construtor de host para configurar serviços e dependências

        builder.Services.AddSingleton(discordBotClient.Client); // Adiciona o cliente Discord como um serviço singleton, permitindo que seja injetado em outros serviços
        builder.Services.AddLavalink(); // Adiciona suporte ao Lavalink, que é usado para streaming de áudio no Discord
        builder.Services.ConfigureLavalink(options => 
        {
            options.BaseAddress = new Uri("http://127.0.0.1:2333");
            options.Passphrase = lavalinkPassword;
        }); // Configura as opções do Lavalink, incluindo o endereço base e a senha para autenticação

        using IHost host = builder.Build(); // Cria o host, que gerencia o ciclo de vida da aplicação e os serviços configurados

        await host.StartAsync(); // Inicia o host, que inicia todos os serviços configurados e prepara a aplicação para execução

        var interactionService = new InteractionService(discordBotClient.Client); // Cria uma instância do serviço de interação, que gerencia os comandos de barra (slash commands) e outras interações do Discord

        await interactionService.AddModulesAsync(typeof(PingCommand).Assembly, host.Services);

        discordBotClient.Client.InteractionCreated += async iteraction =>
        {
            var context = new SocketInteractionContext(discordBotClient.Client, iteraction);
            await interactionService.ExecuteCommandAsync(context, host.Services);
        };
        discordBotClient.Client.Ready += async () =>
        {
            await interactionService.RegisterCommandsToGuildAsync(guildId);
            Console.WriteLine("Comandos registrados no servidor");
        };

        await discordBotClient.ConnectAsync(token);
        await Task.Delay(Timeout.Infinite);
    }

}