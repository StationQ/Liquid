## Sample code from Dave Wecker

Examples:
* [AltOutput.fs](#ALTOUTPUT): QASM output example
* [QLSA.fsx](#QLSA): Source code for the LIQ<i>Ui</i>|&#x232A; `__QLSA()` sample
* [QuAM.fsx](#QuAM) : Source code for the LIQ<i>Ui</i>|&#x232A; `__QuAM()` sample
* [SpinTest.fsx](#SpinTest) : Sample source code for the LIQ<i>Ui</i>|&#x232A; `__Ferro()` sample
* [Tableau.fsx](#Tableau) : Sample LIQ<i>Ui</i>|&#x232A; stabilizer `ShowState()` implementation using the new `Tableau` property
* [Shor_Render.7z](Shor_Render.7z): This is a collection of drawings in both HTML and LaTeX of the sub-circuits used in the **__Shor()** command

### <a name="ALTOUTPUT"></a>AltOutput.fs

I've had some requests on how to generate circuit output for other quantum lanaguages (like QASM). This file contains a complete example. If you add it to your project (ahead of `Main.fs`) you will have a new command line function `__DumpQASM()` that will output QASM source for a Teleport circuit and a QFT.

The sections of the sample are:

* Sample circuit definitions for Teleport and QFT
* `DumpQASM`: Main routine for the example
* `appWs`: append wire names to the output for a gate
* `mapName`: - placeholder to remap LIQ<i>Ui</i>|&#x232A; names to QASM names as desired
* `out`: dump out the current built line and clear it
* `outGate`: call the above routines to output a complete gate. The only non-obvious piece is the `pfx` argument that will add a `c-` to the beginning of the gate name for binary controlled gates
* `dump`: main recursive loop that outputs the circuit. The parse is pretty straight-forward with the exception of:
	* Label gates are skipped (`g.Op = GateOp.String`). You could gather all these up and then output then to QASM in a seperate pass. Not needed for this simple example.
	* `BitCon`: Binary controlled gates need to be parsed to find what they control so that we can pass the correct name to QASM. This is only a partial parse (it's possible to have cases that this code doesn't handle)... but in most cases this will be enough
* At the bottom of dump, we count up the total number of qubits (`wires`) and use that to make a qubit list for QASM.

### <a name="QLSA"></a>QLSA.fsx

There's some interest in using the Quantum Linear Systems Algorithm (QLSA) code in a course and we'e been asked 
to release the source code for this purpose. The routine is an implementation of [Harrow, Hassidim and Lloyd (HHL)](http://arxiv.org/abs/0811.3171). I can't release the exact code (since it contains implementations of unpublished research), 
but I've written a "stripped down" version that does everything the example in LIQ<i>Ui</i>|&#x232A; (`__QLSA()`) does.

Items to note:

* Change the line `#r @"\Liquid\bin\Liquid1.dll"` to point to wherever your 
LIQ<i>Ui</i>|&#x232A; install is so that IntelliSense in Visual Studio works (if you care).
* `QLSA()` is the main routine for the example. You can invoke it with:
	*  `\Liquid\bin\Liquid.exe /s QLSA.fsx QLSA()`
* `QLSA.log` shows a sample run (copy of what Liquid.log will look like when you run)
* After you run, you will have circuit drawings in `QLSA*.htm` (as well as `QLSA*.tex).
* You will also have `Redund.htm` (and `Redund.tex`) which is the detailed circuit with 
obvious redundancies removed.
* The output contains lines that have the text `CSV` on them. If you gather these up and put them in a `.CSV` file, you can use Excel to analyze the data.
* `QLSA.xlsx` is a sample Excel file created from `QLSA.log`
	* Top left is the data exctracted from the log
	* Top right is a Pivot Table that allows manipulation of the data (you can turn the filter for `Good` on and off)
	* Bottom middle is a chart showing the data where we've filtered to show only converged (`Good`) results.
* We try to converge at each value of r. If after 500 iterations we don't converge, we 
give up at that value (hence the GOOD/BAD flag on the output).

### <a name="QuAM"></a>QuAM.fsx

We've had a request to make the source for the Quantum Associative Memory implementation available. It turns
out that the required some work to expose a few of new APIs in the system (updates will be put in the
reference manual). Functionally, the provided example is idential to the one that's built into the simulator
and should give a good idea of how to implement other simliar applications.

See the paper by [Ventura and Martinez](http://arxiv.org/abs/quant-ph/9807053) for details of the approach.

You can compile and run the sample with: `\Liquid\bin\Liquid.exe /s QuAM.fsx QuAM()`

The new APIs are:

* `CSMat.Filled()` which returns a sequence of indices of filled entries in a sparse matrix.
* `CVec.NonZeros()` which returns all non-zero entries in a state vector that are above a specified threshold. This routine is already used by the `Dump()` 
function and was made public for general use.
* `Ket.Join()` allows two ket vectors (registers) to be joined together. There are other ways around this, but when I re-wrote the `QLSA()` example
I decided it would be good to expose this internal routine (since `Ket.Split()` is already exposed).

### <a name="SpinTest"></a>SpinTest.fsx

We've had a couple of requests to see an example of how Spin.Test() is implemented. This file contains a complete stand-alone version (simply called `Test()`) that is called from the command line with `Ferro()`. This will run one of the built-in examples (`__Ferro()`) with a frustrated ferromagnetic chain (one end up and the other down). 

You can compile and run the sample with: `\Liquid\bin\Liquid.exe /s SpinTest.fsx Ferro()`

### <a name="Tableau"></a>Tableau.fsx

We've been told that people would like to access the internal state of the Stabilizer simulator. Tableau is a new property that will return a tuple of n,rs,xs,zs:

* n             : Number of qubits represented
* rs[2*n+1]     : 0=+1 1=+i 2=-1 3=-i
* xs[2*n+1,ints]: X stablizer values
* zs[2*n+1,ints]: Z stablizer values

ints is the number of ints used to represent the bits (`(n >>> 5) + 1`)

`Tableau.fsx` has a complete implementation of `ShowState` and is demonstrated dumping the tableau for Teleport.

The elements of the xs, zs, and rs arrays are all mutable, so this property may also be used to set the tableau state; 
the showState function shows how stabilizers are represented in the arrays. 
Note that the contents of these arrays are not validated as they are set, so it is possible to create an invalid tableau this way.

Note: I have _not_ added the Tableau API to the docs (for now). This is a very specialized call and (I believe) it isn't of great use to most users.

You can compile and run the sample with: `\Liquid\bin\Liquid.exe /s Tableau.fsx Tableau()`
