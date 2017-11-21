@echo off
echo @echo off > mdoc
echo set writer=dotnet %~dp0\Writer.dll %%1 %%2 >> mdoc
echo %%writer%% >> mdoc
move mdoc C:\Windows\mdoc.bat
cls
@echo off
echo wirter has been installed
pause