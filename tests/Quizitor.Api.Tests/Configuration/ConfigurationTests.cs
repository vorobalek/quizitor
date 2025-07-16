using Quizitor.Api.Configuration;
using Quizitor.Tests;

namespace Quizitor.Api.Tests.Configuration;

[TestClass]
public class ConfigurationTests
{
    [TestMethod]
    public void Configuration_CheckAllProperties()
    {
        var locale = Unique.String();
        Environment.SetEnvironmentVariable("LOCALE", locale);

        var port = $"{Math.Abs(Unique.Int32()) % (1 << 16)}";
        Environment.SetEnvironmentVariable("PORT", port);

        var pathBase = Unique.String();
        Environment.SetEnvironmentVariable("PATH_BASE", pathBase);

        var dbConnectionString = Unique.String();
        Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", dbConnectionString);

        var sentryDsn = Unique.String();
        Environment.SetEnvironmentVariable("SENTRY_DSN", sentryDsn);


        Assert.AreEqual(locale, AppConfiguration.Locale);
        Assert.AreEqual(pathBase, AppConfiguration.PathBase);
        Assert.AreEqual(port, AppConfiguration.Port);
        Assert.AreEqual(dbConnectionString, AppConfiguration.DbConnectionString);
        Assert.AreEqual(sentryDsn, SentryConfiguration.Dsn);
    }
}