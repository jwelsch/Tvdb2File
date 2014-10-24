@ECHO OFF
SET ConfigurationName=%~1
SET SolutionDir=%~2
SET TargetDir=%~3
SET TargetPath=%~4
SET TargetName=%~5
IF /I "%ConfigurationName%" == "Release" (
  @ECHO ON
  "%SolutionDir%packages\ilmerge.2.13.0307\ILMerge.exe" /out:"%TargetDir%%TargetName%M.exe" "%TargetPath%" "%TargetDir%CommandLineLib.dll"
  del "%TargetPath%"
  move "%TargetDir%%TargetName%M.exe" "%TargetPath%"
)