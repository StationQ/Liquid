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
// warmups
    let warmup1a (q:Qubit) =
        H [q]
        X [q]
        H [q]

    let warmup1b (q:Qubit) =
        H [q]
        Z [q]
        H [q]

    let warmup2 (qs:Qubits) =
        H [qs.[1]]
        CNOT qs
        H [qs.[1]]

    let warmup3 (qs:Qubits) =
        for i in 0 .. (qs.Length - 1) do
            H [qs.[i]]
        // alternatively H >< qs

    let warmup4 (qs:Qubits) =
        Z >< qs

// circuits
    let circuit1 (qs:Qubits) =
        H qs
        CZ qs
        H qs
        CNOT qs

    let circuit2 (qs:Qubits) =
        CNOT qs
        CNOT [qs.[1]; qs.[0]]
        CNOT qs

    let circuit3 (qs:Qubits) =
        H [qs.[2]]
        CNOT [qs.[2]; qs.[1]]
        CNOT [qs.[0]; qs.[1]]
        H [qs.[0]]
        M [qs.[0]]
        M [qs.[1]]
        // create the classically controlled gates
        let clCX = (BC X)
        let clCZ = (BC Z)
        clCX [qs.[1]; qs.[2]]           
        clCZ [qs.[0]; qs.[2]]     

// problems
    let problem1 (qs:Qubits) =
        let n = qs.Length
        for i in 0..(n - 2) do
            CNOT [qs.[i]; qs.[n-1]]

    let problem2 (qs:Qubits) =
        let n = qs.Length
        H >< qs
        for i in 0..(n - 2) do
            CNOT [qs.[i]; qs.[n-1]]
        H >< qs

    let problem3 (qs:Qubits) =
        let gate =
            let nam     = "Reflection"
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

    let problem4 (x:int) (qs:Qubits) =
        let lst = numToBools x (qs.Length)
        for i in 0 .. (qs.Length - 1) do
            if (lst.[i]) then
                X [qs.[i]]
        problem3 qs
        for i in 0 .. (qs.Length - 1) do
            if (lst.[i]) then
                X [qs.[i]]


///////////////////////////////////////////


    // program entry point
    [<LQD>]
    let Lab_1()    =
//        show "Lab 1"
//        (ex1 1000)          // Test ex1

        // Generating the circuit for ex5
        let k = Ket(4)
        let qs = k.Qubits
        X [qs.[0]]
        show "State before: %s" (k.ToString())
        (problem4 8 qs)
        show "State after: %s" (k.ToString())
//        let qcirc = Circuit.Compile problem1 qs
//        qcirc.RenderHT("Parity")


#if INTERACTIVE
do Script.Lab_1()        // If interactive, then run the routine automatically
#endif
