using Quizitor.Tests;

namespace Quizitor.Common.Tests;

[TestClass]
public sealed class StringExtensionsTests
{
    [TestMethod]
    // base cases
    [DataRow("&", "&amp;")]
    [DataRow("<", "&lt;")]
    [DataRow(">", "&gt;")]
    [DataRow("\"", "&quot;")]
    // complex cases
    [DataRow(
        "&amp;&amp;",
        "&amp;amp;&amp;amp;")]
    [DataRow(
        "genius \"html\" <b>injection</b>",
        "genius &quot;html&quot; &lt;b&gt;injection&lt;/b&gt;")]
    public void EscapeHtml_Correctly(
        string input,
        string expected)
    {
        var result = input.Html;


        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Truncate_InputLengthEqualsMaxLength_ReturnsFull()
    {
        var input = Unique.String();


        var result = input.Truncate(input.Length);


        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public void Truncate_InputLengthLessThanMaxLength_ReturnsFull()
    {
        var input = Unique.String();


        var result = input.Truncate(input.Length + 1);


        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public void Truncate_InputLengthGreaterThanMaxLength_ReturnsTruncated()
    {
        const string input = "050f9dc3bc1445158e3a2985b847eac8";
        const string expected = "050f9dc3bc1445158e3a2985b847ea…";


        var result = input.Truncate(input.Length - 1);


        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Truncate_InputLengthGreaterThanMaxLengthWithCustomSuffix_ReturnsTruncated()
    {
        const string input = "050f9dc3bc1445158e3a2985b847eac8";
        const string expected = "050f9dc3bc1445158e3a2985b8(...)";


        var result = input.Truncate(input.Length - 1, "(...)");


        Assert.AreEqual(expected, result);
    }
}