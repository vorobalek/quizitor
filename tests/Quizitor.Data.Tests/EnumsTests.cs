using Quizitor.Common;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Tests;

[TestClass]
public sealed class EnumsTests
{
    [TestMethod]
    public void BotTypeShouldHaveDisplayNames()
    {
        var values = Enum.GetValues<BotType>();


        foreach (var value in values)
        {
            var result = value.DisplayName;


            Assert.IsNotNull(result, $"[Display(Name = \"...\") for {nameof(BotType)}.{value} should not be null");
            Assert.AreNotEqual(value.ToString(), result, $"[Display(Name = \"...\") for {nameof(BotType)}.{value} should be set");
        }
    }
}