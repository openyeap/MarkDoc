@echo off
echo @echo off > mdoc.bat
echo set writer=dotnet %~dp0\Writer.dll %%1 %%2 >> mdoc.bat
echo %%writer%% >> mdoc.bat
move mdoc.bat C:\Windows\mdoc.bat
cls
@echo off
echo wirter has been installed
pause