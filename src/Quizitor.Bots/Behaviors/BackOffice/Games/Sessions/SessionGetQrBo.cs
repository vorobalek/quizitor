using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Sessions.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Configuration;
using Quizitor.Bots.Services.Crypto;
using Quizitor.Bots.Services.Qr;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Sessions;

using Context = ICallbackQueryDataPrefixContext<ISessionViewBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class SessionGetQrBo(
    IEncryptionService encryptionService,
    IQrService qrService,
    IDbContextProvider dbContextProvider) :
    SessionViewBo(dbContextProvider)
{
    /// <summary>
    ///     <b>sessionjoin</b>.{sessionId}
    /// </summary>
    public const string QrCommand = "sessionjoin";

    /// <summary>
    ///     <b>sessiongetqr</b>.{sessionId}.{gamePageNumber}.{sessionPageNumber}
    /// </summary>
    public new const string Button = "sessiongetqr";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions => [UserPermission.BackOfficeSessionView];

    protected override string ButtonInternal => Button;

    public override async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        var data = $"{QrCommand}.{context.Base.Session.Id}";
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
                    TR.L + "_BACKOFFICE_SESSION_GET_QR_ERROR_TXT",
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
                    TR.L + "_BACKOFFICE_SESSION_QR_TEXT_TXT",
                    context.Base.Game.Title,
                    context.Base.Session.Name)
                .Truncate(37));

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