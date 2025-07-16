using System.ComponentModel.DataAnnotations;

namespace Quizitor.Data.Enums;

public enum BotType
{
    [Display(Name = "univ")]
    Universal = 0,

    [Display(Name = "lb")]
    LoadBalancer,

    [Display(Name = "adm")]
    GameAdmin,

    [Display(Name = "game")]
    GameServer,

    [Display(Name = "bo")]
    BackOffice = int.MaxValue
}