"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" "./ASCOM.DSLR.sln" /p:configuration=debug /property:Platform="x86"
powershell -NoLogo -NoProfile -Command (get-item -Path bin\ASCOM.DSLR.Camera.dll).VersionInfo.FileVersion>version.txt
set /p VERSION=<version.txt
del version.txt
"c:\Program Files (x86)\Inno Setup 6\ISCC.exe" "./DSLR.Camera Setup.iss" /DApplicationVersion=%VERSION%

pause