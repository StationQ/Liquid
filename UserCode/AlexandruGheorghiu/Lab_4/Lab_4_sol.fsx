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
    /// Syndrome measurement for the X flip code implemented as POVM, defined by the projectors:
    /// P0 = |000><000| + |111><111|
    /// P1 = |100><100| + |011><011|
    /// P2 = |010><010| + |101><101|
    /// P3 = |001><001| + |110><110|            
    /// </summary>
    /// <param name="qs">Qubits to measure (assumed to be at least 3)</param>    
    let xflipSyndrome (qs:Qubits) =
        let gate =
            Gate.Build("xFlipSyndrome",fun () ->
                new Gate(
                    Name = "xFlipSyndrome",
                    Help = "Syndrome measurement for bit flip code",
                    Kraus = [{tag=0;mat=CSMat(8,[(0,0,1., 0.);(7,7, 1.,0.)])}
                             {tag=1;mat=CSMat(8,[(3,3,1., 0.);(4,4, 1.,0.)])}
                             {tag=2;mat=CSMat(8,[(2,2,1., 0.);(5,5, 1.,0.)])}
                             {tag=3;mat=CSMat(8,[(1,1,1., 0.);(6,6, 1.,0.)])}],
                    Draw = "\\gate{xFlipSyndrome}",
                    Op = Channel("POVM")
            ))
        gate.Run qs
        let k = qs.Head.Ket
        let outcome = k.Symbol "POVM"
        outcome                



    // Big quantum circuit involving H, CNOT, X and Z and computational basis measurements
    let bigOperation (qs:Qubits) = 
        H qs
        for i in 1 .. (qs.Length - 1)  do
            CNOT [qs.[0]; qs.[i]]
        for i in 0 .. (qs.Length - 1)  do
            if (i % 2 = 0) then
                X [qs.[i]]
            else
                Z [qs.[i]]
        for i in 0 .. (qs.Length - 2) do
            CNOT [qs.[i]; qs.[qs.Length - 1]]
        M >< qs

    // "Naive" way of running the big quantum operation
    // Simply create an initial ket state, run the circuit many times and get the measurement outcomes
    let naiveBigOperation (numQubits:int) (numRuns:int) =
        let k = Ket(numQubits)
        let mutable stats = 0
        for i in 1 .. numRuns do
            let qs = k.Reset()
            bigOperation qs
            stats <- stats + qs.[qs.Length - 1].Bit.v
        show "Number of 0's for last qubit: %d" (numRuns - stats)
        show "Number of 1's for last qubit: %d" stats        

    // Optimized way of running the big quantum operation
    // Use Liquid's GrowGates function to optimize the quantum circuit before running it 
    let optimizedBigOperation (numQubits:int) (numRuns:int) =
        let k = Ket(numQubits)
        let qs = k.Qubits
        let qcirc = Circuit.Compile bigOperation qs
        let qcirc = qcirc.GrowGates(k)        
        let mutable stats = 0
        for i in 1 .. numRuns do
            let qs = k.Reset()
            qcirc.Run qs
            stats <- stats + qs.[qs.Length - 1].Bit.v
        show "Number of 0's for last qubit: %d" (numRuns - stats)
        show "Number of 1's for last qubit: %d" stats        

    // 'Stabilizer' way of running the big quantum operation
    // Since the circuit contains only H, CNOT, X, Z and M, simulate it efficiently using stabilizer (via Gottesmann-Knill theorem)
    let stabilizerBigOperation (numQubits:int) (numRuns:int) =
        let k = Ket(numQubits)
        let qs = k.Qubits
        let qcirc = Circuit.Compile bigOperation qs        
        let mutable stats = 0
        for i in 1 .. numRuns do
            let qs = k.Reset()
            let stab = Stabilizer(qcirc, k)            
            stab.Run()
            let _, b = stab.[qs.Length - 1]
            stats <- stats + b.v
        show "Number of 0's for last qubit: %d" (numRuns - stats)
        show "Number of 1's for last qubit: %d" stats        


// Implement your functions here
///////////////////////////////////////////    

    /// <summary>
    /// Function which takes a qubit and encodes it in the 3-qubit bit flip (X-flip) code.
    /// The list of indexes specifies the order of the qubits. The qubit from idxes.[0] is the one to be encoded.
    /// The encoded qubits will be qs.[idxes.[0 .. 2]].
    /// </summary>
    /// <param name="qs">Qubits which will become the encoded state</param>
    /// <param name="idxes">List of indexes for encoding order</param>        
    let xflipEncoder (qs:Qubits) (idxes:List<int>) =
        CNOT [qs.[idxes.[0]]; qs.[idxes.[1]]]
        CNOT [qs.[idxes.[0]]; qs.[idxes.[2]]]

    /// <summary>
    /// The opposite of xflipEncoder. Takes a list of qubits assumed to be an encoded logical state
    /// and decodes to one qubit.
    /// </summary>
    /// <param name="qs">Qubits representing encoded state</param>
    /// <param name="idxes">List of indexes for decoding order</param>        
    let xflipDecoder (qs:Qubits) (idxes:List<int>) =
        CNOT [qs.[idxes.[0]]; qs.[idxes.[1]]]
        CNOT [qs.[idxes.[0]]; qs.[idxes.[2]]]

    /// <summary>
    /// Function which takes a qubit and encodes it in the 3-qubit phase flip (Z-flip) code.
    /// Operation similar to xflipEncoder.
    /// </summary>
    /// <param name="qs">Qubits which will become the encoded state</param>
    /// <param name="idxes">List of indexes for encoding order</param>        
    let pflipEncoder (qs:Qubits) (idxes:List<int>) =
        xflipEncoder qs idxes
        H >< [qs.[idxes.[0]];qs.[idxes.[1]];qs.[idxes.[2]]]

    /// <summary>
    /// The opposite of pflipEncoder. Takes a list of qubits assumed to be an encoded logical state
    /// and decodes to one qubit.
    /// </summary>
    /// <param name="qs">Qubits representing encoded state</param>
    /// <param name="idxes">List of indexes for decoding order</param>        
    let pflipDecoder (qs:Qubits) (idxes:List<int>) =
        H >< [qs.[idxes.[0]];qs.[idxes.[1]];qs.[idxes.[2]]]
        xflipDecoder qs idxes

    /// <summary>
    /// Syndrome measurement for phase flip code.
    /// </summary>
    /// <param name="qs">Qubits representing encoded state</param>        
    let pflipSyndrome (qs:Qubits) =
        H >< qs
        let out = xflipSyndrome qs
        H >< qs
        out

    /// <summary>
    /// Function to test the X flip error correcting code.
    /// Apply X error randomly on one of the encoded qubits (or none), perform syndrome measurement and correct.
    /// Display state before and after correction.
    /// </summary>
    /// <param name="qs">List of qubits assumed to be an encoded logical state</param>
    /// <param name="n">Number of tests</param>        
    let testXFlip (qs:Qubits) (n:int) =
        let rand = System.Random()
        let k = qs.Head.Ket
        for i in 1 .. n do
            show "State before error: %s" (k.ToString())
            let idx = rand.Next(0, 4)
            if (idx < 3) then
                X [qs.[idx]]
            show "State after error: %s" (k.ToString())                
            let syndrome = xflipSyndrome qs
            if (syndrome > 0) then
                X [qs.[syndrome - 1]] 
            show "State after correction: %s" (k.ToString())                

    /// <summary>
    /// Function to test the phase flip error correcting code.
    /// Apply Z error randomly on one of the encoded qubits (or none), perform syndrome measurement and correct.
    /// Display state before and after correction.
    /// </summary>
    /// <param name="qs">List of qubits assumed to be an encoded logical state</param>
    /// <param name="n">Number of tests</param>        
    let testPFlip (qs:Qubits) (n:int) =
        let rand = System.Random()
        let k = qs.Head.Ket
        for i in 1 .. n do
            show "State before error: %s" (k.ToString())
            let idx = rand.Next(0, 4)
            if (idx < 3) then
                Z [qs.[idx]]
            show "State after error: %s" (k.ToString())                
            let syndrome = pflipSyndrome qs
            if (syndrome > 0) then
                Z [qs.[syndrome - 1]] 
            show "State after correction: %s" (k.ToString())

    /// <summary>
    /// Syndrome measurement for phase flip errors in Shor's code
    /// </summary>
    /// <param name="qs">Qubits representing encoded (9-qubit) state</param>        
    let bigPflipSyndrome (qs:Qubits) =
        xflipDecoder qs [0; 1; 2]
        xflipDecoder qs [3; 4; 5]
        xflipDecoder qs [6; 7; 8]     
        let out = pflipSyndrome [qs.[0]; qs.[3]; qs.[6]]
        xflipEncoder qs [0; 1; 2]
        xflipEncoder qs [3; 4; 5]
        xflipEncoder qs [6; 7; 8]
        out

    /// <summary>
    /// Function to encode a qubit into the 9-qubit logical state of Shor's code
    /// </summary>
    /// <param name="qs">Qubits which will become the encoded state</param>        
    let shorEncoder (qs:Qubits) = 
        pflipEncoder qs [0; 3; 6]
        xflipEncoder qs [0; 1; 2]
        xflipEncoder qs [3; 4; 5]
        xflipEncoder qs [6; 7; 8]

    /// <summary>
    /// Function to decode a logical State from Shor's code to a single qubit
    /// </summary>
    /// <param name="qs">Qubits representing encoded state</param>        
    let shorDecoder (qs:Qubits) =
        xflipDecoder qs [0; 1; 2]
        xflipDecoder qs [3; 4; 5]
        xflipDecoder qs [6; 7; 8]     
        pflipDecoder qs [0; 3; 6]

    /// <summary>
    /// Correction operation for Shor's code. Performs appropriate syndrome measurements and applies corrections based on outcomes.
    /// </summary>
    /// <param name="qs">Qubits representing encoded state</param>        
    let shorCorrector (qs:Qubits) =
        let out = xflipSyndrome qs.[0 .. 2]
        if (out > 0) then
            X [qs.[out - 1]]
        let out = xflipSyndrome qs.[3 .. 5]
        if (out > 0) then
            X [qs.[out - 1 + 3]]
        let out = xflipSyndrome qs.[6 .. 8]
        if (out > 0) then
            X [qs.[out - 1 + 6]]
        let out = bigPflipSyndrome qs
        if (out > 0) then
            Z [qs.[(out - 1) * 3]]

    /// <summary>
    /// Function to test Shor's code. Random errors from the list of errors are applied on one randomly chosen qubit.
    /// State is displayed before and after correction.
    /// </summary>
    /// <param name="qs">Qubit list where first qubit is the one to be encoded in Shor's code</param>        
    let testShor (qs:Qubits) =
        let rand = System.Random()
        let k = qs.Head.Ket
        let errors = [I; X; Z; Y; H; T; (rotX (Math.PI/8.)); M]
        let qIdx = rand.Next(0, qs.Length)
        let errIdx = rand.Next(0, errors.Length)
        show "Qubit before encoding: %s" (qs.[0].ToString())
        show "Error to be applied: %d" errIdx
        show "On qubit: %d" qIdx
        shorEncoder qs
        errors.[errIdx] [qs.[qIdx]] // apply error
        if (errIdx = errors.Length - 1) then // if measurement, reanimate qubit
            qs.[qIdx].ReAnimate(qs.[qIdx].Bit)        
        shorCorrector qs
        shorDecoder qs
        M >< qs.[1 .. 8]
        show "Qubit after decoding: %s" (qs.[0].ToString())

    /// <summary>
    /// Syndrome measurement for X flip code implemented as a parity check operation using computational basis measurements
    /// and ancilla qubits.
    /// </summary>
    /// <param name="qs">Qubits representing encoded state</param>
    /// <param name="anc">Ancilla qubits to store parities</param>                    
    let parityCheck (qs:Qubits) (anc:Qubits) =
        CNOT [qs.[0]; anc.[0]]
        CNOT [qs.[1]; anc.[0]]

        CNOT [qs.[1]; anc.[1]]
        CNOT [qs.[2]; anc.[1]]

        M >< anc
        show "Parities: %d%d" anc.[0].Bit.v anc.[1].Bit.v
///////////////////////////////////////////


    // program entry point
    [<LQD>]
    let Lab_4()    =
        show "Lab 4"
        let k = Ket(9)
        let qs = k.Qubits
        rotX (Math.PI/4.) qs
        testShor qs

#if INTERACTIVE
do Script.Lab_4()        // If interactive, then run the routine automatically
#endif
