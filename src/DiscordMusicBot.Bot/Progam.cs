using Microsoft.Extensions.Configuration;
using DiscordMusicBot.Infrastructure.Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;
using DiscordMusicBot.Bot.Commands.General;

namespace DiscordMusicBot.Bot;
internal class Program
{
    static async Task Main(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        string? token = config["Discord:Token"];

        if(string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Token Discord não encontrado.");
        }

        var discordBotClient = new DiscordBotClient();
        var interactionService = new InteractionService(discordBotClient.Client);

        await interactionService.AddModulesAsync(typeof(PingCommand).Assembly, null);
        discordBotClient.Client.InteractionCreated += async iteraction =>
        {
            var context = new SocketInteractionContext(discordBotClient.Client, iteraction);
            await interactionService.ExecuteCommandAsync(context, null);
        };

        await discordBotClient.ConnectAsync(token);

        await Task.Delay(Timeout.Infinite);
    }

}