set msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
%msbuild% "./ASCOM.Sony.Camera.sln" /p:configuration=debug /property:Platform="Any CPU" /t:Clean,Build
powershell -NoLogo -NoProfile -Command (get-item -Path bin\ASCOM.Sony.Camera.dll).VersionInfo.FileVersion>version.txt
set /p VERSION=<version.txt
del version.txt
"c:\Program Files (x86)\Inno Setup 6\ISCC.exe" "./ASCOM.Sony.Camera.Setup.iss" /DApplicationVersion=%VERSION%

pause