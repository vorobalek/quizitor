namespace Quizitor.Redis.Contracts;

public record RatingShortLineDto(
    long? UserId,
    string? UserFullName,
    int? TeamId,
    string? TeamName,
    int TotalScore,
    int TotalTime);