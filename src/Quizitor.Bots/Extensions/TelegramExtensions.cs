using Quizitor.Bots.Behaviors.BackOffice;
using Quizitor.Bots.Behaviors.BackOffice.Bots;
using Quizitor.Bots.Behaviors.BackOffice.Games;
using Quizitor.Bots.Behaviors.BackOffice.Games.Rounds;
using Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Questions;
using Quizitor.Bots.Behaviors.BackOffice.Games.Sessions;
using Quizitor.Bots.Behaviors.BackOffice.Mailings;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Audience;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Channel;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Bots;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.BotTypes;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Games;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Sessions;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Teams;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Users;
using Quizitor.Bots.Behaviors.BackOffice.Services;
using Quizitor.Bots.Behaviors.BackOffice.Users;
using Quizitor.Bots.Behaviors.GameAdmin;
using Quizitor.Bots.Behaviors.GameAdmin.Games;
using Quizitor.Bots.Behaviors.GameAdmin.Rating;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions;
using Quizitor.Bots.Behaviors.GameAdmin.Sessions;
using Quizitor.Bots.Behaviors.GameServer;
using Quizitor.Bots.Behaviors.GameServer.Default;
using Quizitor.Bots.Behaviors.GameServer.Games;
using Quizitor.Bots.Behaviors.GameServer.Rating;
using Quizitor.Bots.Behaviors.GameServer.Sessions;
using Quizitor.Bots.Behaviors.GameServer.Teams;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.LoadBalancer;
using Quizitor.Bots.Behaviors.Universal;

namespace Quizitor.Bots.Extensions;

internal static class TelegramExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddBehaviors()
        {
            return services
                .AddUniversalBehaviors()
                .AddLoadBalancerBehaviors()
                .AddGameAdminBehaviors()
                .AddGameServerBehaviors()
                .AddBackOffBehaviors();
        }

        private IServiceCollection AddUniversalBehaviors()
        {
            return services
                .AddBehavior<NotImplementedUniv>()
                .AddBehavior<LogInteractionUniv>()
                .AddBehavior<CancelPromptUniv>();
        }

        private IServiceCollection AddLoadBalancerBehaviors()
        {
            return services
                .AddBehavior<BalanceGameAdminLb>()
                .AddBehavior<BalanceGameServerLb>();
        }

        private IServiceCollection AddGameAdminBehaviors()
        {
            return services
                .AddBehavior<MainPageGa>()
                .AddBehavior<SessionJoinGa>()
                .AddBehavior<RoundListGa>()
                .AddBehavior<RoundViewGa>()
                .AddBehavior<QuestionViewGa>()
                .AddBehavior<QuestionStartGa>()
                .AddBehavior<QuestionStopGa>()
                .AddBehavior<QuestionTimeGa>()
                .AddBehavior<RatingStageShortGa>()
                .AddBehavior<RatingStageFullGa>()
                .AddBehavior<GameGa>()
                .AddBehavior<SyncRatingEnableGameGa>()
                .AddBehavior<SyncRatingDisableGameGa>()
                .AddBehavior<SessionLeaveGa>();
        }

        private IServiceCollection AddGameServerBehaviors()
        {
            return services
                .AddBehavior<MainPageGs>()
                .AddBehavior<TeamGs>()
                .AddBehavior<TeamSetLeaderGs>()
                .AddBehavior<TeamUnsetLeaderGs>()
                .AddBehavior<TeamQrGs>()
                .AddBehavior<TeamSessionJoinGs>()
                .AddBehavior<TeamLeaveGs>()
                .AddBehavior<TeamListGs>()
                .AddBehavior<CreateTeamGs>()
                .AddBehavior<TeamJoinGs>()
                .AddBehavior<GameGs>()
                .AddBehavior<SessionJoinGs>()
                .AddBehavior<SessionLeaveGs>()
                .AddBehavior<RatingFinalShortGs>()
                .AddBehavior<RatingFinalFullGs>()
                .AddBehavior<DefaultGs>();
        }

        private IServiceCollection AddBackOffBehaviors()
        {
            return services
                .AddBehavior<MainPageBo>()
                .AddBehavior<BotListBo>()
                .AddBehavior<BotListStartBo>()
                .AddBehavior<BotListStopBo>()
                .AddBehavior<BotViewBo>()
                .AddBehavior<BotStartBo>()
                .AddBehavior<BotStopBo>()
                .AddBehavior<BotTypeBo>()
                .AddBehavior<BotPendingBo>()
                .AddBehavior<UserListBo>()
                .AddBehavior<UserViewBo>()
                .AddBehavior<UserRoleListBo>()
                .AddBehavior<UserRoleListGrantBo>()
                .AddBehavior<UserRoleListRevokeBo>()
                .AddBehavior<MailingListBo>()
                .AddBehavior<CreateMailingBo>()
                .AddBehavior<DeleteMailingBo>()
                .AddBehavior<DeleteMailingConfirmBo>()
                .AddBehavior<MailingViewBo>()
                .AddBehavior<MailingPreviewBo>()
                .AddBehavior<MailingProfileBo>()
                .AddBehavior<MailingAudienceBo>()
                .AddBehavior<MailingAudienceGameListBo>()
                .AddBehavior<MailingAudienceGameBo>()
                .AddBehavior<MailingAudienceSessionListBo>()
                .AddBehavior<MailingAudienceSessionBo>()
                .AddBehavior<MailingAudienceTeamListBo>()
                .AddBehavior<MailingAudienceTeamBo>()
                .AddBehavior<MailingAudienceUserListBo>()
                .AddBehavior<MailingAudienceUserBo>()
                .AddBehavior<MailingChannelBo>()
                .AddBehavior<MailingChannelContactTypeBo>()
                .AddBehavior<MailingChannelBotTypeListBo>()
                .AddBehavior<MailingChannelBotTypeBo>()
                .AddBehavior<MailingChannelBotListBo>()
                .AddBehavior<MailingChannelBotBo>()
                .AddBehavior<MailingSchemaBo>()
                .AddBehavior<MailingSendBo>()
                .AddBehavior<MailingSendConfirmBo>()
                .AddBehavior<GameListBo>()
                .AddBehavior<CreateGameBo>()
                .AddBehavior<GameViewBo>()
                .AddBehavior<DeleteGameBo>()
                .AddBehavior<DeleteGameConfirmBo>()
                .AddBehavior<RoundListBo>()
                .AddBehavior<CreateRoundBo>()
                .AddBehavior<RoundViewBo>()
                .AddBehavior<QuestionViewBo>()
                .AddBehavior<QuestionSubmissionNotificationTypeBo>()
                .AddBehavior<SessionListBo>()
                .AddBehavior<CreateSessionBo>()
                .AddBehavior<SessionViewBo>()
                .AddBehavior<SessionGetQrBo>()
                .AddBehavior<ServicePageBo>()
                .AddBehavior<UnlinkUserSessionsBo>()
                .AddBehavior<Load100Bo>();
        }

        private IServiceCollection AddBehavior<TBehavior>()
            where TBehavior : class, IBehavior
        {
            services
                .AddScoped<IBehavior, TBehavior>()
                .AddScoped<TBehavior>();

            return services;
        }
    }
}