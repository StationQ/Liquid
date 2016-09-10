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
    /// Similar to numToBools but returns a list of qubits in a basis state (binary representation of the number) 
    /// </summary>
    /// <param name="num">Number to convert to binary</param>
    /// <param name="numBits">Number of bits in the representation</param>
    let numToQubits (num:int) (numBits:int) =
        let lst = (numToBools num numBits)
        let k = Ket(num)
        let qs = k.Qubits
        for i in 0 .. (numBits - 1) do
            X [qs.[i]]
        qs

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
    /// Naive way to check for a bomb 
    /// </summary>
    /// <param name="qs">Qubits</param>
    /// <param name="oracle">The bomb (CNOT) or dud (identity)</param>    
    let naiveBombTester (qs:Qubits) (oracle:Qubits -> unit) =
        X qs
        oracle qs
        M [qs.[1]]
        if (qs.[1].Bit.v = 1) then
            failwith "BOOM!"
        else
            show "Dud"

//////////////////////////////////////////////////////////////////////////////////////////
// Your code starts here
    let deutschJosza (qs:Qubits) (oracle:Qubits -> unit) =
        X !!(qs,qs.Length - 1)
        H >< qs
        oracle qs
        H >< qs
        M >< qs
        let mutable stats = 0
        for i in 0..(qs.Length - 2) do
            stats <- stats + qs.[i].Bit.v
        if (stats = 0) then
            show "Constant"
        else
            show "Balanced"

    let smartBombTester (qs:Qubits) (oracle:Qubits -> unit) =
        let N = 100.0
        let delta = Math.PI / N
        for i in 1.0 .. N do
            rotX delta qs
            oracle qs
            M [qs.[1]]
            if (qs.[1].Bit.v = 1) then
                failwith "BOOM!"
            qs.[1].ReAnimate(qs.[1].Bit)
        M [qs.[0]]
        if (qs.[0].Bit.v = 0) then
            show "Bomb"
        else
            show "Dud"
        

///////////////////////////////////////////


    // program entry point
    [<LQD>]
    let Lab_2a()    =
        show "Lab_2a"
        (*
        let k = Ket(10)
        let qs = k.Qubits
        deutschJosza qs BlackBox.fun3
        *)
        let k = Ket(2)
        let qs = k.Qubits
        smartBombTester qs BlackBox.candidate2

#if INTERACTIVE
do Script.Lab_2a()        // If interactive, then run the routine automatically
#endif

