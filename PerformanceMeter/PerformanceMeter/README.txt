Performance Meter is a console tool used to test applications for parallelism by
measuring their execution times on specified number of processor logical cores.
Tests are analysed using Gustafsons Law and alpha factor is returned describing what part of the program
is linear.

-- INSTALLATION --
1) Pre-requisites:
This program is build on .NET Core 2.0 Framework.
To run the software first download .NET Core Framework (Runtime packet) for your operating system from:
https://www.microsoft.com/net/download

This software is designed for X64 architecture and will not work on X86 systems (where support for multithreading is limited).
Supported operating systems match those from .NET Core Framework:
 - Windows
 - Linux
 - MacOS

2) Installation:
Program has no installer, unzip the binaries to desired location.
PerformanceMeter.dll is a program that is run using 'dotnet' command (from .NET Core framework).

-- RUNNING THE PROGRAM --
1) How to run:
   Performance Meter is executed using .NET Core framework.
   To start the program open the bash/console (applicable for your OS) in the installation directory of the program and type: 
   dotnet PerformanceMeter.dll
   The following will start Performance Meter without any arguments and will display help.

2) Input arguments:
   To provide input arguments simply append them to Performance Meter run command, example:
   dotnet PerformanceMeter.dll -a "C:\Windows\System32\cmd.exe" -i "runas /user:domainname\username" -p AboveNormal -c 3 -o "Output.xml"

-- HOW TO USE --
Naming: "AUT" - Application Under Test (The program that is tested by Performance Meter for parallelism).
Usage of the program is devided into two main parts:
  - Performing tests of the AUT
  - Analizing results of the AUT from the xml file that contains results

1) Running AUT test:
   To see the list of Performance Meter input arguments see programs help by calling: dotnet PerformanceMeter.dll
   Usage syntax ([] - encloses optional parameters):
   dotnet PerformanceMeter.dll -a "<pathToAUT>" [-i "<AUTInputArgs>" ] [-p "<AUTProcessPriority>" ] [-c "<AUTCores>"] [-o "<PathToResultFile.xml>"]

   Usage example:
   dotnet PerformanceMeter.dll -a "C:\Windows\System32\cmd.exe" -i "runas /user:admin someProgram" -p "AboveNormal" -c "1, 2, 4, 8" -o "ResultFile.xlm"
  
2) Running test analisys:
   To run AUT tests analysys at least two test results are required to compare and at least one must be executed on single core.
   Usage syntax:
   dotnet PerformanceMeter.dll -a "<pathToAUT>" -r "<PathToResultsFile.xml>"

   Usage example:
   dotnet PerformanceMeter.dll -a "C:\Windows\System32\cmd.exe" -r "C:\ProgramFiles\PerformanceMeter\Results.xml"

-- CONFIGURATION AND LOGGING --
Performance Meter has several configuration files, all reside in "Settings" folder.

1) PerformanceMeter.config:
Configuration file containing Performance Meter settings.
This file allows to: set AUT input / output redirection, AUT run mode (backgound or own window) and default results file name
(file where AUT test results are written if -o flag is not specified).

2) AutLogger.conf:
Configuration file containing settings of the logger that registers input / output of the AUT itself.
This logger is very configurable. To see examples and documentation of the logger visit: 
https://logging.apache.org/log4net/release/config-examples.html

3) Logger.config:
Configuration file containing settings of the logger that registers Performance Meter actions.
This logger is very configurable. To see examples and documentation of the logger visit: 
https://logging.apache.org/log4net/release/config-examples.html






This software is sorely developed by Andrey Borisovich and may not be:
decompilated, modified, sold or redistributed without further author notice.
Software is provided "as is", the author does not take responsibility for any
issues or damages cause by usage of this software.
Performance Meter is distributed as freeware and may not be sold.