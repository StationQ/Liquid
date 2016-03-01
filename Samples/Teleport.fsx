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
open Tests                          // Just gets us the RenderTest call for dumping files

module Script =                     // The script module allows for incremental loading

    // Define an EPR function
    let EPR (qs:Qubits) = 
        H qs; CNOT qs

    let teleport (qs:Qubits) =
        let q0,q1,q2    = qs.[0],qs.[1],qs.[2]      // Extract 3 qubits

        // Give names to the first three qubits
        LabelL "Src" [q0]
        LabelL "\\ket{0}" [q1]
        LabelL "\\ket{0}" [q2]

        EPR[q1;q2]; CNOT qs; H qs               // EPR 1,2, then CNOT 0,1 and H 0
        M[q1]; BC X [q1;q2]                     // Conditionally apply X
        M[q0]; BC Z [q0;q2]                     // Conditionally apply Z
        LabelR "Dest" [q2]                      // Label output

    let teleportRun (teleF:Qubits->unit) (cnt:int) =
        show "============ TELEPORT ============="
        let k   = Ket()

        // Get a random number between -1.0 and +1.0
        let rVal =
            let rnd = Random(DateTime.Now.Ticks |> int)
            fun () -> 2.0 * rnd.NextDouble() - 1.0

        for i in 0..(cnt-1) do
            let qs      = k.Reset(3)
            let q0      = qs.[0]

            // Force the first qubit into a random location on the Bloch sphere
            q0.StateSet(rVal(),rVal(),rVal(),rVal())

            show "Initial State: %O" q0

            let _   = teleF qs

            show "Final   State: %O (bits:%d%d)" qs.[2] (q0.Bit.v) (qs.[1].Bit.v)

        show "=================================="
        show ""

    [<LQD>]
    let Teleport()    = 
        // Run directly
        teleportRun teleport 10

        // Look up the teleport circuit
        let ket     = Ket(3)
        let circ    = Circuit.Compile teleport ket.Qubits

        let circ2   = circ.Fold()
        circ2.Dump()

        // Don't do render's if we're really running under HPC (wasn't portable)
        if true then 
            RenderTest "Teleport" circ ket

        // Run the circuit
        let teleF (qs:Qubits) = circ.Run qs
        teleportRun teleF 10

        // Maked a wrapped gate out of teleport
        let teleGate    =
            let gate (qs:Qubits) =
                new Gate(
                    Qubits  = qs.Length,
                    Name    = "teleGate",
                    Op      = WrapOp teleport
                )
            fun (qs:Qubits) -> (gate qs).Run qs

        let ket     = Ket(3)
        let circ    = Circuit.Compile teleGate ket.Qubits
        let circ2   = circ.GrowGates(ket)

        show "Original circuit gates: %d" (circ.GateCount())
        show "   Grown circuit gates: %d" (circ2.GateCount())
        show "Original circuit:"
        circ.Dump()
        show "Grown circuit:"
        circ2.Dump()

        let teleF (qs:Qubits) = circ2.Run qs
        teleportRun teleF 10


#if INTERACTIVE
do Script.Teleport()        // If interactive, then run the routine automatically
#endif
