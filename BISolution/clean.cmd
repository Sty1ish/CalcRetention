@echo off
for /d /r %%i in (obj) do rd /s /q "%%i"
for /d /r %%i in (x64) do rd /s /q "%%i"
for /d /r %%i in (bin) do rd /s /q "%%i"
for /d /r %%i in (Publish) do rd /s /q "%%i"
for /d /r %%i in (publish) do rd /s /q "%%i"
@ECHO =====================================
@ECHO Clean Completed
