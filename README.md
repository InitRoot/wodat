# wodat
Windows Oracle Database Attack Tool

![Follow on Twitter](https://img.shields.io/twitter/follow/initroott?label=Follow%20&style=social)
![GitHub last commit](https://img.shields.io/github/last-commit/initroot/wodat)
![GitHub stars](https://img.shields.io/github/stars/initroot/wodat)
 
Simple port of the popular  Oracle Database Attack Tool (ODAT) (https://github.com/quentinhardy/odat) to C# .Net Framework. 
Credit to https://github.com/quentinhardy/odat as lots of the functionality are ported from his code.
* Perform password based attacks e.g. username as password, username list against given password, password list against given username, username:pass combolist.
* Test if a credential/connection string is working against target
* Brute force attacks to discover valid SID/ServiceNames
* More to come I hope!

##  Disclaimer
I take not responsibility for your use of the software. Development is done in my personal capacity and carry no affiliation to my work.

## Usage
The general command line arguments required are as follow:

```
wodat.exe COMMAND ARGGUMENTS
 COMMAND (ALL,BRUTECRED,BRUTESID,BRUTESRV,TEST)
 -server:XXX.XXX.XXX.XXX -port:1520
 -sid:AS OR -srv:AS
 -user:Peter -pass:Password

```
To test if a specific credential set works.
```
wodat.exe TEST -server:XXX.XXX.XXX.XXX -port:1521 -sid:XE -user:peter -pass:pan

```
See the outline on modules for further usage. The tool will always first check if the TNS listener that is targeted works.

## Modules
### ALL
Not implemented yet.

#### RECON
Not implemented yet.

#### BRUTECRED
Module performs wordlist password based attack. The following options exist:
```
A - username:password combolist with no credentials given during arguments
B - username list with password given in arguments
C - password list with username given in arguments
D - username as password with username list provided
```
To perform a basic attack with a given file that has username:password combos.
```
wodat.exe BRUTECRED -server:XXX.XXX.XXX.XXX -port:1521 -sid:XE

```
#### BRUTESID
Module performs wordlist SID guessing attack if not successfull will ask for brute force attack.
```
wodat.exe BRUTESID -server:XXX.XXX.XXX.XXX -port:1521
```

#### BRUTESRV
Module performs wordlist ServiceName guessing attack if not successfull will ask for brute force attack.
```
wodat.exe BRUTESRV -server:XXX.XXX.XXX.XXX -port:1521
```

#### TEST
Module tests if the given connection string can connect successfully.
```
wodat.exe TEST -server:XXX.XXX.XXX.XXX -port:1521 -sid:XE -user:peter -pass:pan
```
## Setup and Requirements
You can grab automated release build or build yourself using the following commands:

```
nuget restore wodat.sln
msbuild wodat.sln -t:rebuild -property:Configuration=Release

```
Some general notes:
The `Oracle.ManagedDataAccess.dll` library will have to be copied with the binary. I'm looking at ways of embedding it.

## Changelog
Version 0.1 - Base toolkit and functionality
