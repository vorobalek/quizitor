using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Bots.Configuration;
using Quizitor.Bots.Services.Crypto;
using Quizitor.Bots.Services.Qr;
using Quizitor.Common;
using Quizitor.Data;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Teams;

using Context = ICallbackQueryDataEqualsContext<IGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class TeamQrGs(
    IEncryptionService encryptionService,
    IQrService qrService,
    IDbContextProvider dbContextProvider) : TeamGs(dbContextProvider)
{
    public new const string Button = "teamqr";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInternal => Button;

    public override async Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (context.Base.SessionTeamInfo is not { } teamInfo)
        {
            await base.PerformCallbackQueryDataEqualsAsync(context, cancellationToken);
            return;
        }

        var data = $"{TeamSessionJoinGs.QrCommand}.{teamInfo.Team.Id}.{context.Base.Session.Id}";
        var expiredAt = 0L;
        if (long.TryParse(AppConfiguration.QrCodeExpirationSeconds, out var expirationSeconds) &&
            expirationSeconds > 0)
        {
            var serverTime = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
            expiredAt = serverTime.AddSeconds(expirationSeconds).ToUnixTimeSeconds();
        }

        data += $".{expiredAt}";

        var encryptedData = encryptionService.TryEncryptString(data);
        if (encryptedData is null)
        {
            await context
                .Base
                .Client
                .AnswerCallbackQuery(
                    context.Base.UpdateContext,
                    context.CallbackQueryId,
                    TR.L + "_GAME_SERVER_TEAM_GET_QR_ERROR_TXT",
                    true,
                    cancellationToken: cancellationToken);
            return;
        }

        var file = await qrService.GenerateFromStringIfNeededAndSaveFileAsync(
            encryptedData,
            data,
            cancellationToken,
            true,
            string.Format(
                    TR.L + "_GAME_SERVER_SESSION_QR_TEXT_TXT",
                    context.Base.Game.Title,
                    context.Base.Session.Name)
                .Truncate(37),
            teamInfo.Team.Name.Truncate(37));

        await context
            .Base
            .Client
            .SendChatAction(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                ChatAction.UploadPhoto,
                cancellationToken: cancellationToken);

        await context
            .Base
            .Client
            .SendPhoto(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                file,
                cancellationToken: cancellationToken);
    }
}