# TheCDTroll
I got bored and created this annoying tool for opening CD trays of PCs in local network (and more)

## How to install
To install go to [Releases](https://github.com/loudsheep/TheCDTroll/releases) and download latest wizard .zip file. After downloading unpack files from zip and run **setup.exe** inside **wizard folder**.

## Because this app can remotely open processes and execute commands Windows Defender detects it as a virus. To install you have to **run anyway** in Defender when prompted!

<br><br>
## After install
When app is installed it will automaticaly run and hide to System tray.

![img1](https://github.com/loudsheep/TheCDTroll/blob/master/Resources/img3.jpg)

To send commands right click it and select option **Open**.

After that the following window will show.

![img2](https://github.com/loudsheep/TheCDTroll/blob/master/Resources/img1.jpg)

- **Listening on addresses** - shows your network card IP addresses on which app is listening for commands
- **Respond to own commands** - by default when you send a command your PC also executed it. Uncheck to disable
- **Hide app** - hides app to system tray
- **Minimaize** - as name suggests
- **Close app** - doesn't actually close the app but hides it system tray (to close app right click on icon in tray and press **exit**)
- **Send** - opens another window to send commands

## How to send command
![img3](https://github.com/loudsheep/TheCDTroll/blob/master/Resources/img2.jpg)

1. Select an IP address used for sending command
2. Enter a **command**
3. **Send to network**

Available commands:
1. **open** - if PC has CD tray then it opens
2. **close** - if PC has CD tray and it can close by itself then it closes
3. **rickroll** - hehe
4. **start** - starts a process by name after space, when multiple names given every will run. For example command **start cmd cmd cmd** opens 3 cmd windows
5. **msg**, parameters:
    - mode: **-error**, **-warrning**, **-info**, **-question**
    - -t - title. Enter a message title after that. Does not support spaces
    - after parameters enter actual message content
