# wodat
Windows Oracle Database Attack Tool

![Follow on Twitter](https://img.shields.io/twitter/follow/initroott?label=Follow%20&style=social)
![GitHub last commit](https://img.shields.io/github/last-commit/initroot/wodat)

 
Simple port of the popular Oracle Database Attack Tool (ODAT) (https://github.com/quentinhardy/odat) to C# .Net Framework. 
Credit to https://github.com/quentinhardy/odat as lots of the functionality are ported from his code.
* Perform password based attacks e.g. username as password, username list against given password, password list against given username, username:pass combolist.
* Test if a credential/connection string is working against target
* Brute force attacks to discover valid SID/ServiceNames
* Perform discovery of valid TNS listeners against provided target file or CIDR range
* More to come, I hope!

![image](https://user-images.githubusercontent.com/954507/180816033-31dbc5d5-0012-401a-9748-48df230b0fdf.png)

##  Disclaimer
I take not responsibility for your use of the software. Development is done in my personal capacity and carry no affiliation to my work.

## Usage
The general command line arguments required are as follow:

```
wodat.exe COMMAND ARGGUMENTS
 COMMAND (ALL,BRUTECRED,BRUTESID,BRUTESRV,TEST,DISC)
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
#### BRUTESID
Module performs wordlist SID guessing attack if not successful will ask for brute force attack.
```
wodat.exe BRUTESID -server:XXX.XXX.XXX.XXX -port:1521
```
![image](https://user-images.githubusercontent.com/954507/180816431-7bb82722-55cf-4233-9cca-8e809ebf5f4a.png)

#### BRUTESRV
Module performs wordlist ServiceName guessing attack if not successful will ask for brute force attack.
```
wodat.exe BRUTESRV -server:XXX.XXX.XXX.XXX -port:1521
```
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
![image](https://user-images.githubusercontent.com/954507/180830466-3bf2f809-8373-44cc-a72f-bc11ad012283.png)

#### TEST
Module tests if the given connection string can connect successfully.
```
wodat.exe TEST -server:XXX.XXX.XXX.XXX -port:1521 -sid:XE -user:peter -pass:pan
```
![image](https://user-images.githubusercontent.com/954507/180830998-112671d7-d747-43ba-90e9-01c615ca5248.png)

#### DISC
Module will perform discovery against provided CIDR range or file with instances. Note, only instances with valid TNS listeners will be returned.
Testing a network range will be much faster as it’s processed in parallel. 
```
wodat.exe DISC

```
Instances to test must be formatted as per the below example `targets.txt`:

```
192.168.10.1
192.168.10.5,1521

```


### ALL
Not implemented yet.

#### RECON
Not implemented yet.


## Setup and Requirements
You can grab automated release build from the GitHub Actions or build yourself using the following commands:

```
nuget restore wodat.sln
msbuild wodat.sln -t:rebuild -property:Configuration=Release

```
Some general notes:
The `Oracle.ManagedDataAccess.dll` library will have to be copied with the binary. I'm looking at ways of embedding it.

## Todo
 - Handle SYSDBA and SYSOPER connections
 - Implement outstanding modules
 - Various validation, error handling code still needs to be done
 - Some minor known bugfixes
 - Add options to check against built in lists for SID, ServiceNames or common credentials
 
## Changelog
Version 0.1 - Base toolkit and functionality
Version 0.2 - Several bugfixes, improved socket connection and added RECON module
