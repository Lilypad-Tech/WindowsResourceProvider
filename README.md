# WindowsResourceProvider
This installer is designed to help you set up and configure your system to provide resources to the Lilypad network. Once the installation is complete, your system will be ready to start accepting compute jobs from the Lilypad network.

# Usage
Enabling Windows Subsystem for Linux (WSL) allows you to run a Linux distribution alongside your Windows installation. Here's how you can enable and set up WSL on your Windows machine:

## 1. Enable WSL
Open PowerShell as Administrator

Right-click on the Start button and select "Windows PowerShell (Admin)" or "Command Prompt (Admin)".

Run the following commands:

```
dism.exe /online /enable-feature /featurename:VirtualMachinePlatform /all /norestart
dism.exe /online /enable-feature /featurename:Microsoft-Windows-Subsystem-Linux /all /norestart
```

Restart your computer to apply the changes.

## 2. Install Docker
https://docs.docker.com/desktop/install/windows-install/

## 3. Compile/Run Project
Clone this repository

Open Solution File with Visual Studio (`WindowsResourceProvider.sln`)

# Resources
- [Lilypad Docs](https://docs.lilypad.tech/lilypad)
- [Read our Blog](https://lilypad.team/blog)
- [Join the Discord](https://lilypad.team/discord)
- [Follow us on Twitter/X](https://lilypad.team/x)
- [Check out our videos on YouTube](https://lilypad.team/youtube)
