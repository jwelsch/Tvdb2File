@ECHO OFF
SET ConfigurationName=%~1
SET SolutionDir=%~2
SET ProjectDir=%~3
IF /I "%ConfigurationName%" == "Release" (
   @ECHO ON
   "%SolutionDir%AssemblyVersionIncrement-1.0.0.3\AssemblyVersionIncrement.exe" Build "%ProjectDir%Properties\AssemblyInfo.cs"
)