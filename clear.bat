echo Parameter：%1
echo 当前盘符：%~d0
echo 当前盘符和路径：%~dp0
echo 当前批处理全路径：%~f0
echo 当前盘符和路径的短文件名格式：%~sdp0
echo 当前CMD默认目录：%cd%
echo 目录中有空格也可以加入""避免找不到路径
echo 当前盘符："%~d0"
echo 当前盘符和路径："%~dp0"
echo 当前批处理全路径："%~f0"
echo 当前盘符和路径的短文件名格式："%~sdp0"
echo 当前CMD默认目录："%cd%"

REM - performs a remove directory, with wildcard matching - example ; rd-wildcard 2007-*
dir BIN /b /s >loglist.txt
setlocal enabledelayedexpansion
for /f %%a in (./loglist.txt) do (
	rd /s /q %%a
)
del /q loglist.txt
endlocal

dir OBJ /b /s >loglist.txt
setlocal enabledelayedexpansion
for /f %%a in (./loglist.txt) do (
	rd /s /q %%a
)
del /q loglist.txt
endlocal

dir project.lock.json /b /s >loglist.txt
dir *.targets /b /s >>loglist.txt
dir *.user /b /s >>loglist.txt
setlocal enabledelayedexpansion
for /f %%a in (./loglist.txt) do (
	del /s /q %%a
) 
del /q loglist.txt
endlocal

rd /s /q Publish
