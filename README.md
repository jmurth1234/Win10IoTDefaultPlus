IoT Core Default App Plus
===============

![Screenshot of the app](https://images.rymate.co.uk/images/35eV2Wh.png)

## What is this?

This is a fork of the default startup app on the Windows 10 IoT that adds an app 
launcher as a SplitView, allowing the user to launch applications on the device.

I've also added a built in browser feature, which additionally supports the 
basic HTTP authentication needed to access the built in settings panel of Win10 
IoT

## How to install?

Clone this repository on a Windows computer with VS 2015 and all the necessary
stuff for Windows 10 IoT installed. 

Once downloaded, open the sln in Visual Studio, then deploy this app to your 
remote device.

You can also install this on your computer, however it's a bit pointless as it 
just runs in a window.

Note: I have tested this on the latest insider preview (Build 14986) and the 
"Anniversary Update" (Build 14393) and both seem to run fine, however the extra 
Cortana features do not work on Build 14393.

## Additional resources
* [Documentation for the MS example](https://developer.microsoft.com/en-us/windows/iot/samples/iotdefaultapp) 
* [The original version of this project](https://github.com/ms-iot/samples/tree/develop/IoTCoreDefaultApp)
* [Windows 10 IoT Core home page](https://developer.microsoft.com/en-us/windows/iot/)

