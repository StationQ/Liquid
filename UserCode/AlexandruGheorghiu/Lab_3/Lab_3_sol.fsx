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
    /// Performs an arbitrary rotation around X.
    /// </summary>
    /// <param name="theta">Angle to rotate by</param>
    /// <param name="qs">The head qubit of this list is operated on.</param>
    let rotY (theta:float) (qs:Qubits) =
        let gate (theta:float) =
            let nam     = "Ry" + theta.ToString("F2")
            new Gate(
                Name    = nam,
                Help    = sprintf "Rotate in Y by: %f" theta,
                Mat     = (
                    let phi     = theta / 2.0
                    let c       = Math.Cos phi
                    let s       = Math.Sin phi
                    CSMat(2,[0,0,c,0.;0,1,s,0.;1,0,-s,0.;1,1,c,0.])),                    
                Draw    = "\\gate{" + nam + "}"
                )
        (gate theta).Run qs

    // This operation creates the specialKet state (see below) from the |00...0> state.
    // WARNING! Do not use this function for anything else! It doesn't implement a unitary operation!
    let silly (qs:Qubits) =
        let gate =
            let nam     = "Silly"
            new Gate(
                Name    = nam,
                Help    = sprintf "Nothing useful",
                Mat     = (
                    let l2 = sqrt(1./4. + 1./(4.*sqrt(2.)))
                    let l3 = sqrt(1./4. - 1./(4.*sqrt(2.)))
                    let n1 = 1. / sqrt(2. + 2.*(sqrt(2.) - 1.)*(sqrt(2.) - 1.))
                    let n2 = 1. / sqrt(2. + 2.*(sqrt(2.) + 1.)*(sqrt(2.) + 1.))
                    CSMat(16,[0,0,0.5,0.;12,0,0.5,0.;1,0,-l2 * n1,0.;5,0,l2 * n1 * (-1. + sqrt(2.)),0.;9,0,l2 * n1 * (-1. + sqrt(2.)),0.;
                    13,0,l2 * n1,0.;2,0,-l3 * n2,0.;6,0,l3 * n2 * (-1. - sqrt(2.)),0.;10,0,l3 * n2 * (-1. - sqrt(2.)),0.;14,0,l3 * n2,0.;])),
                Draw    = "\\gate{" + nam + "}"
                )
        gate.Run qs

    // State designed to produce correlations between Alice and Bob's measurements in the E91 protocol but also correlate with
    // a third party (Eve) who is trying to learn Alice and Bob's outcomes.
    let specialKet =
        let k = Ket(4)
        let qs = k.Qubits
        silly qs
        k

// Additional states to test in E91
    // State which is close to a perfect Bell pair (create a Bell pair and X rotate first qubit by pi/3)
    let almostBell1 =
        let k = Ket(2)
        let qs = k.Qubits
        H qs
        CNOT qs
        rotX (Math.PI/3.0) qs
        k

    // State which is close to a perfect Bell pair (create a Bell pair and X rotate first qubit by pi/4)
    let almostBell2 =
        let k = Ket(2)
        let qs = k.Qubits
        H qs
        CNOT qs
        rotX (Math.PI/4.0) qs
        k

    // State which is close to a 3 qubit GHZ state (|000>+|111>)/sqrt(2), having first qubit X rotated by pi/10
    let almostGHZ =
        let k = Ket(3)
        let qs = k.Qubits
        H qs
        CNOT qs
        CNOT [qs.[0]; qs.[2]]        
        rotX (Math.PI/10.0) qs
        k


// Implement your functions here
///////////////////////////////////////////

    // Teleportation circuit (first qubit is the one to be teleported, while the other two qubits form a Bell pair)
    let teleport (qs:Qubits) =
        H [qs.[2]]
        CNOT [qs.[2]; qs.[1]]
        CNOT [qs.[0]; qs.[1]]
        H [qs.[0]]
        M [qs.[0]]
        M [qs.[1]]
        let clCX = (BC X)
        let clCZ = (BC Z)
        clCX [qs.[1]; qs.[2]]           
        clCZ [qs.[0]; qs.[2]]        


    // Z basis measurement
    let Mz (qs:Qubits) =
        M qs

    // X basis measurement (Default measurement axis on Bloch sphere is Z, so we rotate by pi/2 around Y axis to get the X axis)
    let Mx (qs:Qubits) =
        rotY (Math.PI/2.0) qs
        M qs

    // (X+Z)/sqrt(2) basis measurement (rotate measurement axis from Z to X+Z, by rotating by pi/4 around Y axis)
    let Mxpz (qs:Qubits) =
        rotY (Math.PI/4.0) qs
        M qs

    // (X-Z)/sqrt(2) basis measurement (rotate measurement axis from Z to X-Z, by rotating by 3pi/4 around Y axis)
    let Mxmz (qs:Qubits) =
        rotY (3.0 * Math.PI/4.0) qs
        M qs

    /// <summary>
    /// Testing the CHSH game with Alice and Bob. The game will have n rounds, each time Alice and Bob share the state k.
    /// Depending on their inputs, Alice's outcome is the result of measuring either in the X or Z basis, whereas Bob
    /// will measure either in the (X+Z)/sqrt(2) or (X-Z)/sqrt(2) basis.
    /// Display win rate (number of wins over number of rounds) for Alice and Bob. They win if the AND of their inputs equals the XOR of their outputs
    /// </summary>
    /// <param name="k">Shared state of Alice and Bob in each round (Alice has first qubit, Bob has second qubit)</param>
    /// <param name="n">Number of rounds</param>
    let chshGame (k:Ket) (n:int) =
        let aliceMeasurements = [Mx; Mz]
        let bobMeasurements = [Mxpz; Mxmz]
        let rand = System.Random()
        let mutable win = 0
        for i in 1 .. n do
            let newK = k.Copy()
            let qs = newK.Qubits
            let x = rand.Next(0, 2)
            let y = rand.Next(0, 2)            
            let aliceM = aliceMeasurements.[x]
            let bobM = bobMeasurements.[y]
            aliceM [qs.[0]]
            bobM [qs.[1]]
            let a = qs.[0].Bit.v
            let b = qs.[1].Bit.v 
            if (x * y = a ^^^ b) then
                win <- win + 1
        show "Win rate: %f" ((float win) / (float n))

    /// <summary>
    /// Testing the Ekert '91 protocol.
    /// </summary>
    /// <param name="k">Shared state of Alice and Bob in each round (Alice has first qubit, Bob has second qubit)</param>
    /// <param name="n">Number of rounds</param>
    let E91 (k:Ket) (n:int) =
        let aliceMeasurements = [Mx; Mz; Mxpz]
        let bobMeasurements = [Mxpz; Mxmz; Mz]
        let rand = System.Random()
        let mutable win = 0
        let mutable key = []
        let mutable numTests = 0
        let mutable difs = 0
        let mutable newK = k.Copy()
        for i in 1 .. n do
            (*
            // Use this to randomly change the shared state of Alice and Bob
            if (rand.Next(0, 2) = 0) then
                newK <- specialKet.Copy()
            else
                newK <- k.Copy()
            *)
            newK <- k.Copy()
            let qs = newK.Qubits
            let x = rand.Next(0, 3)
            let y = rand.Next(0, 3)            
            let aliceM = aliceMeasurements.[x]
            let bobM = bobMeasurements.[y]
            aliceM [qs.[0]]
            bobM [qs.[1]]
            let a = qs.[0].Bit.v
            let b = qs.[1].Bit.v
            // Case where Alice and Bob measure in the same basis and should get matching outcomes
            if ((x = 1 && y = 2) || (x = 2 && y = 0)) then
                if (a = b) then // matching outcomes added to the shared key
                    key <- a :: key
                else // count number of non-matching outcomes
                    difs <- difs + 1
            else // Case where Alice and Bob measure in different basis, consider CHSH game and compute win rate
                if (x < 2 && y < 2) then
                    numTests <- numTests + 1 
                    if (x * y = a ^^^ b) then
                        win <- win + 1
        show "Win rate: %f" (100. * (float win) / (float numTests))
        printf "Key: "
        for i in 0 .. 100 do
            printf "%d" key.[i]
        printf "\n"
        show "Different %d" difs
        show "Same %d" key.Length        
    
///////////////////////////////////////////


    // program entry point
    [<LQD>]
    let Lab_3()    =
        show "Lab 3"
        let k = Ket(2)
        let qs = k.Qubits
        // Test E91 with perfect Bell state
        H qs
        CNOT qs
        E91 k 60000

#if INTERACTIVE
do Script.Lab_3()        // If interactive, then run the routine automatically
#endif
