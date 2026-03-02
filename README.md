# MyPC Custom Patch
Breaking the Reservation system. When the computers are in the 'Reserved' state to unauthorized users.

## What was the vulnerability?
The vulnerability was to do with people ending two processes in a specific order:

```shell
*nwlg.exe*
```

```shell
*lgmpc.exe*
```

This could have simply been replicated by performing quickly in task manager or running a batch file containing this soon as you log in on your desktop.

```shell
@echo off
:: Declare the start of a loop
:a
timeout 0
:: Ending the two processes
taskkill /IM nwlg.exe /f
taskkill /IM lgmpc.exe /f
:: Go back to the start of the loop.
goto a
```

And so I've solved this by making my own C# background application (which was AI assisted via Proton Lumo in early 2026)

It starts a loop & checks processes every 30 seconds.

It looks for the presence of EITHER *MyPCUserSession.exe* OR the presence of both *nwlg.exe* & *lgmpc.exe*, and if one of those two processes were missing after the first 15 seconds of the program waiting after launch. The user gets a message and then signed out.

After the MyPCUserSession process had started and been left alone to operate, it runs as an administrator so the process is safe to run. once the program detects that. It exits and stops operating till next login.

## How do I deploy this patch myself?
Good question, you can deploy it yourself by putting the application in the 'Startup' directory and this will run for every user that logs in. you can do this by opening an Explorer window on the computer you want to deploy it on and change the file path to 'shell:common startup' and it will open the global Startup folder. (This will execute the program as a standard user only - this isn't recommended as the process can be exited by any standard user in Task manager, posing the same vulnerability as before)

![ExplorerPreview](https://github.com/user-attachments/assets/ca92e53f-6fb7-4de6-8057-d001ef589ff2)

It is recommended however that the program is set as Startup but as an administrator too so it cannot be quit by a standard user. To execute as an Administrator you will need to use Task Scheduler (taskschd.msc) or alternative startup solution. The program can be ran as either and doesn't matter but preferably administrator is recommended otherwise all the standard user will have to do is look for an additional process and terminate it.

![AccessDenied](https://github.com/user-attachments/assets/cd335234-d7ae-4e8b-a8b4-3f22891a68cc)

I have created this solution as an alternative way to patch the bug used to circumvent the reservation system and to keep task manager allowed so we can still see what processes are running on the computer and wouldn't cause disruption for any IT courses that require the use of Task manager. Using this method will keep the transparency between the user and the tools running in the background.
