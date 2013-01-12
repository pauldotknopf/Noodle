SET Version=1.0.0.0
IF NOT "%1"=="" SET Version=%1
%windir%\Microsoft.net\Framework\v4.0.30319\msbuild.exe "..\Noodle.sln" /p:OutDir="%CD%\Output" /p:Configuration="Release" /p:AssemblyVersion="%Version%"
PAUSE