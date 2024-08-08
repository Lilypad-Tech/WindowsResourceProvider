# WindowsResourceProvider
Enabling Windows Subsystem for Linux (WSL) allows you to run a Linux distribution alongside your Windows installation. Here's how you can enable and set up WSL on your Windows machine:

# Step 1: Enable WSL
Open PowerShell as Administrator

Right-click on the Start button and select "Windows PowerShell (Admin)" or "Command Prompt (Admin)".

Run the Following Commands

powershell
Copy code
```dism.exe /online /enable-feature /featurename:VirtualMachinePlatform /all /norestart
dism.exe /online /enable-feature /featurename:Microsoft-Windows-Subsystem-Linux /all /norestart
```
Restart your computer to apply the changes.

# Step 2 Install Docker
https://docs.docker.com/desktop/install/windows-install/

# Step 3 Compile/Run Project
Clone repository

Open Solution File
