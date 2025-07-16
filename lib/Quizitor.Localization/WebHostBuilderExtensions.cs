using LPlus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Quizitor.Localization;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder AddLocalization(this IWebHostBuilder builder, string locale = "en") =>
        builder
            .ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                var localizationFiles = Directory
                    .EnumerateFiles(
                        Path.Combine(context.HostingEnvironment.ContentRootPath, "Localization"),
                        "*.json",
                        SearchOption.TopDirectoryOnly);

                foreach (var localizationFile in localizationFiles)
                    configurationBuilder
                        .AddJsonFile(
                            localizationFile,
                            true,
                            true);
            })
            .ConfigureServices((context, _) =>
            {
                TR.Configure(options =>
                {
                    options.DetermineLanguageCodeDelegate = () => locale;
                    options.BuildTranslationKeyDelegate = (languageCode, text) => $"Localization:{languageCode}:{text}";
                    options.TryGetTranslationDelegate =
                        translationKey => context.Configuration.GetValue<string>(translationKey);
#if DEBUG
                    var path = Path.Combine(
                        context.HostingEnvironment.ContentRootPath,
                        "missing-translation-keys.txt");
                    options.MissingTranslationKeyOutputDelegate = translationKey =>
                    {
                        File.AppendAllLines(path, [translationKey]);
                    };
#endif
                });
            });
}