using System.Diagnostics;
using YoutubeIPv6BlockForVRChat.Utils;

namespace YoutubeIPv6BlockForVRChat.Forms;

public partial class MainForm : Form
{
    private bool isInitializing = true;

    public MainForm()
    {
        InitializeComponent();
    }

    private void Form1_Shown(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Minimized;

        //残存ファイアウォールルールのクリーンアップ
        FirewallUtils.DeleteFirewallRule();

        //タスク有無を確認し、タスクが登録済みの場合はスタートアップのチェックボックスON
        if (TaskUtils.IsTaskExist())
        {
            checkBoxAutoStart.Checked = true;
        }

        //VRC起動監視 開始
        timerCheckVRCInitializing.Start();

        isInitializing = false;
        Debug.WriteLine("App Init Finish");
    }

    private void ButtonBlock_Click(object sender, EventArgs e)
    {
        if (IsVRCRunning())
        {
            MessageBox.Show("VRChat起動中は操作できません", Properties.Resources.AppName);
            return;
        }

        ExecuteBlock();
    }

    private void ButtonUnblock_Click(object sender, EventArgs e)
    {
        if (IsVRCRunning())
        {
            MessageBox.Show("VRChat起動中は操作できません", Properties.Resources.AppName);
            return;
        }

        ExecuteUnblock();
    }

    private void CheckBoxAutoStart_CheckedChanged(object _, EventArgs e)
    {
        if (isInitializing) return;

        if (checkBoxAutoStart.Checked)
        {
            TaskUtils.CreateTask();
        }
        else
        {
            TaskUtils.DeleteTask();
        }
    }

    private void TimerCheckVRCInitializing_Tick(object sender, EventArgs e)
    {
        if (!IsVRChatInitializing())
        {
            return;
        }

        timerCheckVRCInitializing.Stop();
        ExecuteBlock();
        timerCheckVRCRunning.Start();
    }

    private void TimerCheckVRCRunning_Tick(object sender, EventArgs e)
    {
        if (IsVRChatInitializing() || IsVRCRunning())
        {
            return;
        }

        timerCheckVRCRunning.Stop();
        ExecuteUnblock();
        timerCheckVRCInitializing.Start();
    }

    private void ExecuteBlock()
    {
        FirewallUtils.CreateFirewallRule();
        buttonBlock.Enabled = false;
        buttonUnblock.Enabled = true;
        IPv6Block.Checked = true;
    }

    private void ExecuteUnblock()
    {
        FirewallUtils.DeleteFirewallRule();
        buttonBlock.Enabled = true;
        buttonUnblock.Enabled = false;
        IPv6Block.Checked = false;
    }

    private void IPv6Block_Click(object sender, EventArgs e)
    {
        if (IsVRCRunning())
        {
            MessageBox.Show("VRChat起動中は操作できません", Properties.Resources.AppName);
            return;
        }

        if (IPv6Block.Checked)
        {
            ButtonUnblock_Click(sender, e);
        }
        else
        {
            ButtonBlock_Click(sender, e);
        }
    }

    private void ShowMenu_Click(object sender, EventArgs e)
        => WindowState = FormWindowState.Normal;

    private void Exit_Click(object sender, EventArgs e)
        => Close();

    private void Form1_SizeChanged(object sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            ShowInTaskbar = false;
        }
        else if (WindowState == FormWindowState.Normal)
        {
            ShowInTaskbar = true;
        }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        => FirewallUtils.DeleteFirewallRule();

    private static bool IsVRCRunning()
        => Process.GetProcessesByName("VRChat").Length != 0;

    private static bool IsVRChatInitializing()
    {
        const string targetProcessName = "start_protected_game";

        try
        {
            if (Process.GetProcessesByName(targetProcessName).Length != 0)
            {
                var processFilePath = Process.GetProcessesByName(targetProcessName)[0].MainModule.FileName;
                var parentDirName = Directory.GetParent(processFilePath).Name;
                return parentDirName == "VRChat";
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
}
