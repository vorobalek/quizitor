using LPlus;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Migrator.Seeds;

// ReSharper disable once ClassNeverInstantiated.Global
public class BotCommandSeed(
    IDbContextProvider dbContextProvider) : ISeed
{
    public async Task ApplyAsync(CancellationToken cancellationToken)
    {
        Dictionary<BotType, Dictionary<string, string>> botTypeCommandsMap = new()
        {
            {
                BotType.Universal, []
            },
            {
                BotType.LoadBalancer, new Dictionary<string, string>
                {
                    {
                        "start", TR.L + "_BOT_COMMAND_LOAD_BALANCER_START"
                    }
                }
            },
            {
                BotType.GameAdmin, new Dictionary<string, string>
                {
                    {
                        "start", TR.L + "_BOT_COMMAND_GAME_ADMIN_START"
                    }
                }
            },
            {
                BotType.GameServer, new Dictionary<string, string>
                {
                    {
                        "start", TR.L + "_BOT_COMMAND_GAME_SERVER_START"
                    }
                }
            },
            {
                BotType.BackOffice, new Dictionary<string, string>
                {
                    {
                        "start", TR.L + "_BOT_COMMAND_BACKOFFICE_START"
                    }
                }
            }
        };

        foreach (var (botType, commands) in botTypeCommandsMap)
            await ApplyBotTypeCommandsAsync(botType, commands, cancellationToken);
    }

    private async Task ApplyBotTypeCommandsAsync(
        BotType botType,
        Dictionary<string, string> commands,
        CancellationToken cancellationToken)
    {
        foreach (var (command, description) in commands)
            await ApplyCommandAsync(botType, command, description, cancellationToken);
    }

    private async Task ApplyCommandAsync(
        BotType botType,
        string command,
        string description,
        CancellationToken cancellationToken)
    {
        var botCommand = await dbContextProvider
            .BotCommands
            .GetByTypeAndCommandAsync(
                botType,
                command,
                cancellationToken);

        if (botCommand is null)
        {
            await dbContextProvider
                .BotCommands
                .AddAsync(new BotCommand
                    {
                        BotType = botType,
                        Command = command,
                        Description = description
                    },
                    cancellationToken);
        }
        else
        {
            botCommand.Description = description;
            await dbContextProvider
                .BotCommands
                .UpdateAsync(
                    botCommand,
                    cancellationToken);
        }
    }
}