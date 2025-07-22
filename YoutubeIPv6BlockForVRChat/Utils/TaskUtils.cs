using Microsoft.Win32.TaskScheduler;

namespace YoutubeIPv6BlockForVRChat.Utils;

internal static class TaskUtils
{
    internal static void CreateTask()
    {
        var TaskName = Properties.Resources.AppName;
        var TaskExeFile = Environment.ProcessPath;

        using TaskService TaskData = new TaskService();
        TaskDefinition TaskDefine = TaskData.NewTask();
        TaskDefine.Principal.RunLevel = TaskRunLevel.Highest;
        TaskDefine.Principal.LogonType = TaskLogonType.InteractiveToken;
        TaskDefine.Actions.Add(new ExecAction(TaskExeFile));
        TaskDefine.Triggers.Add(new LogonTrigger());
        TaskDefine.RegistrationInfo.Author = "NyaHo";
        TaskDefine.Settings.DisallowStartIfOnBatteries = false;
        TaskData.RootFolder.RegisterTaskDefinition(TaskName, TaskDefine, TaskCreation.CreateOrUpdate, null, null, TaskLogonType.InteractiveToken, null);
        return;
    }

    internal static void DeleteTask()
    {
        var TaskName = Properties.Resources.AppName;
        var TaskService = new TaskService();
        TaskService.RootFolder.DeleteTask(TaskName);
    }

    internal static bool IsTaskExist()
    {
        using var TaskService = new TaskService();
        var Task = TaskService.FindTask(Properties.Resources.AppName);
        return Task != null;
    }
}
