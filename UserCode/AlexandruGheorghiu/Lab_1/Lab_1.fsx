// Copyright (c) 2015,2016 Microsoft Corporation

#if INTERACTIVE
#r @"/home/andru/Liquid/bin/Liquid1.dll"
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


// Example functions
///////////////////////////////////////////
    let ex1 (n:int) =
        let k = Ket(1)
        let mutable stats = 0
        for i in 1 .. n do
            let qs = k.Reset(1)
            H qs
            M qs
            stats <- stats + qs.[0].Bit.v
        show "Measurement statistics: 0=%d; 1=%d" (n - stats) stats

    let ex2a (q:Qubit) =
        (R 3) [q]

    let ex2b (q:Qubit) =
        rotX (Math.PI/4.) [q]

    let ex3 (qs:Qubits) =
        H qs
        CNOT qs

    let ex4 (qs:Qubits) =
        H qs
        for i in 1 .. (qs.Length - 1)  do
            CNOT [qs.[0]; qs.[i]]

    let ex5 (qs:Qubits) =
        H [qs.[0]]
        H [qs.[1]]
        CNOT qs
        H [qs.[0]]
///////////////////////////////////////////


// Implement your functions here
///////////////////////////////////////////

///////////////////////////////////////////


    // program entry point
    [<LQD>]
    let Lab_1()    =
        show "Lab 1"
        (ex1 1000)          // Test ex1

        // Generating the circuit for ex5
        let k = Ket(2)
        let qs = k.Qubits
        let qcirc = Circuit.Compile ex5 qs
        qcirc.RenderHT("Example5")


#if INTERACTIVE
do Script.Lab_1()        // If interactive, then run the routine automatically
#endif
