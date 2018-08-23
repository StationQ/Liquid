# The Language-Integrated Quantum Operations (LIQ<i>Ui</i>|&#x232A;) simulator

## News

__2017/12/11 Microsoft Quantum Development Kit (Preview) released__
 
We are very happy to announce the availability of the [Microsoft Quantum Development Kit (Preview)](https://www.microsoft.com/en-us/quantum/development-kit). There is a nice [blog entry](https://blogs.microsoft.com/ai/2017/12/11/future-quantum-microsoft-releases-free-preview-quantum-development-kit/) for details if you'd like to read more. The release contains a new language (Q#), compiler, simulators, examples, libraries (in source form) and full documentation. There is also a video walk through showing quantum code samples as part of the [Microsoft Mechanics](https://www.youtube.com/watch?v=v7b4J2INq9c) series.

__2017/11/03 Updated Binaries__
 
The latest version of Visual Studio/.Net/FSharp (2017) is incompatible with the binaries we've previously shipped for LIQ<i>Ui</i>\|&#x232A;. All the compiled examples will work, but new code or compiling from scripts will fail. The current version on the Github site has been updated to fix this. Following the [Getting Started](http://stationq.github.io/Liquid/getting-started/) is the easiest way to re-install everything. If you already have a version of VS2017, then you can skip that piece.

__2017/09/26 Announcement of Microsoft Quantum__
 
With the [Microsoft Ignite](https://www.youtube.com/watch?v=N3csr3fXBfo) announcement of our broader quantum effort yesterday, I'd encourage members of the list to sign up for the new Microsoft quarterly [Quantum Newsletter](https://info.microsoft.com/Quantum-Computing-Newsletter-Signup.html) for the latest information on our complete software stack for quantum computing.

__2017/07/13 Re-compiled for VS 2017__
 
User Brandon Langenberg at FAU discovered that any compilations done with VS 2017 will not work with the shipped bits (since they were built with VS 2015). We have re-compiled all the binaries and have moved to F# 4.1 (FSharp.Core 4.4.1.0) along with .Net Framework 4.6.1.

__2017/06/21 Bug fixed in UserCode/DaveWecker/AltOutput.fs__

User thiennm noted that classically controlled gates were dropping a qubit in the QASM output (see the Teleport example). His suggested fix has been adopted. Thanks!

__2017/06/14 Fixed scripts on Linux and OSX__

A recent change to FSharp on Linux and OSX made it impossible to incrementaly load scripts that were compiled with `/S`. We have updated the kit so that the linux directory now contains private copies of `FSharp.Core.*` which should fix the problem. 

__2017/04/18 Drawing Shor Circuits__

We had a request for renderings of the circuits used by the **__Shor()** command. There is now a zipped file of all the circuits in both HTML and LaTeX in [UserCode/DaveWecker](UserCode/DaveWecker) as **Shor_Render.7z**

__2016/07/02 Sample implementation of Spin.Test and Stabilizer.ShowState released__

In UserCode\DaveWecker there are two new releases of sample implementations. The first shows how to implement the Spin.Test routine that's caled by the `__Ferro()` example in LIQ<i>Ui</i>\|&#x232A;. There's also a new property on stabilizers called `Tableau` which will return the current (raw) stabilizers. See the `Tableau.fsx` sample in the same directory for further information. The [README.md](UserCode/DaveWecker/README.md) file is a good place to start.

__2016/05/20 Channels/POVMs added__

We've made a major addition to the available noise models in the system by adding Channels (via Kraus operators) and Generalized Measurement (via POVMs). The Users Manual has a new
section under "Advanced Noise Models" called "Channels and POVMs", the API docs have been updated and there's a new built in example called `__Kraus`. A sample call would be: 
`__Kraus(1000,0.02,.02,true)` which would do 1000 runs with Amplitude Damping and Depolarizing noise. See the Users Manual for full details as well as the provided source code in `Samples\Kraus.fsx`.
You can also find an example of a simple quantum game (Peres) using POVMs in the `UserCode\MartinRoettler` directory.

__2016/05/16 First Quantum Challenge Winners announced!__

The winners of the first Quantum Challenge have been announced in the [Microsoft Research Blog](https://blogs.msdn.microsoft.com/msr_er/2016/05/16/microsoft-quantum-challenge-results-are-in/).
Full information about the entries may be found in our [GitHub repo](https://github.com/StationQ/Liquid/tree/master/QuantumChallenge/QC_1).

__2016/05/03 QuAM source code released__

In UserCode\DaveWecker there's a new release of source code for the QuAM example that's built in to LIQ<i>Ui</i>\|&#x232A;. See the [README.md](UserCode/DaveWecker/README.md) file in the same directory for details. We also exposed a few more APIs to help make this and other examples easier to code.

__2016/04/23 QLSA source code released__

In UserCode\DaveWecker there's a new release of source code for the QLSA example that's built in to LIQ<i>Ui</i>\|&#x232A;. See the [README.md](UserCode/DaveWecker/README.md) file in the same directory for details.

__2016/03/11 New introduction videos__

Seven new short videos have been posted to the [LIQ<i>Ui</i>\|&#x232A; Research site](http://research.microsoft.com/en-us/projects/liquid/)

__2016/03/04 Outputting circuits for other Quantum Languages__

We've uploaded sample code that shows how to print out circuits for other quantum languages. This specific example shows how to parse a circuit and generate QASM code. See [UserCode example](https://github.com/StationQ/Liquid/tree/master/UserCode/DaveWecker) from DaveWecker


As always, we encourage you to join our (low volume) mailing list (instructions below) so you will directly receive announcements like these.

## What Is LIQ<i>Ui</i>|&#x232A;?

LIQ<i>Ui</i>|&#x232A; is a simulation platform developed by the [Quantum Architectures and Computation](http://research.microsoft.com/en-us/groups/quarc/) team at Microsoft Research to aid in the exploration of quantum computation. 
LIQ<i>Ui</i>|&#x232A; stands for “Language Integrated Quantum Operations”.  
A quantum operation is usually referred to as a unitary operator (U) applied to a column state vector (also known as a ket: |>).
The “i” is just a constant scaling factor, hence the acronym.

LIQ<i>Ui</i>|&#x232A; includes three simulators: 
* A full state vector simulator that tracks the detailed evolution of the quantum state
* A stabilizer simulator based on CHP (Aaronson and Gottesman, http://arXiv.org/abs/quant-ph/0406196)
* A highly-optimized full state vector simulator for fermionic Hamiltonians

LIQ<i>Ui</i>|&#x232A; is available under a [Microsoft Research license](LICENSE.md). 

## For More Information

See the [StationQ website](http://www.StationQ.com)
and the paper, [LIQ<i>Ui</i>|&#x232A;: A Software Design Architecture and Domain-Specific Language for Quantum Computing](http://research.microsoft.com/pubs/209634/1402.4467.pdf).

To stay up to date on what we're doing with LIQ<i>Ui</i>|&#x232A;, please watch our repository and sign up for the LIQ<i>Ui</i>|&#x232A; email list.
To sign up, send an email to LISTSERV@lists.research.microsoft.com with a one-line body reading:
```
SUB Liquid-news FirstName LastName
```
Replacing FirstName and LastName with your first and last names.
If you prefer to remain anonymous, you may instead send an email containing:
```
SUB Liquid-news anonymous
```

## How To Cite LIQ<i>Ui</i>|&#x232A;

If you use LIQ<i>Ui</i>|&#x232A; in your research, please cite it as follows:
* bibTex:
```
@misc{1402.4467,
  author = {Dave Wecker and Krysta M.~Svore},
  title = {{LIQU}i|>: {A} {S}oftware {D}esign {A}rchitecture and {D}omain-{S}pecific 
            {L}anguage for {Q}uantum {C}omputing},
  year = {2014},
  eprint = {1402.4467},
  url = {arXiv:1402.4467v1}
}
```
* Text: 
```
D. Wecker and K. M. Svore, “LIQ<i>Ui</i>|&#x232A;: A Software Design Architecure and Domain-Speciﬁc 
    Language for Quantum Computing,” (2014), arXiv:1402.4467v1 [quant-ph], 
    http://arxiv.org/abs/1402.4467.
```

## What Can I Do With It?

You can use LIQ<i>Ui</i>|&#x232A; to define quantum circuits, render them into a variety of graphical formats, and execute them
using an appropriate simulator.

Some of the specific algorithms you can simulate with LIQ<i>Ui</i>|&#x232A; are:
* Simple quantum teleportation
* Shor's factoring algorithm
* Quantum chemistry: computing the ground state energy of a molecule
* Quantum error correction
* Quantum associative memory (Ventura and Martinez, http://arxiv.org/abs/quant-ph/9807053)
* Quantum linear algebra (Harrow, Hassidim, and Lloyd, http://arxiv.org/abs/0811.3171)

All of these algorithms, and many more, are included as samples with LIQ<i>Ui</i>|&#x232A;. 
A video of a recent talk  at [IQC](https://uwaterloo.ca/institute-for-quantum-computing/) on the reserach we've done with the simulator is at https://youtu.be/Q6M0ueXLTak?t=1s

## How Do I Get It?

You can download the LIQ<i>Ui</i>|&#x232A; executable, libraries, sample scripts, and documentation from this site.

See the [Getting Started](GettingStarted.md) page for directions on how to download and start using LIQ<i>Ui</i>|&#x232A;.

If you wish to use LIQ<i>Ui</i>|&#x232A; on a Windows virtual machine,
see the [Using LIQ<i>Ui</i>|&#x232A; on Microsoft Azure](AzureGuide.md) page for directions.

## How Do I Use It?

See the [users' guide](http://stationq.github.io/Liquid/docs/LIQUiD.pdf) and the [full help documentation](http://stationq.github.io/Liquid/docs/index.html).
The help may also be downloaded as a [single file](http://stationq.github.io/Liquid/docs/Liquid.chm), if desired. There is also a [tutorial video](http://research.microsoft.com/apps/video/default.aspx?id=258279) available that will take you through the basics.

We've also posted seven short videos to the [LIQ<i>Ui</i>|&#x232A; Research site](http://research.microsoft.com/en-us/projects/liquid/). These include:
* [Station Q Overview](http://research.microsoft.com/apps/video/default.aspx?id=263557)
* [LIQ<i>Ui</i>|&#x232A; Simulator History](http://research.microsoft.com/apps/video/default.aspx?id=263605)
* [The Quantum Challenge](http://research.microsoft.com/apps/video/default.aspx?id=263584)
* [LIQ<i>Ui</i>|&#x232A; Quantum Simulator QuickStart](http://research.microsoft.com/apps/video/default.aspx?id=263046)
* [LIQ<i>Ui</i>|&#x232A; Quantum Error Correction](http://research.microsoft.com/apps/video/default.aspx?id=263597)
* [LIQ<i>Ui</i>|&#x232A; Simulation Optimization](http://research.microsoft.com/apps/video/default.aspx?id=263612)
* [LIQ<i>Ui</i>|&#x232A; Quantum Chemistry](http://research.microsoft.com/apps/video/default.aspx?id=263611)

Note that this version of LIQ<i>Ui</i>|&#x232A; is limited to a maximum of 23 physical qubits for full state vector simulation.

If you run into a problem or have a question, you can [post an issue](https://github.com/StationQ/Liquid/issues).
If you have other feedback, you can contact the LIQ<i>Ui</i>|&#x232A; team at liquid@microsoft.com.

---

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
