using Quizitor.Bots.Configuration;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using SkiaSharp;
using Telegram.Bot.Types;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.SkiaSharp;

namespace Quizitor.Bots.Services.Qr;

internal sealed class QrService : IQrService
{
    public async Task<string?> TryExtractFromUpdateAndSaveFileAsync(
        ITelegramBotClientWrapper client,
        int? botId,
        Update update,
        CancellationToken cancellationToken)
    {
        if (update is not
            {
                Message:
                {
                    Id: var messageId,
                    Photo: { Length: > 0 } photo,
                    From.Id: var fromId
                }
            } || photo.LastOrDefault()?.FileId is not { } fileId)
            return null;

        var rawDirectory = Path.Combine(
            AppConfiguration.WorkingDirectory,
            "files",
            "cached");
        if (!Directory.Exists(rawDirectory))
        {
            Directory.CreateDirectory(rawDirectory);
        }

        var raw = Path.Combine(rawDirectory, fileId);
        if (!File.Exists(raw))
        {
            await using var fileStream = File.Create(raw);
            var fileInfo = await client.GetFile(fileId, cancellationToken);
            if (fileInfo is not { FilePath: { } filePath }) return null;

            await client.DownloadFile(filePath, fileStream, cancellationToken);
        }

        var fileDirectory = Path.Combine(
            AppConfiguration.WorkingDirectory,
            "files",
            "decoded",
            fromId.ToString(),
            botId?.ToString() ?? "backoffice");
        if (!Directory.Exists(fileDirectory))
        {
            Directory.CreateDirectory(fileDirectory);
        }

        using var image = SKImage.FromEncodedData(raw);
        if (image is null) return null;

        using var bitmap = SKBitmap.FromImage(image);
        if (bitmap is null) return null;

        var file = Path.Combine(fileDirectory, $"{messageId}.png");
        if (!File.Exists(file))
        {
            await using var fileStream = File.Create(file);
            bitmap.Encode(fileStream, SKEncodedImageFormat.Png, 100);
        }

        var source = new SKBitmapLuminanceSource(bitmap);
        var binarizer = new HybridBinarizer(source);
        var binaryBitmap = new BinaryBitmap(binarizer);
        var decoder = new QRCodeReader();
        var result = decoder.decode(binaryBitmap);

        var text = result?.Text;
        return text;
    }

    public async Task<string> GenerateFromStringIfNeededAndSaveFileAsync(
        string data,
        string fileName,
        CancellationToken cancellationToken,
        bool forceRegenerate = false,
        string? textUp = null,
        string? textDown = null)
    {
        var directory = Path.Combine(
            AppConfiguration.WorkingDirectory,
            "files",
            "generated",
            "qr");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var file = Path.Combine(directory, $"{fileName}.png");

        if (forceRegenerate && File.Exists(file))
        {
            File.Delete(file);
        }

        if (File.Exists(file)) return file;

        await using var fileStream = File.Create(file);
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = 1024,
                Width = 1024,
                Margin = 4
            }
        };
        using var bitmap = writer.Write(data);

        if (!string.IsNullOrWhiteSpace(textUp) || !string.IsNullOrWhiteSpace(textDown))
        {
            using var canvas = new SKCanvas(bitmap);
            var fontPath = Path.Combine(Environment.CurrentDirectory, "Fonts", "JetBrainsMono-Light.ttf");
            using var typeface = File.Exists(fontPath)
                ? SKTypeface.FromFile(fontPath)
                : SKTypeface.Default;
            using var font = new SKFont(typeface);
            font.Size = 36f;
            using var paint = new SKPaint();
            paint.Color = SKColors.Black;
            if (!string.IsNullOrWhiteSpace(textUp))
            {
                canvas.DrawText(
                    textUp,
                    114f,
                    80f,
                    font,
                    paint);
            }

            if (!string.IsNullOrWhiteSpace(textDown))
            {
                canvas.DrawText(
                    textDown,
                    114f,
                    980f,
                    font,
                    paint);
            }
        }

        bitmap.Encode(fileStream, SKEncodedImageFormat.Png, 100);
        return file;
    }
}