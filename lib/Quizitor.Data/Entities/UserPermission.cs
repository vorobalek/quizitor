using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("user_permission", Schema = "public")]
[PrimaryKey(nameof(Id))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class UserPermission
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public long UserId { get; set; }

    [MaxLength(256)]
    public string SystemName { get; set; } = null!;

    [InverseProperty(nameof(Entities.User.Permissions))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; } = null!;

    #region Constants

    #region BackOffice

    public const string BackOfficeMain = "BackOfficeMain";
    public const string BackOfficeBotList = "BackOfficeBotList";
    public const string BackOfficeBotView = "BackOfficeBotView";
    public const string BackOfficeBotEdit = "BackOfficeBotEdit";
    public const string BackOfficeBotStart = "BackOfficeBotStart";
    public const string BackOfficeBotStop = "BackOfficeBotStop";
    public const string BackOfficeUserList = "BackOfficeUserList";
    public const string BackOfficeUserView = "BackOfficeUserView";
    public const string BackOfficeUserEdit = "BackOfficeUserEdit";
    public const string BackOfficeGameList = "BackOfficeGameList";
    public const string BackOfficeGameCreate = "BackOfficeGameCreate";
    public const string BackOfficeGameDelete = "BackOfficeGameDelete";
    public const string BackOfficeGameView = "BackOfficeGameView";
    public const string BackOfficeRoundList = "BackOfficeRoundList";
    public const string BackOfficeRoundCreate = "BackOfficeRoundCreate";
    public const string BackOfficeRoundView = "BackOfficeRoundView";
    public const string BackOfficeQuestionList = "BackOfficeQuestionList";
    public const string BackOfficeQuestionCreate = "BackOfficeQuestionCreate";
    public const string BackOfficeQuestionView = "BackOfficeQuestionView";
    public const string BackOfficeSessionList = "BackOfficeSessionList";
    public const string BackOfficeSessionCreate = "BackOfficeSessionCreate";
    public const string BackOfficeSessionEdit = "BackOfficeSessionEdit";
    public const string BackOfficeSessionView = "BackOfficeSessionView";
    public const string BackOfficeMailingList = "BackOfficeMailingList";
    public const string BackOfficeMailingCreate = "BackOfficeMailingCreate";
    public const string BackOfficeMailingView = "BackOfficeMailingView";
    public const string BackOfficeMailingSend = "BackOfficeMailingSend";
    public const string BackOfficeServiceView = "BackOfficeServiceView";
    public const string BackOfficeUnlinkUserSessions = "BackOfficeUnlinkUserSessions";

    #endregion

    #region LoadBalancer

    public const string LoadBalancerGameAdminAssign = "LoadBalancerGameAdminAssign";

    #endregion

    #region GameAdmin

    public const string GameAdminMain = "GameAdminMain";
    public const string GameAdminRoundList = "GameAdminRoundList";
    public const string GameAdminRoundView = "GameAdminRoundView";
    public const string GameAdminQuestionList = "GameAdminQuestionList";
    public const string GameAdminQuestionView = "GameAdminQuestionView";
    public const string GameAdminQuestionStart = "GameAdminQuestionStart";
    public const string GameAdminQuestionStop = "GameAdminQuestionStop";
    public const string GameAdminRatingStageShortView = "GameAdminRatingStageShortView";
    public const string GameAdminRatingStageFullView = "GameAdminRatingStageFullView";
    public const string GameAdminGameView = "GameAdminGameView";
    public const string GameAdminSessionLeave = "GameAdminSessionLeave";

    #endregion

    #endregion
}