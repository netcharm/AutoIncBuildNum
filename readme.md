# AutoIncBuildNum
Automatic increment of C# project number in VS IDE

Intro
=====
Automatic increment of C# project number in VS IDE, 

Requirements
============
1. VS2015 Express fro Desktop
2. .net framework 2.0

Usage
=====
in project properties page click build and add cmd-line like: 
```
"D:\Develop\MS\AutoIncBuildNumber.exe" "$(ProjectDir)"
```
to pre-build event.

Source
======
1. https://github.com/netcharm/AutoIncBuildNum
2. https://bitbucket.org/netcharm/AutoIncBuildNum
