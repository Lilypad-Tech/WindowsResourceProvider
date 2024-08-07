del WindowsResourceProvider.7z
del WindowsResourceProviderSetup.exe
del WindowsResourceProvider.zip
del WindowsResourceProviderSetup.zip

7z a c:\tmp\WindowsResourceProvider.7z c:\tmp\setup.exe c:\tmp\WindowsResourceProvider.application c:\tmp\start.bat "c:\tmp\Application Files"
7z a c:\tmp\WindowsResourceProvider.zip c:\tmp\setup.exe c:\tmp\WindowsResourceProvider.application c:\tmp\start.bat "c:\tmp\Application Files"
copy /b c:\tmp\7zSD.sfx + c:\tmp\config.txt + c:\tmp\WindowsResourceProvider.7z c:\tmp\WindowsResourceProviderSetup.exe
7z a c:\tmp\WindowsResourceProviderSetup.zip c:\tmp\WindowsResourceProviderSetup.exe

copy c:\tmp\WindowsResourceProviderSetup.exe "G:\My Drive\WindowsResourceProvider\" /Y
copy c:\tmp\WindowsResourceProviderSetup.zip "G:\My Drive\WindowsResourceProvider\" /Y
copy c:\tmp\WindowsResourceProvider.7z "G:\My Drive\WindowsResourceProvider\" /Y
copy c:\tmp\WindowsResourceProvider.zip "G:\My Drive\WindowsResourceProvider\" /Y