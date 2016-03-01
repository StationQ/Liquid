// Copyright (c) 2015,2016 Microsoft Corporation

#if INTERACTIVE
#r @"..\bin\Liquid1.dll"                 
#else
namespace Microsoft.Research.Liquid // Tell the compiler our namespace
#endif

open System                         // Open any support libraries

open Microsoft.Research.Liquid      // Get necessary Liquid libraries
open Util                           // General utilites
open Operations                     // Basic gates and operations

// Show elapsed times
type QubitTimer() =
    let sw  = 
        let sw  = Diagnostics.Stopwatch()
        show ""
        show "%8s %8s %8s %s" "Secs/Op" "S/Qubit" "Mem(GB)" "Operation"
        show "%8s %8s %8s %s" "-------" "-------" "-------" "---------"
        sw.Restart()
        sw

    member x.Show(str:string,?i:int,?reset:bool) =
        let i           = defaultArg i 1
        let reset       = defaultArg reset true
        let ps          = procStats(false)
        let secs        = float sw.ElapsedMilliseconds / 1000.0
        let spi         = secs / float i
        show "%8.3f %8.3f %8.3f %s" secs spi (float ps.privMB / 1024.) str
        if reset then sw.Restart()

module Script =                     // The script module allows for incremental loading

    [<LQD>]
    let Entangle1(entSiz:int) =
        logOpen "Liquid.log" false

        let qt      = QubitTimer()

        let ket     = Ket(entSiz)                           // Start with a full sized state vector    
        let _       = ket.Single()
        qt.Show "Created single state vector"
    
        let qs      = ket.Qubits
        H qs                                                // Hadamard the first qubit
        qt.Show "Did Hadamard"
    
        let q0  = qs.Head
        for i in 1..qs.Length-1 do
            let q   = qs.[i]
            CNOT[q0;q]                                      // Entangle all the other qubits
            let str = sprintf "  Did CNOT: %2d" i
            qt.Show(str,i,(i=qs.Length-1))
    
        M >< qs                                             // Measure all the qubits
        qt.Show("Did Measure",qs.Length)
        show ""

    [<LQD>]
    let Entangle2(entSiz:int) =
        let ops (qs:Qubits) =
            H qs
            let q0  = qs.Head
            for i in 1..qs.Length-1 do CNOT !!(qs,0,i)
            M >< qs                                         // Measure all the qubits

        let ket     = Ket(entSiz)                           // Start with a full sized state vector    
        let _       = ket.Single()
        let qs      = ket.Qubits

        let qt      = QubitTimer()
        ops qs
        qt.Show "Straight function calls"

        let qs      = ket.Reset(entSiz)
        let _       = ket.Single()
        let circ    = Circuit.Compile ops qs
        qt.Show "Compile cost"
        circ.Run qs
        qt.Show "Compiled circuit run time"

        let qs      = ket.Reset(entSiz)
        let _       = ket.Single()
        let circ2   = circ.GrowGates(ket)
        qt.Show "Optimization cost"
        circ2.Run qs
        qt.Show "Optimized circuit run time"

        show "Circuit dump (in Liquid.log):"
        circ.Dump(showLogInd)
        show "Optimized circuit:"
        circ2.Dump(showLogInd)
        
        circ.Fold().RenderHT("Entangle2raw")
        circ2.Fold().RenderHT("Entangle2opt")

#if INTERACTIVE
do 
    Script.Entangle1(10)        // If interactive, then run the routine automatically
    Script.Entangle2(10)
#endif
