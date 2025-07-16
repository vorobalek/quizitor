namespace Quizitor.Bots.Helpers;

public static class PaginationHelper
{
    public delegate string GetLineDelegate<in T>(T item, out bool isSpecial);

    public delegate string GetPageDelegate(ICollection<string> lines);

    public static IEnumerable<string> CreatePages<T>(
        this T[] items,
        GetLineDelegate<T> getLineDelegate,
        GetPageDelegate getPageDelegate,
        int maxItemsPerPage = 10,
        int maxPageLength = 4096)
    {
        if (items.Length == 0) yield break;

        var specialLines = new SortedSet<int>();
        var allLines = new List<string>();
        for (var i = 0; i < items.Length; ++i)
        {
            var item = items[i];
            var text = getLineDelegate(item, out var isSpecial);
            if (isSpecial) specialLines.Add(i);
            allLines.Add(text);
        }

        var currentPageLines = new List<string>();
        for (var i = 0; i < allLines.Count; ++i)
        {
            if (currentPageLines.Count == 0 && specialLines.Count > 0)
            {
                currentPageLines
                    .AddRange(specialLines
                        .Where(x => x < i)
                        .Select(x => allLines[x]));
            }

            var footerLines = specialLines
                .Where(x => x > i)
                .Select(x => allLines[x])
                .ToArray();

            var pageDraft = getPageDelegate(
            [
                ..currentPageLines,
                allLines[i],
                ..footerLines
            ]);

            if (currentPageLines.Count + footerLines.Length + 1 <= maxItemsPerPage &&
                pageDraft.Length <= maxPageLength)
            {
                currentPageLines.Add(allLines[i]);
                continue;
            }

            footerLines = specialLines
                .Where(x => x > i - 1)
                .Select(x => allLines[x])
                .ToArray();
            var page = getPageDelegate(
            [
                ..currentPageLines,
                ..footerLines
            ]);
            yield return page;
            currentPageLines.Clear();
            --i;
        }

        if (currentPageLines.Count == 0) yield break;
        yield return getPageDelegate(currentPageLines);
    }

    public static string GetPage<T>(
        this T[] items,
        GetLineDelegate<T> getLineFunc,
        GetPageDelegate getPageFunc,
        int pageNumber,
        out int actualPageNumber,
        out int actualPageCount,
        int maxItemsPerPage = 10,
        int maxPageLength = 4096)
    {
        var pages = CreatePages(items, getLineFunc,
                getPageFunc,
                maxItemsPerPage,
                maxPageLength)
            .ToArray();
        actualPageCount = pages.Length;
        actualPageNumber = Math.Min(Math.Max(0, pageNumber), actualPageCount - 1);
        return pages[actualPageNumber];
    }
}