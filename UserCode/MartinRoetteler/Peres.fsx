// quick test of POVM functions using the Peres game as an example
// Copyright (c) 2015,2016 Microsoft Corporation

#if INTERACTIVE
#r @"..\bin\Liquid1.dll"                 
#else
namespace Microsoft.Research.Liquid // Tell the compiler our namespace
#endif

open System                         // Open any support libraries
open System.Collections.Generic

open Microsoft.Research.Liquid      // Get necessary Liquid libraries
open Util                           // General utilites
open Operations                     // Basic gates and operations

module Script =                     // The script module allows for incremental loading

    let PeresPOVM (qs:Qubits) =
        let gate =
            Gate.Build("Peres",fun () ->
                let p   = 1.0/(Math.Sqrt(6.0)) // I tried various other scalings also -> no dice
                let a   = 2.0*Math.PI/3.0
                let vR  = Math.Cos a
                let vI  = Math.Sin a  
                new Gate(
                    Name    = sprintf "Peres game POVM",
                    Help    = sprintf "Unambiguous state discrimination in Peres three state game",
                    Kraus   = [                                        
                        {tag=0;mat=CSMat(2,[(0,0,p,0.);(0,1,-p,0.);(1,0,-p,0.);(1,1,p,0.)])}
                        {tag=1;mat=CSMat(2,[(0,0,p,0.);(0,1,-p*vR,p*vI);(1,0,-p*vR,-p*vI);(1,1,p,0.)])}
                        {tag=2;mat=CSMat(2,[(0,0,p,0.);(0,1,-p*vR,-p*vI);(1,0,-p*vR,p*vI);(1,1,p,0.)])}
                        ],
                    Draw    = "\\gate{" + "Peres" + "}",
                    Op      = Channel("Peres")
                    ))
        gate.Run qs

    // Support for prep (input) state
    let RotationGate (qs:Qubits) = 
        let a   = 2.0*Math.PI/3.0
        let vR  = Math.Cos a
        let vI  = Math.Sin a 
        let gate = 
            new Gate(
                Name    = sprintf "RZ(2 pi /3)",
                Help    = sprintf "Diagonal rotation 2 pi /3",
                Mat     = CSMat(2,[(0,0,1.,0.);(1,1,vR,vI)])
                )
        gate.Run qs
        
    // Make the input state
    let PrepState (input:int) (qs:Qubits) = 
        H qs
        let rot = RotationGate
        match input with 
        | 0 -> I qs
        | 1 -> rot qs
        | 2 -> rot qs; rot qs
        | _ -> failwith "input must be 0, 1, or 2"

    /// <summary>
    /// A simple application for POVMs on 1 qubit: Peres' three state game [Peres, Quantum Theory, Kluwer, 2002, p. 287]
    /// </summary>
    /// <param name="input">An integer the specifies one (out of three equiangular single qubit states) to be prepared</param>
    /// <returns>1 if bad, 0 otherwise</returns>
    let GeneratePeresGameChallenge (input:int) = 
        let k = Ket(1)
        let qs = k.Qubits
            
        // Build a channel corresponding to the Peres POVM
        let gateUA  = !< PeresPOVM qs
        let chan    = gateUA.OptimizeKraus()
                    
        // Challenger creates qubit and hands to opponent
        PrepState input qs

        // Opponent makes a guess
        chan qs
        let povm    = qs.Head.Ket.Symbol "Peres"
        show "Input = %d, Measurement result = %d %s" input povm (if input = povm then " <===== BAD!!!!" else "")
        if povm <> input then 0 else 1
        
    [<LQD>]    
    let Peres(count:int) =
        let rnd = Random()
        let rec runExamples i bad =
            if i = count then bad
            else
                let input   = rnd.Next(3)
                let rslt    = GeneratePeresGameChallenge input
                runExamples (i+1) (bad + rslt)
        let rslt    = runExamples 0 0
        show "Count of bad results: %d out of %d" rslt count
            
          
#if INTERACTIVE
do Script.Peres(100)        // If interactive, then run the routine automatically
#endif
