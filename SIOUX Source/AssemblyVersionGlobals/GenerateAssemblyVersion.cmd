@ECHO off

echo Generation assembly version

FOR /F "tokens=1,2 delims=:M" %%A in ('svnversion ../ -c') do SET PART_1=%%A&SET PART_2=%%B

SET file="%~dp0\AssemblyVersion.cs"

IF EXIST %file% (del %file%)

IF NOT DEFINED PART_2 (SET SVN_REV=%PART_1%)

IF NOT DEFINED SVN_REV (SET SVN_REV=%PART_2%)

ECHO using System.Reflection;> %file%
ECHO.>> %file% 
ECHO // Version information for an assembly consists of the following four values:>> %file%
ECHO //>> %file%
ECHO //      Major Version>> %file%
ECHO //      Minor Version>> %file%
ECHO //      Build Number>> %file%
ECHO //      Revision>> %file%
ECHO //>> %file%
ECHO // You can specify all the values or you can default the Build and Revision Numbers>> %file%
ECHO // by using the '*' as shown below:>> %file%
ECHO // [assembly: AssemblyVersion("1.0.*")]>> %file%
ECHO [assembly: AssemblyVersion("5.0.0.%SVN_REV%")]>> %file%
ECHO [assembly: AssemblyFileVersion("5.0.0.%SVN_REV%")]>> %file%

:end