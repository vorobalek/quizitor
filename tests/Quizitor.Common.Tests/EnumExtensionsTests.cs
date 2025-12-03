using System.ComponentModel.DataAnnotations;

namespace Quizitor.Common.Tests;

[TestClass]
public sealed class EnumExtensionsTests
{
    [TestMethod]
    public void GetDisplayName_AttributeWithNameValue_ReturnsDisplayName()
    {
        const TestEnum testEnum = TestEnum.ValueWithName;


        var displayName = testEnum.DisplayName;


        Assert.AreEqual("TestEnumValue", displayName);
    }

    [TestMethod]
    public void GetDisplayName_AttributeWithNullName_ReturnsNull()
    {
        const TestEnum testEnum = TestEnum.ValueWithNullName;


        var displayName = testEnum.DisplayName;


        Assert.IsNull(displayName);
    }

    [TestMethod]
    public void GetDisplayName_AttributeDoesNotExist_ReturnsDefault()
    {
        const TestEnum testEnum = TestEnum.ValueWithoutName;


        var displayName = testEnum.DisplayName;


        Assert.AreEqual("ValueWithoutName", displayName);
    }

    private enum TestEnum
    {
        [Display(Name = "TestEnumValue")]
        ValueWithName = 1,

        [Display(Name = null)]
        ValueWithNullName = 2,

        ValueWithoutName = 3
    }
}