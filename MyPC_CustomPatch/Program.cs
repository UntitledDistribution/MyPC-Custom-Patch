
// Program is hidden on execution because the Project file is tricked into thinking its launching a UI
// app on an app that has no UI. You can change this by double clicking the item below 'Solution Explorer'
// which should be the project name.
// Swap <OutputType>Exe</OutputType> with <OutputType>WinExe</OutputType> to hide the program, and vise versa
// to show the program again.


// ------------------------------------------------------------
// MyPC watchdog – sign‑out on any process loss
// (admin‑run console app, .NET 6+)
// ------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Threading;

// ------------------------------------------------------------------
// Configuration – absolute paths to the three processes we care about.
// ------------------------------------------------------------------
const string GateKeeperExe = @"C:\Program Files\ITS\MyPC Client\MyPCUserSession.exe";
const string Vulnerable1Exe = @"C:\Program Files\ITS\MyPC Client\nwlg.exe";
const string Vulnerable2Exe = @"C:\Program Files\ITS\MyPC Client\lgmpc.exe";

// ------------------------------------------------------------------
// Helper: display a quick message to the logged‑in user.
// Uses the built‑in `msg` command – no P/Invoke, no extra libraries.
// ------------------------------------------------------------------
static void ShowMessage(string title, string text)
{
    string msg = $"{title}: {text}";
    Process.Start(new ProcessStartInfo
    {
        FileName = "msg",
        Arguments = $"* /time:10 \"{msg}\"",
        CreateNoWindow = true,
        UseShellExecute = false
    })?.Dispose();
}

// ------------------------------------------------------------------
// Helper: force a Windows log‑off.
// ------------------------------------------------------------------
static void ForceLogOff()
{
    System.Threading.Thread.Sleep(10000);
    Process.Start(new ProcessStartInfo
    {
        FileName = "shutdown",
        Arguments = "/l /f",
        CreateNoWindow = true,
        UseShellExecute = false
    })?.Dispose();
}

// ------------------------------------------------------------------
// Utility: does a process with the given *file name* exist?
// ------------------------------------------------------------------
static bool IsProcessRunning(string exePath)
{
    string name = System.IO.Path.GetFileNameWithoutExtension(exePath);
    return Process.GetProcessesByName(name).Length > 0;
}

System.Threading.Thread.Sleep(15000);


// ------------------------------------------------------------------
// 1️⃣  Gate‑keeper presence – if it’s already running we exit.
// ------------------------------------------------------------------
if (IsProcessRunning(GateKeeperExe))
{
    // Debug output only; the console will be hidden in production.
    Console.WriteLine($"{DateTime.Now}: Gate‑keeper already running – exiting.");
    return; // End of program.
}

// ------------------------------------------------------------------
// 2️⃣  No gate‑keeper → start monitoring loop.
// ------------------------------------------------------------------
Console.WriteLine($"{DateTime.Now}: Gate‑keeper NOT running – starting monitor.");


while (true)
{
    // Refresh status each iteration.
    bool gateKeeperAlive = IsProcessRunning(GateKeeperExe);
    bool vulnerable1Alive = IsProcessRunning(Vulnerable1Exe);
    bool vulnerable2Alive = IsProcessRunning(Vulnerable2Exe);

    // If the gate‑keeper appears while we are monitoring, stop cleanly.
    if (gateKeeperAlive)
    {
        Console.WriteLine($"{DateTime.Now}: Gate‑keeper started while monitoring – exiting.");
        break;
    }

    // Trigger when *any* vulnerable process stops and the gate‑keeper is still absent.
    if (!vulnerable1Alive || !vulnerable2Alive)
    {
        string missing = !vulnerable1Alive ? "nwlg.exe" : "lgmpc.exe";

        Console.WriteLine($"{DateTime.Now}: Detected missing process '{missing}'. Initiating log‑off.");

        ShowMessage("MyPC Watchdog - Potential Bypass Detected",
                    $"Required component '{missing}' not running. You will now be signed out.");

        // Short pause so the user sees the message before the forced log‑off.
        Thread.Sleep(3000);
        ForceLogOff();

        // After the log‑off Windows will terminate the process; break just in case.
        break;
    }

    // All monitored processes are healthy – wait before the next check.
    Thread.Sleep(TimeSpan.FromSeconds(30));
}