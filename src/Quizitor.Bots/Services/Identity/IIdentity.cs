using Quizitor.Data.Entities;

namespace Quizitor.Bots.Services.Identity;

internal interface IIdentity
{
    User User { get; }
    bool HasFullAccess { get; }
    UserPrompt? Prompt { get; }
    string[] Permissions { get; }
}