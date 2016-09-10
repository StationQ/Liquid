// Copyright (c) 2015,2016 Microsoft Corporation

#if INTERACTIVE
#r @"/home/andru/Liquid/bin/Liquid1.dll"
#r @"blackBox.dll"                 
#else
namespace Microsoft.Research.Liquid // Tell the compiler our namespace
#endif

open System                         // Open any support libraries

open Microsoft.Research.Liquid      // Get necessary Liquid libraries
open Util                           // General utilites
open Operations                     // Basic gates and operations
open Tests                          // Just gets us the RenderTest call for dumping files

module Script =                     // The script module allows for incremental loading

    /// <summary>
    /// Performs an arbitrary rotation around X. 
    /// </summary>
    /// <param name="theta">Angle to rotate by</param>
    /// <param name="qs">The head qubit of this list is operated on.</param>
    let rotX (theta:float) (qs:Qubits) =
        let gate (theta:float) =
            let nam     = "Rx" + theta.ToString("F2")
            new Gate(
                Name    = nam,
                Help    = sprintf "Rotate in X by: %f" theta,
                Mat     = (
                    let phi     = theta / 2.0
                    let c       = Math.Cos phi
                    let s       = Math.Sin phi
                    CSMat(2,[0,0,c,0.;0,1,0.,s;1,0,0.,s;1,1,c,0.])),
                Draw    = "\\gate{" + nam + "}"
                )
        (gate theta).Run qs

    /// <summary>
    /// Helper function to convert a number to binary (as a list of booleans). 
    /// </summary>
    /// <param name="num">Number to convert to binary</param>
    /// <param name="numBits">Number of bits in the representation</param>
    let numToBools (num:int) (numBits:int) =
        let mutable tempNum = num 
        let mutable lst = []
        for i in 1..numBits do
            lst <- [(tempNum % 2) = 1] @ lst
            tempNum <- tempNum / 2 
        lst

    /// <summary>
    /// Helper function to print a number stored in binary from qubits 
    /// </summary>
    /// <param name="qs">Qubits (unmeasured) representing number in binary</param>
    let printFromBinary (qs:Qubits) =
        M >< qs
        let mutable res = 0
        let mutable pow = 1
        for i in (qs.Length - 1) .. -1 .. 0 do
            if (qs.[i].Bit.v = 1) then
                res <- res + pow
            pow <- pow <<< 1
        show "Result = %d" res

    /// <summary>
    /// Function from Lab_1 which flips the phase of the |00..0> state (leaving other basis states unchanged)
    /// </summary>
    /// <param name="qs">Qubits</param>
    let phaseFlip (qs:Qubits) =
        let gate =
            let nam     = "Phlip"
            new Gate(
                Name    = nam,
                Help    = sprintf "Flip phase of |00..0> state",
                Mat     = (
                    let n = qs.Length
                    let pow2n = 1 <<< n
                    let mutable lst = [(0,0,-1.0,0.0)]
                    for i in 1 .. (pow2n - 1) do
                        lst <- lst @ [(i,i,1.0,0.0)]
                    CSMat(pow2n, lst)),
                Draw    = "\\gate{" + nam + "}"
                )
        gate.Run qs

    /// <summary>
    /// Same as phaseFlip but flips the phase of a particulare state |x>
    /// </summary>
    /// <param name="qs">Qubits</param>
    let xPhaseFlip (x:int) (qs:Qubits) =
        let lst = numToBools x (qs.Length)
        for i in 0 .. (qs.Length - 1) do
            if (lst.[i]) then
                X [qs.[i]]
        phaseFlip qs
        for i in 0 .. (qs.Length - 1) do
            if (lst.[i]) then
                X [qs.[i]]

//////////////////////////////////////////////////////////////////////////////////////////
// Your code starts here
    let groverIteration (oracle:Qubits -> unit) (qs:Qubits) =
        oracle qs
        H >< qs
        phaseFlip qs
        H >< qs

    let grover (oracle:Qubits -> unit) (qs:Qubits) =
        let n = (1 <<< ((qs.Length) / 2))
        H >< qs
        for i in 1 .. n do
            groverIteration oracle qs

    let swapTest (qs:Qubits) =
        let cSWAP = (Cgate SWAP)
        H [qs.[2]]
        cSWAP [qs.[2]; qs.[0]; qs.[1]]
        H [qs.[2]]
        M [qs.[2]]
        if (qs.[2].Bit.v = 0) then
            show "Equal"
        else
            show "Different"
///////////////////////////////////////////


    // program entry point
    [<LQD>]
    let Lab_2()    =
        let k = Ket(11)
        let qs = k.Qubits
        let numRuns = 20
        let qcirc = Circuit.Compile (grover BlackBox.bigOracle) qs
        let qcirc = qcirc.GrowGates(k)
        for i in 1 .. numRuns do
            let qs = k.Reset(11)
            qcirc.Run qs
            printFromBinary qs

#if INTERACTIVE
do Script.Lab_2()        // If interactive, then run the routine automatically
#endif
