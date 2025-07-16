using LPlus;
using Quizitor.Bots.Behaviors.Universal;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.UI.Shared;

internal static class Buttons
{
    public delegate string PaginationButtonCallbackDataProvider(int pageNumber);

    public static InlineKeyboardButton CancelPrompt =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_PROMPT_CANCEL_BTN",
            CancelPromptUniv.Button);

    public static IEnumerable<InlineKeyboardButton> BuildPaginationInlineButtons(
        int pageNumber,
        int pageCount,
        PaginationButtonCallbackDataProvider backwardCallbackDataProvider,
        PaginationButtonCallbackDataProvider forwardCallbackDataProvider)
    {
        var buttons = new List<InlineKeyboardButton>();

        if (pageNumber > 0)
            buttons.Add(
                InlineKeyboardButton.WithCallbackData(
                    string.Format(TR.L + "_SHARED_PAGE_BACK_BTN", pageNumber),
                    backwardCallbackDataProvider(pageNumber - 1)));
        if (pageNumber < pageCount - 1)
            buttons.Add(
                InlineKeyboardButton.WithCallbackData(
                    string.Format(TR.L + "_SHARED_PAGE_NEXT_BTN", pageNumber + 2),
                    forwardCallbackDataProvider(pageNumber + 1)));

        return buttons;
    }
}