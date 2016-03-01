# Getting Started

## Prerequisites

These directions assume you're running on a Windows environment, such as a Windows PC or using an emulator on another operating system. Please refer to additional instructions for [Linux](#Linux) and [OSX](#OSX) at the end of this file.

If you wish to run on Windows in the cloud, you can use LIQ<i>Ui</i>|&#x232A; on a Windows virtual machine.
See the [Using LIQ<i>Ui</i>|&#x232A; on Microsoft Azure](AzureGuide.md) page for directions.

### Visual Studio

Visual Studio 2015 Community Edition is free for individuals and for academic use.
It is available at https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx.

While Visual Studio is not required to run the LIQ<i>Ui</i>|&#x232A; samples, it is necessary if you want to write your own algorithms or run scripts.

Installation steps:

1. Make sure you select F# as one of the programming languages to install (everything else is optional).
2. After the installation, follow the steps below to download LIQ<i>Ui</i>|&#x232A; into c:\Liquid
3. Open the "solution file" at: C:\Liquid\source\Liquid.sln
4. After you choose your preferences (a few clicks) you'll see the "Solution Explorer" window on the right side. The Liquid project will show up as "unavailable". This is because F# is actually installed on demand. Right click on the project and one of the options will be to install F#.
5. After F# installs you're done.

### .NET 4.5.1

The .NET Framework 4.5.1 or later is required.
Installing Visual Studio 2015 will install .NET 4.6.

If you are only running the samples and aren't installing Visual Studio, you can install .NET 4.6 from http://www.microsoft.com/en-us/download/details.aspx?id=48130 if it isn't already installed.

## <a name="Downloading"></a>Downloading LIQ<i>Ui</i>|&#x232A;

We recommend placing the download into a folder named c:\Liquid.
If you choose a different folder, you will need to create a c:\Liquid folder and copy LiquidTikZ.tex from your LIQ<i>Ui</i>|&#x232A; folder into c:\Liquid.
The samples and directions below assume that you have installed into the c:\Liquid directory.

There are three ways to download LIQ<i>Ui</i>|&#x232A;:

* Click on the button on the main project page to the right labeled **Download ZIP**. This will download LIQ<i>Ui</i>|&#x232A; and its supporting files to your system in a file named Liquid-master.zip, which you may then extract to a folder on your system. If you extract into C:\  you will now have a root folder named Liquid-master. Rename the folder to C:\Liquid and you're done.
* If you have a GitHub command-line client installed, you may instead change directory to the c:\Liquid directory and use the command:
```
git clone https://github.com/msr-quarc/Liquid
```
* If you have the GitHub desktop application installed, you may click on the button on the main project page to the right labeled **Clone in Desktop**.

The first time you run LIQ<i>Ui</i>|&#x232A;, you may get a security warning telling you that the program was downloaded from another system and may not be safe.
Once you grant LIQ<i>Ui</i>|&#x232A; permission to run by clicking the appropriate button, the warning should not recur.

## Accepting the License

You have to accept the license terms the first time you run LIQ<i>Ui</i>|&#x232A; on a new environment.

To do so, open a command window and enter the following commands:
```
c:
cd \Liquid\bin
Liquid.exe
```
You should see:
```
0:0000.0/===================================================================================================
0:0000.0/=    The Language-Integrated Quantum Operations (LIQUi|>) Simulator                               =
0:0000.0/=    is made available under license by Microsoft Corporation                                     =
0:0000.0/=    License terms may be viewed at https://github.com/msr-quarc/Liquid/blob/master/LICENSE.md    =
0:0000.0/=    Please type Y, followed by return, to indicate your acceptance of these terms                =
0:0000.0/===================================================================================================
```
If you type Y (or y) and then return, you will see:
```
0:0000.0/Thank you for accepting the license terms
```
LIQUi||> will then exit. After this, you should not be asked to accept the license again.

## Running LIQ<i>Ui</i>|&#x232A;

LIQ<i>Ui</i>|&#x232A; is a command-line program, so you need to open a command window to use it.
Once you have a command window open, enter the following commands:
```
c:
cd \Liquid\bin
Liquid.exe
```
You should see the following:
```
0:0000.0/
0:0000.0/===========================================================================================
0:0000.0/=    The Language-Integrated Quantum Operations (LIQUi|>) Simulator                       =
0:0000.0/=        Copyright (c) 2015,2016 Microsoft Corporation                                    =
0:0000.0/=        If you use LIQUi|> in your research, please follow the guidelines at             =
0:0000.0/=        https://github.com/msr-quarc/Liquid for citing LIQUi|> in your publications.     =
0:0000.0/===========================================================================================
0:0000.0/
0:0000.0/
0:0000.0/TESTS (all start with two underscores):
0:0000.0/   __Big()             Try to run large entanglement tests (16 through 33 qubits)
0:0000.0/   __Chem(m)           Solve Ground State for molecule m (e.g., H2O)
0:0000.0/   __ChemFull(...)     See QChem docs for all the arguments
0:0000.0/   __Correct()         Use 15 qubits+random circs to test teleport
0:0000.0/   __Entangle1(cnt)    Run n qubit entanglement circuit (for timing purposes)
0:0000.0/   __Entangle2(cnt)    Entangle1 with compiled and optimized circuits
0:0000.0/   __Entangles()       Draw and run 100 instances of 16 qubit entanglement test
0:0000.0/   __EntEnt()          Entanglement entropy test
0:0000.0/   __EIGS()            Check eigevalues using ARPACK
0:0000.0/   __EPR()             Draw EPR circuit (.htm and .tex files)
0:0000.0/   __Ferro(false,true) Test ferro magnetic coupling with true=full, true=runonce
0:0000.0/   __JointCNOT()       Run CNOTs defined by Joint measurements
0:0000.0/   __Noise1(d,i,p)     d=# of idle gates, i=iters, p=probOfNoise
0:0000.0/   __NoiseAmp()        Amplitude damping (non-unitary) noise
0:0000.0/   __NoiseTele(S,i,p)  Noise on Teleport S=doSteane? i=iters p=prob
0:0000.0/   __QECC()            Test teleport with errors and Steane7 code (gen drawing)
0:0000.0/   __QFTbench()        Benchmark QFT used in Shor (func,circ,optimized)
0:0000.0/   __QLSA()            Test of HHL linear equation solver
0:0000.0/   __QuAM()            Quantum Associative Memory
0:0000.0/   __QWalk(typ)        Walk tiny,tree,graph or RMat file with graph information
0:0000.0/   __Ramsey33()        Try to find a Ramsey(3,3) solution
0:0000.0/   __SG()              Test spin glass model
0:0000.0/   __Shor(N,true)      Factor N using Shor's algo false=direct true=optimized
0:0000.0/   __show("str")       Test routine to echo str and then exit
0:0000.0/   __Steane7()         Test basic error injection in Steane7 code
0:0000.0/   __Teleport()        Draw and run original, circuit and grown versions
0:0000.0/   __TSP(5)            Try to find a Traveling Salesman soltion for 5 to 8 cities
0:0000.0/
0:0000.0/Liquid Usage:  Liquid [/switch...] function
0:0000.0/    Enclose multi-word arguments in double quotes
0:0000.0/
0:0000.0/Arguments (precede with / or -):
0:0000.0/
0:0000.0/   Switch     Default              Purpose
0:0000.0/   ------     -------------------- ------------------------
0:0000.0/    /log      Liquid.log           Output log file name path
0:0000.0/    /la       Unset                Append to old log files (otherwise erase)
0:0000.0/
0:0000.0/    /s        ""                   Compile and load script file
0:0000.0/    /l        ""                   Load compiled script file
0:0000.0/
0:0000.0/    /reg      ""                   Register this copy of LIQ<i>Ui</i>|&#x232A;
0:0000.0/
0:0000.0/ Final arg is the function to call:
0:0000.0/   function(pars,...)
0:0000.0/
0:0000.0/============================================
0:0000.0/
0:0000.0/!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
0:0000.0/!!! ERROR: Need to provide at least one argument
0:0000.0/!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
```

if you then type (note that there are **two** underscores):
```
Liquid.exe __Entangle1(10)
```
you should see:
```
0:0000.0/
0:0000.0/===========================================================================================
0:0000.0/=    The Language-Integrated Quantum Operations (LIQUi|>) Simulator                       =
0:0000.0/=        Copyright (c) 2015,2016 Microsoft Corporation                                    =
0:0000.0/=        If you use LIQUi|> in your research, please follow the guidelines at             =
0:0000.0/=        https://github.com/msr-quarc/Liquid for citing LIQUi|> in your publications.     =
0:0000.0/===========================================================================================
0:0000.0/
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

## BLAS (optional)

If you want to use the quantum random walk sample or compute state vector entanglement entropies,
you will need a BLAS library (note: BLAS library loading currently only works on Windows).

**If you don't use these functions, then LIQ<i>Ui</i>|&#x232A; will work fine with no BLAS implementation available.
The BLAS library is dynamically loaded before use, and is not required for LIQ<i>Ui</i>|&#x232A; to operate.**

We have tested with OpenBLAS, which is available from http://www.openblas.net/.

OpenBLAS requires two additional DLLs, libgfortran-3.dll and libquadmath-0.dll.
These are most easily obtained as part of the MinGW-w64 package, available at http://mingw-w64.org/doku.php.
You will need to build the FORTRAN compiler to create these DLLs.

There are some helpful hints at http://icl.cs.utk.edu/lapack-for-windows/lapack/index.html and at http://www.r-bloggers.com/an-openblas-based-rblas-for-windows-64-step-by-step/.

LIQ<i>Ui</i>|&#x232A; looks for a library named "libopenblas.dll" in either the same directory as the LIQ<i>Ui</i>|&#x232A; executable,
or in a directory on your PATH, or in the C:\Liquid\bin directory (if it exists).
If you are using OpenBLAS, the other two libraries should be placed in the same directory as libopenblas.dll.

If you are using a different BLAS implementation, you will need to rename the DLL to libopenblas.dll and place it,
along with any required supporting DLLs, into one of the valid directories.

## <a name="Linux"></a>Linux

1. Make sure you remove any previous installations of mono or fsharp on your machine to avoid incompatibilities.

1. Depending on your version of Linux you can get [mono](http://www.mono-project.com/docs/getting-started/install/linux/#debian-ubuntu-and-derivatives) and [fsharp](http://fsharp.org/use/linux/) installation instructions. However for Ubuntu (we installed on 14.04) the following sufficed (we also tested on Debian 8.2):
	
	```
	$ sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
	$ echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
	$ sudo apt-get update
	$ sudo apt-get install mono-complete fsharp
	```
    
1. Follow the instructions earlier on this page for [Downloading LIQ<i>Ui</i>|&#x232A;](#Downloading) and unpack the file into your home directory (~/Liquid-master) and then copy it up to the root tree:
	
    ```
    $ cd ~/Liquid-master
    $ sudo mkdir /Liquid
    $ sudo chown $USER /Liquid
    $ cp * /Liquid --recursive
    ```
    We put it in the root just so rendering will work (as expalined elsewhere). It's 	not strictly necessary (you could run completely out of your home directory instead).
    
1. Go to the downloaded /Liquid/linux directory and see that the software runs:
    
    ```
    $ cd /Liquid/linux
    $ mono Liquid.exe
    ```
    
1. This will run LIQ<i>Ui</i>|&#x232A; and if all is well, will ask for you to accept the license. Type a **Y** (followed by Enter) to accept. Now try running a simulation:
    
    ```
    mono Liquid.exe "__Teleport()"
    ```
    
    This should run the Teleport test and give a bunch of output. Note that you need quotes (single or double) around the command since it has parenthesis which need to be escaped on Linux (you can also use backslash in front of each paren).

    You can now run all the built-in examples. The next step is to get the IDE running so that you can write your own code.

1. Install the IDE (monodevelop):
    
    ```
    sudo apt-get install monodevelop
    ```
    
1. Add F# support to monodevelop via the [add-in](https://github.com/fsharp/xamarin-monodevelop-fsharp-addin):
	
    - Run *monodevelop*
	- Tools -> Add-in Manager -> Gallery -> Language Bindings -> F# Language Binding -> Install
	
1. You can now open the solution file (**/Liquid/linux/Liquid.sln**):
    
    ```
    $ cd /Liquid/linux
    $ monodevelop Liquid.sln
    ```
    
1. Test building and running by pressing Ctrl-F5 (or selecting Run -> Start Without Debugging). The output for Teleport should be in a pop-window or in "Application Output" at the bottom right of the IDE window.

1. Now would be a good time to watch the [tutorial video](http://research.microsoft.com/apps/video/default.aspx?id=258279) and learn how to write and simulate your own quantum circuits.

## <a name="OSX"></a>OSX

These instructions were originally written and tested on OSX El Capitan 10.11.2 by vsinha (*thanks!* from the LIQ<i>Ui</i>|&#x232A; team). Latest update was tested on 10.11.3.

1. Install mono and [F#](http://fsharp.org/use/mac/) for OSX [from here](http://www.mono-project.com/download/), we've tested the 'universal' version.
2. Download the Zip file from the LIQ<i>Ui</i>|&#x232A; [git repository](https://github.com/msr-quarc/Liquid).
3. You'll find `Liquid-master` in your Downloads, open the folder.
4. Type `Command-A` `Command-C` to select all the files and copy them.
5. Type `Shift-Command-G` and then a slash (`/`) to go to the root.
6. Type `Shift-Command-N` to create a new folder (you will have to enter your admin password). Then create the `Liquid` folder.
7. Double click on the `Liquid` folder to open it and then type `Command-V` to paste the contents of the downloaded `Liquid-master` tree into `\Liquid`.
8. To check that the simulator runs, start a `Terminal` window (`Command-space Terminal`) and type the following:
``` 
cd /Liquid/linux 
mono Liquid.exe 
```
If all is well, Liquid will ask you to accept its license. (Type `y` followed by Enter to accept.)

Now try running a simulation:
```
mono Liquid.exe "__Teleport()"
```
This will run the Teleport test and give a bunch of output. 
>Note that you need quotes (single or double) around the command since it has parenthesis which need to be escaped on OSX and Linux (you can also use backslash in front of each paren).

You can now run all the built-in examples! 

The next step is to get the IDE running so that you can write your own code.

1. MonoDevelop has been rebranded as Xamarin Studio. You can download and install Xamarin Studio from its [website](http://www.monodevelop.com/download/).

2. Add F# support via the [add-in](https://github.com/fsharp/xamarin-monodevelop-fsharp-addin). 
  - open Xamarin Studio
  - Xamarin Studio > Add-in Manager > Language Bindings > F# Language Binding > Install
  >The F# plugin may be installed already (if it is you'll see it under Language bindings / F# Language Binding).

3. You can now open the solution file **(/Liquid/linux/Liquid.sln)** 4. Test building and running by pressing the "Play" button in the top right, pressing CMD+ENTER, or selecting Run -> Start Without Debugging. The output for Teleport should be in a pop-window or in "Application Output" at the bottom right of the IDE window.
5. Congrats! If you've made it to this step you're ready to start developing quantum circuits, pat yourself on the back.
6. Now would be a good time to watch the [tutorial video](http://research.microsoft.com/apps/video/default.aspx?id=258279) and learn how to write and simulate your own quantum circuits.

##### Troubleshooting & Helpful Link(s)
- [Installing F# on OSX](http://fsharp.org/use/mac/)
