@ECHO OFF
SETLOCAL

IF "%TargetVisualStudioVersion%"=="v8.0" (
	SET DegradeToolsVersion=/toolsversion:2.0
) ELSE (
	SET TargetVisualStudioVersion=v9.0
	SET DegradeToolsVersion=/toolsversion:3.5
)

CALL "%~dp0BuildTestsWithAutomation.bat" %* /consoleloggerparameters:DisableMPLogging %DegradeToolsVersion%