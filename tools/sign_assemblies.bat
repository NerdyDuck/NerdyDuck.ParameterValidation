@ECHO OFF
REM sign_assemblies.bat
REM Signs the project output assemblies
REM Copyright 2015 Daniel Kopp
REM Licensed under the Apache License, Version 2.0

SET KEY_NAME="HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.1A"
SET VALUE_NAME=InstallationFolder
SET TIMESTAMP_URL=http://www.startssl.com/timestamp
SET INFO_URL="https://www.nerdyduck.de"
SET HASHALG=sha256
SET OUTPUT_PATH=%1
SET BASE_NAME=%2
SET TARGET_EXT=%3

FOR /F "usebackq tokens=2,* skip=2" %%L IN (`REG QUERY %KEY_NAME% /v %VALUE_NAME% 2^>nul`) do (SET Sdk7Path=%%M)
IF NOT DEFINED Sdk7Path GOTO error_No_SDK

FOR /R %OUTPUT_PATH% %%G IN (%BASE_NAME%*.dll) DO (
  "%Sdk7Path%bin\signtool.exe" sign /a /q /fd %HASHALG% /tr %TIMESTAMP_URL% /du %INFO_URL% "%%G"
)

IF "%TARGET_EXT%"==".dll" goto end

FOR /R %OUTPUT_PATH% %%G IN (%BASE_NAME%*%TARGET_EXT%) DO (
  "%Sdk7Path%bin\signtool.exe" sign /a /q /tr /fd %HASHALG% %TIMESTAMP_URL% /du %INFO_URL% "%%G"
)

goto end

:error_No_SDK
ECHO Path to Windows 7.1A SDK not found in registry (%KEY_NAME:"=%@%VALUE_NAME%)
goto end

:end
