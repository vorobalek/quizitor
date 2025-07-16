namespace Quizitor.Redis.Contracts;

public record RatingFullLineDto(
    long? UserId,
    string? UserFullName,
    int? TeamId,
    string? TeamName,
    Dictionary<int, Dictionary<int, int>> ScorePerRound,
    Dictionary<int, Dictionary<int, int>> TimePerRound)
{
    public virtual bool Equals(RatingFullLineDto? other)
    {
        if (other is null) return false;

        if (UserId != other.UserId) return false;
        if (UserFullName != other.UserFullName) return false;

        if (TeamId != other.TeamId) return false;
        if (TeamName != other.TeamName) return false;

        if (!CompareOuterDictionaries(ScorePerRound, other.ScorePerRound))
            return false;

        if (!CompareOuterDictionaries(TimePerRound, other.TimePerRound))
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(UserId);
        hash.Add(UserFullName);
        hash.Add(TeamId);
        hash.Add(TeamName);
        AddDictionaryToHash(ScorePerRound, ref hash);
        AddDictionaryToHash(TimePerRound, ref hash);
        return hash.ToHashCode();
    }

    private static bool CompareOuterDictionaries(
        Dictionary<int, Dictionary<int, int>>? left,
        Dictionary<int, Dictionary<int, int>>? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        if (left.Count != right.Count) return false;

        foreach (var (key, leftSubDict) in left)
        {
            if (!right.TryGetValue(key, out var rightSubDict))
                return false;

            if (!CompareInnerDictionaries(leftSubDict, rightSubDict))
                return false;
        }

        return true;
    }

    private static bool CompareInnerDictionaries(
        Dictionary<int, int> left,
        Dictionary<int, int> right)
    {
        if (left.Count != right.Count) return false;
        foreach (var (key, leftValue) in left)
        {
            if (!right.TryGetValue(key, out var rightValue))
                return false;

            if (leftValue != rightValue)
                return false;
        }

        return true;
    }

    private static void AddDictionaryToHash(
        Dictionary<int, Dictionary<int, int>>? dict,
        ref HashCode hash)
    {
        if (dict == null)
        {
            hash.Add(0);
            return;
        }

        foreach (var outerKey in dict.Keys.OrderBy(k => k))
        {
            hash.Add(outerKey);
            var innerDict = dict[outerKey];
            AddInnerDictionaryToHash(innerDict, ref hash);
        }
    }

    private static void AddInnerDictionaryToHash(
        Dictionary<int, int>? innerDict,
        ref HashCode hash)
    {
        if (innerDict == null)
        {
            hash.Add(0);
            return;
        }

        foreach (var key in innerDict.Keys.OrderBy(k => k))
        {
            hash.Add(key);
            hash.Add(innerDict[key]);
        }
    }
}