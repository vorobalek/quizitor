using Quizitor.Tests;

namespace Quizitor.Common.Tests;

[TestClass]
public sealed class EnvironmentExtensionsTests
{
    [TestMethod]
    public void GetEnvironmentVariable_VariableExists_ReturnValue()
    {
        var variableName = Unique.String();
        var variableValue = Unique.String();
        Environment.SetEnvironmentVariable(variableName, variableValue);


        var result = variableName.EnvironmentValue;


        Assert.AreEqual(variableValue, result);
    }

    [TestMethod]
    public void GetEnvironmentVariable_VariableNotExists_ReturnNull()
    {
        var variableName = Unique.String();


        var result = variableName.EnvironmentValue;


        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetEnvironmentVariableOrThrowIfNullOrWhiteSpace_VariableExists_ReturnValue()
    {
        var variableName = Unique.String();
        var variableValue = Unique.String();
        Environment.SetEnvironmentVariable(variableName, variableValue);


        var result = variableName.RequiredEnvironmentValue;


        Assert.AreEqual(variableValue, result);
    }

    [TestMethod]
    public void GetEnvironmentVariableOrThrowIfNullOrWhiteSpace_VariableIsEmpty_ThrowException()
    {
        var variableName = Unique.String();
        var variableValue = string.Empty;
        Environment.SetEnvironmentVariable(variableName, variableValue);


        Assert.ThrowsExactly<ArgumentNullException>(
            () => variableName.RequiredEnvironmentValue,
            $"Environment variable '{variableName}' is not defined.");
    }

    [TestMethod]
    public void GetEnvironmentVariableOrThrowIfNullOrWhiteSpace_VariableIsWhiteSpace_ThrowException()
    {
        var variableName = Unique.String();
        const string variableValue = " ";
        Environment.SetEnvironmentVariable(variableName, variableValue);


        Assert.ThrowsExactly<ArgumentNullException>(
            () => variableName.RequiredEnvironmentValue,
            $"Environment variable '{variableName}' is not defined.");
    }

    [TestMethod]
    public void GetEnvironmentVariableOrThrowIfNullOrWhiteSpace_VariableNotExists_ThrowException()
    {
        var variableName = Unique.String();


        Assert.ThrowsExactly<ArgumentNullException>(
            () => variableName.RequiredEnvironmentValue,
            $"Environment variable '{variableName}' is not defined.");
    }

    [TestMethod]
    public void GetEnvironmentVariableWithFallbackValue_StringVariableExists_ReturnValue()
    {
        var variableName = Unique.String();
        var variableValue = Unique.String();
        Environment.SetEnvironmentVariable(variableName, variableValue);
        var fallbackValue = Unique.String();


        var result = variableName.GetEnvironmentValueWithFallback(fallbackValue);


        Assert.AreEqual(variableValue, result);
    }

    [TestMethod]
    public void GetEnvironmentVariableWithFallbackValue_StringVariableIsEmpty_ReturnFallbackValue()
    {
        var variableName = Unique.String();
        var variableValue = string.Empty;
        Environment.SetEnvironmentVariable(variableName, variableValue);
        var fallbackValue = Unique.String();


        var result = variableName.GetEnvironmentValueWithFallback(fallbackValue);


        Assert.AreEqual(fallbackValue, result);
    }

    [TestMethod]
    public void GetEnvironmentVariableWithFallbackValue_StringVariableIsWhiteSpace_ReturnFallbackValue()
    {
        var variableName = Unique.String();
        const string variableValue = " ";
        Environment.SetEnvironmentVariable(variableName, variableValue);
        var fallbackValue = Unique.String();


        var result = variableName.GetEnvironmentValueWithFallback(fallbackValue);


        Assert.AreEqual(fallbackValue, result);
    }

    [TestMethod]
    public void GetEnvironmentVariableWithFallbackValue_StringVariableNotExists_ReturnFallbackValue()
    {
        var variableName = Unique.String();
        var fallbackValue = Unique.String();


        var result = variableName.GetEnvironmentValueWithFallback(fallbackValue);


        Assert.AreEqual(fallbackValue, result);
    }

    [TestMethod]
    public void GetEnvironmentVariableWithFallbackValue_Int32VariableExists_ReturnValue()
    {
        var variableName = Unique.String();
        var variableValue = Unique.Int32();
        Environment.SetEnvironmentVariable(variableName, variableValue.ToString());
        var fallbackValue = Unique.Int32();


        var result = variableName.GetEnvironmentValueWithFallback(fallbackValue);


        Assert.AreEqual(variableValue, result);
    }

    [TestMethod]
    public void GetEnvironmentVariableWithFallbackValue_Int32VariableIsEmpty_ReturnFallbackValue()
    {
        var variableName = Unique.String();
        var variableValue = string.Empty;
        Environment.SetEnvironmentVariable(variableName, variableValue);
        var fallbackValue = Unique.Int32();


        var result = variableName.GetEnvironmentValueWithFallback(fallbackValue);


        Assert.AreEqual(fallbackValue, result);
    }

    [TestMethod]
    public void GetEnvironmentVariableWithFallbackValue_Int32VariableIsWhiteSpace_ReturnFallbackValue()
    {
        var variableName = Unique.String();
        const string variableValue = " ";
        Environment.SetEnvironmentVariable(variableName, variableValue);
        var fallbackValue = Unique.Int32();


        var result = variableName.GetEnvironmentValueWithFallback(fallbackValue);


        Assert.AreEqual(fallbackValue, result);
    }

    [TestMethod]
    public void GetEnvironmentVariableWithFallbackValue_Int32VariableNotExists_ReturnFallbackValue()
    {
        var variableName = Unique.String();
        var fallbackValue = Unique.Int32();


        var result = variableName.GetEnvironmentValueWithFallback(fallbackValue);


        Assert.AreEqual(fallbackValue, result);
    }
}