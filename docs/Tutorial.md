# Workshop tutorial flow

This outline may be used to help follow along with the [tutorial video](http://research.microsoft.com/apps/video/default.aspx?id=258279).

## Before the tutorial

### Install Required Software

* Install TexWorks/MikTeX 
* Install Notepad++ 
* Install [Visual Studio Community Edition](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx):
  * Custom install 
  * Programming languages: `Visual F#` (uncheck anything else) 
  * Will probably take ~20 minutes (your mileage may vary). 

### Install LIQ<i>Ui</i>|&#x232A; (may be done while VS is installing)

* Download `Liquid-master.zip` from [GitHub](https://github.com/msr-quarc/Liquid/archive/master.zip)
* Open your downloads folder, right-click on `Liquid-master.zip`, and select `Extract All…`
* Extract to `C:\` 
* Rename the `c:\Liquid-Master` folder to `c:\Liquid`

### Prep Visual Studio

Once you have everything installed: 
* Open 'c:\Liquid\source\Liquid.sln'
* Answer any setup dialogs 
* Go to Solution Explorer on the right side and see if it says that Liquid is "unavailable" 
* Right click on Liquid and select "install" (to get F# which is installed on demand). 

## Tutorial

### Basics

* Show the install directory and basic structure (use File Explorer). Stress where the docs are. 
* Show how to sprout a command windows (Right click Windows -> Command Prompt) 
* Show how to get to C: and `cd \Liquid\bin` 
* Run `liquid.exe` (no params) and talk about the output. 
* Respond with a Y to the license. Run again, show the output (mention the listserv) 
* Quick overview (stress the two underscores at the beginning of each) 

### Shor

* Run `liquid __Shor(21,false)`. Explain the time at the beginning of the line and some of the output lines. Note how long it took to run 
* Run `liquid __Shor(63,false)`. Note how long it's taking and ^C it 
* Run `liquid __Shor(63,true)`. Note the run time and explain a little of the optimizations 

### Teleport

* Talk about the 3 outputs 
* Show the `Liquid.log` file 
* Show `Teleport_CN.htm` 
* Show `Teleport_CF.htm` 
* Show `Teleport_GF.htm` 
* Show `Teleport_CF.tex` 
  * Explain `LiquidTikZ.tex` and its location 
  * Run Texworks and show the output 

### Programming Intro

* Use File Explorer to bring up `Liquid.sln` in the `C:\Liquid\source` directory 
* Show the solution files 
* Open up `Main.fs` 
* Explain &lbrack;&lt;LQD&gt;&rbrack; and show 
* Show how to put `__UserSample()` in the Liquid debug properties, switch to Release and run it. 
* Create a new function that will measure a single qubit: 
```fsharp
    let qfunc (qs:Qubits) =
        M qs
```
* Update `__UserSample` to call it: 
 ```fsharp
    [<LQD>]
    let __UserSample() =
        let stats       = Array.create 2 0
        let k           = Ket(1)

        for i in 0..9999 do
            let qs      = k.Reset(1)
            qfunc qs
            let v       = qs.Head.Bit.v
            stats.[v]  <- stats.[v] + 1

        show "Measured: 0=%d, 1=%d" stats.[0] stats.[1]
```
* Compile and run and show that you get all zeros. 
* Now add a Hadamard to `qfunc` and run again: 
```fsharp
    let qfunc (qs:Qubits) =
        H qs
        M qs
```
* Explain the `rotX` gate in the same file 
* Call with: 
```fsharp
    let qfunc (qs:Qubits) =
        rotX (Math.PI/2.) qs
        M qs
```
* Show that the result is the same as for a Hadamard 
* Now call with `Math.PI/4`. The probability of 0 should now be cos&sup2;(&pi;/8) &approx; 0.85355.
* Now change to N qubits with a CNOT: 
```fsharp
    let qfunc (qs:Qubits) =
        rotX (Math.PI/4.) qs
        for q in qs.Tail do CNOT [qs.Head;q]
        M >< qs 

    [<LQD>]
    let __UserSample(n:int) =
        let stats       = Array.create 2 0
        let k           = Ket(n)

        for i in 0..9999 do
            let qs      = k.Reset(n)
            qfunc qs
            let v       = qs.Head.Bit.v
            stats.[v]  <- stats.[v] + 1
            for q in qs.Tail do
                if qs.Head.Bit <> q.Bit then
                    failwith "Different!?!?!?!?"

        show "Measured: 0=%d, 1=%d" stats.[0] stats.[1]
```
* Change Properties to run using 10 qubits 
* Compile, run, explain (note how long it took) 
* Now change to using a circuit: 
```fsharp
    [<LQD>]
    let __UserSample(n:int) =
        let stats       = Array.create 2 0
        let k           = Ket(n)
        let circ        = Circuit.Compile qfunc k.Qubits
        for i in 0..9999 do
			...
```
* Compile, run. Not really any faster (still executing the same number of gates) so add a `GrowGates()`: 
```fsharp
        let circ        = Circuit.Compile qfunc k.Qubits
        let circ        = circ.GrowGates(k)

        for i in 0..9999 do
            let qs      = k.Reset(n)
            circ.Run qs
            let v       = qs.Head.Bit.v
```
* Show that it's now n times faster 
* Generate drawings and circuit dumps: 
```fsharp
        let circ        = Circuit.Compile qfunc k.Qubits
        show "Test1:"
        circ.RenderHT("Test1")
        circ.Dump()
        let circ        = circ.GrowGates(k)
        show "Test2:"
        circ.RenderHT("Test2")
        circ.Dump()
        for i in 0..9999 do
			...
```
* Run and show the two drawings 
* Show the dump results from `Liquid.log` 

### Quantum Chemistry

* cd to `\liquid\samples`
* Run: `..\bin\Liquid.exe __Chem(H2)` 
* Talk through the `liquid.log` file 
* Run: `..\bin\Liquid.exe __Chem(H2) | find "CSV" | sort /+25` 
* Open `H2_sto3g_4.dat` and explain what it is. 
* Open `H2O_sto6g_14.dat` and describe it 
* Run: `..\bin\Liquid.exe __Chem(H2O)` 
* Talk about the output and optimizations 

### Scripts:

* Show the `h2.fsx` file and talk through it 
* Run: `..\bin\Liquid.exe /s H2.fsx Test(26) | find "CSV" | sort /+25` 
* Run: `..\bin\Liquid.exe /l H2.dll Test(26) | find "CSV" | sort /+25` 
* Show the difference with Trotterization: 
  * `..\bin\Liquid.exe /l h2.dll Trot(2) | find "CSV" | sort /+25` 
  * `..\bin\Liquid.exe /l h2.dll Trot(1024) | find "CSV" | sort /+25` 

### Docs:

* cd to `\Liquid\Help` 
* Open/walk Contents of `Liquid.pdf` 
* Open `Liquid.chm` 
* Go to Circuit 
* Describe Compile, Dump and RenderHT 
* Online API docs are also at the GitHub site 
