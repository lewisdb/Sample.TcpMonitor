
Sample.TcpMonitor version 1.0.0.0 03/14/2017
---------------------------------------------------------------------------------
Developer: Luat (Lewis) Bui
email: luatbui2@yahoo.ca
---------------------------------------------------------------------------------
Project name: Sample.TcpMonitor
Windows service name: Sample TCP monitor on port 55555
Project description:
This is a windows service which monitors messages on TCP port 55555, and keeps counts
of the number of handshakes and active client connections. The current version only
accepts the following commands from a tcp client:
	HELO (handshake)
	COUNT
	CONNECTIONS
	PRIME
	TERMINATE
---------------------------------------------------------------------------------
Development Environment: Visual Studio 2015
OS: Windows 10, 64 bits
.NET Framework version: 4.5
	.NET Framework 4.5 was chosen for this project as a target to take advantage of asychronous
	methods such as TcpListener.AcceptTcpClientAsync, TcpClient.ReadAsync, 
	TcpClient.WriteAsync
---------------------------------------------------------------------------------
Installation:
System requirements: make sure .NET Framework 4.5
Follow these steps to install the Sample.TcpMonitor service
1.	locate the folder where .NET Framework 4.5 is installed in the local machine,
	for example, C:\Windows\Microsoft.NET\Framework64\v4.0.30319
2.	run the windows command prompt as an administrator
3.	in the windows command prompt, use to 'cd' command to move to the folder where
	.NET Framework 4.5 is installed.
	for example: >cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319
4.	type InstallUtil [folder]\Sample.TcpMonitor.exe, where [folder] is the path of the 
	folder that contains the Sample.TcpMonitor.exe file
	for example, >InstallUtil D:\WorkSpace\_bioconnect\Sample.TcpMonitor\bin\Release\Sample.TcpMonitor.exe

After a successful installation, a "Sample TCP monitor on port 55555" service should be part of the windows services.

To start the service:
NOTE: the project was intentionally compiled with the StartType option 'Manual'
1.	Start the Computer Management Console
2.	In the Computer Management Console, under 'services and applications', select 'services'
3.	On the right hand side is the list of services, locate "Sample TCP monitor on port 55555"
	right click and start "Sample TCP monitor on port 55555".
---------------------------------------------------------------------------------
Copyright: none