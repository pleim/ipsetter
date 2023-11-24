# IP setter service
Windows background service to set ipaddresses of network interfaces

## Background
I found no solution to allow a non privileged user to change IP addresses on networik interfaces.
This service can be installed as background service and runs with system rights. The service listens as TCP server for new settings.

## Guide
__Install the service as described here:__  
https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service?pivots=dotnet-6-0
Verify that the service is installed and set as automatic.

__Configure appsettings.json:__  
Set the name of the interface where ip settings should be applied. There is no check if the interface really exists. Set TCP port number to listen for ip change command.
```
...
  "Interface": "eth3",
  "Port": 1234
}
```

__Apply new settings:__
Connect with a raw tcp client to localhost:1234 and you should see a prompt ```ip nm gw```. The feedback is ```OK``` when successfull or ```ERR``` when failed. All actions will be logged in the system log. 
Prepare a script for this procedure.
```
ip nm gw
192.168.22.23 255.255.255.0 192.168.22.1
ACK - OK
```

