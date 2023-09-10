using System.Text;
using Pororoca.Desktop.UITesting.Tests;

namespace Pororoca.Desktop.UITesting;

public static class UITestsRunner
{
    public static Task<string> RunAllTestsAsync() => RunTestsAsync(new UITest[]
    {
        new TopMenuUITest(),
        new SwitchLanguagesUITest(),
        new SwitchThemesUITest(),
        new EditableTextBlockUITest(),
        new CollectionAndCollectionFolderUITest(),
        new TreeCopyAndPasteItemsUITest(),
        new TreeCutAndPasteItemsUITest(),
        new TreeDeleteItemsUITest(),
        new HttpRequestValidationsUITest(),
        new HttpRequestsUITest(),
        // TODO: UI tests to be made:
        // new HeadersCutCopyPasteUITest(),
        // new VariablesCutCopyPasteUITest(),
        // new FormDataParamsCutCopyPasteUITest(),
        // foreach (var version in new[] { 1.0m, 2.0m })
        // new WebSocketConnectionAndClientMessagesUITest(version)
        // new WebSocketConversationUITest(version)
        // Out of scope of automated UI tests:
        // some keybindings
        // import and export collections and environments (involve dialogs)
        // help dialogs
        // context menu actions
        // cut collection, cut and paste to itself
        // delete items actions
        // save responses to file
    });

    private static async Task<string> RunTestsAsync(params UITest[] tests)
    {
        static TimeSpan SumTotalTime(IEnumerable<UITest> ts)
        {
            var totalTime = TimeSpan.Zero;
            foreach (var t in ts)
            {
                totalTime += t.ElapsedTime;
            }
            return totalTime;
        }

        StringBuilder allTestsLogsAppender = new();
        foreach (var test in tests)
        {
            await RunTestAsync(allTestsLogsAppender, test);
        }
        var totalTime = SumTotalTime(tests);
        string fmtTime = @"hh'h'mm'm'ss's'";
        allTestsLogsAppender.AppendLine("TOTAL TIME: " + totalTime.ToString(fmtTime));
        return allTestsLogsAppender.ToString();
    }

    private static async Task RunTestAsync(StringBuilder allTestsLogsAppender, UITest test)
    {
        try
        {
            test.Start();
            await test.RunAsync();
        }
        catch (Exception ex)
        {
            allTestsLogsAppender.AppendLine(ex.ToString());
        }
        finally
        {
            test.Finish();
            allTestsLogsAppender.AppendLine($"{test.TestName}: {(test.Successful == true ? "SUCCESS" : "FAILED")} {test.TotalElapsedSeconds}s");
            if (!string.IsNullOrWhiteSpace(test.Log))
            {
                allTestsLogsAppender.AppendLine(test.Log);
            }
        }
    }
}