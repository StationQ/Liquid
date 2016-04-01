## Sample code from Dave Wecker


### AltOutput.fs

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
