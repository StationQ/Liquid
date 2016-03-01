# Using LIQ<i>Ui</i>|&#x232A; on Microsoft Azure

If you don't have access to a Windows environment, using a Microsoft Azure virtual machine (VM) is a good way to use LIQ<i>Ui</i>|&#x232A;.
These instructions walk you through creating and connecting to a VM and setting it up with LIQ<i>Ui</i>|&#x232A;.

Setting up a VM for the first time will take about 45 minutes, though you can carry on working while things happen in the background.
Once you have created a VM, restarting it usually takes 2 or 3 minutes.

## Microsoft Azure Information and Cost

An Azure basic-tier A2 instance is appropriate for most uses of LIQ<i>Ui</i>|&#x232A;.
This VM type, if located in the south central US region, costs $0.148 per hour.
At 8 hours per day for 21 weekdays in a month, this comes to $24.86 per month.
See the [Microsoft Azure pricing calculator](https://azure.microsoft.com/en-us/pricing/calculator/) for details.

A new Microsoft Azure account comes with a one month free trial for up to $100 of usage.

If you are teaching a course using LIQ<i>Ui</i>|&#x232A; and you and your students will be using LIQ<i>Ui</i>|&#x232A; on Microsoft Azure, the [Microsoft Educator Grant Program](https://www.microsoftazurepass.com/azureu) may be applicable.
It will provide Microsoft Azure funding for you and your students that will more than cover their LIQ<i>Ui</i>|&#x232A; usage.

## Prerequisites

If you are using a non-Windows computer, you will need to install a Remote Desktop Client to complete this Quick Start. 
Links to supported clients are available [here for Mac, iOS, and Android](https://technet.microsoft.com/en-us/library/dn473009.aspx). 
For Linux, you might try [FreeRDP](http://www.freerdp.com/), but we haven't tested it.

## Steps

### Create a Virtual Machine (VM)

* Get a free <a href="http://windows.microsoft.com/en-US/windows-live/sign-up-create-account-how" target="_blank">Microsoft account</a> if you don't already have one. 

TIP: You can use an existing email address for this, if you want, such as your institutional account or an existing Gmail or Yahoo! email account.

* Sign up for a free <a href="https://azure.microsoft.com/en-us/pricing/free-trial/" target="_blank">Azure trial</a> account.

* Go to the <a href="http://azure.microsoft.com/en-us/marketplace/partners/microsoft/visualstudiocommunity2015withazuresdk27onwindowsserver2012r2/" target="_blank">Visual Studio Community 2015</a> page in the Azure Virtual Machine Marketplace, and click on the "Create Virtual Machine" link. If requested, log in to the Microsoft account you set up in step 1.  This brings you to the Azure Portal.

**TIP**: If you are using a different log in for Azure to the one you are using on your machine, then you might need to ensure you have a "clean" browser window in order to sign in. On the Edge browser, click the ... on the right and select New InPrivate Window". For IE, close all windows, the shift-right click on the IE icon and choose “Run as different user”.
![Azure Portal](/img/CreateVM.jpg)

* Click on the "Create" button as indicated in the picture above. This will open the "Create VM" panel, pictured below.  
![Create VM Panel](/img/CreateVMPanel.jpg)

* Fill in a host name, user name, and password in the Create VM panel. The host name will be the name of your new VM; the user name and password will be for a new administrator account set up on the machine. The Location is the data center where your VM will reside and you should choose somewhere close to you.

**TIP**: Make a note of the name and password you choose because you will use them to log in to your VM later.

* Click on the "Pricing Tier" item in the panel which opens up the "Choose your pricing tier" panel, and click on the "View all" link to see all virtual machine options:  
![Pricing Tier Panel](/img/PricingTierPanel.jpg)

* Scroll down and click on the "A2 Basic" entry to select it, and then click on the "Select" button at the bottom of the panel. This will close the pricing tier panel. You can use a different size if you like, but the A2 Basic size is sufficient for most uses.  
![A2 Basics](/img/SelectA2Basic.jpg)

* Click on the "Create" button at the Create VM panel to create and start your virtual machine. After the creation process has finished, this will close the Create VM panel and bring you back to the Azure Portal front page. 
**TIP**: Creation could take anything from 2-20 minutes depending on load. Be patient. You can use your computer for other work while waiting.
![Click to create](/img/ClickToCreate.jpg)

* Click on the "Virtual machines (classic)" link on in the portal navigation menu. This will open the "Virtual machines (classic)" panel.  
![Portal navigation](/img/PortalNav.jpg)

* In the Virtual machines panel, click on the row with the virtual machine name you used in step 5. This will open the management panel for your VM.  
![VM Panel](/img/VMPanel.jpg)

* Wait until the Status field says "Running" (it might say Provisioning or Starting or another variation when you first navigate to this panel).
**TIP**: You might need to wait several minutes for this status.  
![Status: Running](/img/StatusRunning.jpg)

### Remote Connect to the VM

* Click on the "Connect" icon at the top of the panel:  
![Connect](/img/Connect.jpg)

* At the bottom of your browser window, you should get a notification asking you if you want to open or save an RDP file from ms.portal.azure.com. Click on the "Open" button.  
**NOTE**: If you are not on a Windows client, the next two steps may be slightly different.

* Windows will pop up a notification from Remote Desktop Connection, "The publisher of this remote connection can't be identified. Do you want to connect anyway?". Click on the "Connect" button.

* Windows will pop up a login dialog from Windows Security asking you to enter your credentials for the VM. Select "Use another account" and enter the user name and password you entered in step 5. A progress window will pop up, then that will close and another notification will appear from Remote Desktop Connection, "The identity of the remote computer cannot be verified. Do you want to connect anyway?". Click on the "Yes" button. The progress will reappear until your remote desktop session starts.

* The remote desktop session will start full screen. While your login is being set up, the screen may be black for several minutes. Once the remote desktop session starts, you may get a notification on the right-hand side of the screen asking whether you want to enable network discovery. Click on the "No" button.

### Download LIQ<i>Ui</i>|&#x232A;

* Open up a web browser on the VM by hitting the Windows key and then clicking on the IE icon. Click on the OK button to use recommended settings. Navigate to the LIQ<i>Ui</i>|&#x232A; GitHub page in the VM browser.

* Click on the button to the right labeled **Download ZIP**. This will download LIQ<i>Ui</i>|&#x232A; and its supporting files to your system in a file named Liquid-master.zip, which you may then extract to a folder on your VM. We recommend extracting into a folder named c:\Liquid; the TikZ rendering requires this path.

### Accept the License

* Follow the instructions on the [Getting Started page](GettingStarted.md#accepting-the-license) to accept the LIQ<i>Ui</i>|&#x232A; license terms.

### Run a Test

* If you don't have the command window open from the previous step, open up a command prompt by right-clicking on the Windows icon in the bottom-left corner of the VM remote desktop. Change directory to "c:\Liquid\bin" and type "Liquid.exe __Entangle1(12)" (*Note*: there are **two** underscores before the E). You should see output similar to the following:  
```
0:0000.0/
0:0000.0/===========================================================================================
0:0000.0/=    The Language-Integrated Quantum Operations (LIQUi|>) Simulator                       =
0:0000.0/=        Copyright (c) 2015,2016 Microsoft Corporation                                    =
0:0000.0/=        If you use LIQUi|> in your research, please follow the guidelines at             =
0:0000.0/=        https://github.com/msr-quarc/Liquid for citing LIQUi|> in your publications.     =
0:0000.0/===========================================================================================
0:0000.0/
0:0000.0/=============== Logging to: Liquid.log opened ================
0:0000.0/
0:0000.0/ Secs/Op  S/Qubit  Mem(GB) Operation
0:0000.0/ -------  -------  ------- ---------
0:0000.0/   0.057    0.057    0.274 Created single state vector
0:0000.0/   0.050    0.050    0.277 Did Hadamard
0:0000.0/   0.010    0.010    0.278   Did CNOT:  1
0:0000.0/   0.020    0.010    0.280   Did CNOT:  2
0:0000.0/   0.028    0.009    0.280   Did CNOT:  3
0:0000.0/   0.037    0.009    0.281   Did CNOT:  4
0:0000.0/   0.046    0.009    0.282   Did CNOT:  5
0:0000.0/   0.054    0.009    0.283   Did CNOT:  6
0:0000.0/   0.062    0.009    0.284   Did CNOT:  7
0:0000.0/   0.069    0.009    0.285   Did CNOT:  8
0:0000.0/   0.076    0.008    0.285   Did CNOT:  9
0:0000.0/   0.013    0.001    0.286 Did Measure
0:0000.0/
0:0000.0/=============== Logging to: Liquid.log closed ================
```
* To see all of the available samples, just type "Liquid.exe" at the command prompt.

**That's it -- LIQ<i>Ui</i>|&#x232A; is now installed on your virtual machine.**

### Stopping the VM

When you're done with your session, log out of your remote desktop session and stop your VM in the Azure Portal by clicking on the "Stop" icon.
TIP: That way your account won't be charged for your VM when you're not using it.

### Restarting the VM

You can restart your VM by going to the management panel and clicking on the "Start" icon again, and then on the "Connect" icon once the VM is running.

**TIP**: If you've closed the portal in your browser, you can always get to it as <a href="http://azure.com/portal" target="_blank">azure.com/portal</a>.

**TIP**: The disk image is saved when you stop and restart, so you will not need to reinstall GitHub or LIQ<i>Ui</i>|&#x232A;.

