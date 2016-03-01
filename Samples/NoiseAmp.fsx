// Copyright (c) 2015,2016 Microsoft Corporation

#if INTERACTIVE
#r @"..\bin\Liquid1.dll"                 
#else
namespace Microsoft.Research.Liquid // Tell the compiler our namespace
#endif

open System                         // Open any support libraries
open System.Text                    // to get StringBuilder

open Microsoft.Research.Liquid      // Get necessary Liquid libraries
open Util                           // General utilites
open Operations                     // Basic gates and operations

module Script =                     // The script module allows for incremental loading

    [<LQD>]
    let NoiseAmp() =

        // Output dump routine
        let sb              = StringBuilder()
        let app (x:string)  = sb.Append x |> ignore
        let dump iter (v:CVec) =
            if iter = 0 then show "Iter,qs=00,qs=01,qs=10,qs=11"

            sb.Length      <- 0
            sprintf "%4d" iter |> app
            for i in 0UL..v.Length-1UL do
                app ","
                sprintf "%7.5f" v.[i].MCC |> app
            show "%O" sb

        // Probabilities for our two types of noise
        let probDamp        = 2.e-3
        let probPolar       = 2.e-3

        // 2 Qubit tests
        let ket     = Ket(2)

        // Build the initial state
        let qs          = ket.Reset(2)
        H    qs
        CNOT qs

        // Create Idle circuit
        let circ    = Circuit.Compile (fun (qs:Qubits) -> I >< qs) ket.Qubits

        // Create noise model
        let mkM (p:float) (g:string) (mx:int) = {Noise.DefaultNoise p with gate=g;maxQs=mx}
        let models      = [
            mkM 0.0             "H"         1
            mkM 0.0             "CNOT"      2
            mkM probPolar       "I"         1
        ]

        let noise           = Noise(circ,ket,models)
        
        ket.TraceRun       <- 0         // 1=log 2=console
        noise.LogGates     <- false     // Show each gate execute?
        noise.TraceWrap    <- false
        noise.TraceNoise   <- false
        noise.DampProb(0)  <- probDamp
        noise.DampProb(1)  <- probDamp

        // Get a handle to the state vector for output
        let v           = ket.Single()

        dump 0 v
        for iter in 1..500 do
            if iter = 1 then noise.Run ket else noise.Run()
            dump iter v

        noise.Dump(showInd,0,true)

        // Generate a circuit diagram
        let circ    = Circuit.Compile (fun (qs:Qubits) -> 
                        H qs
                        CNOT qs
                        for i in 0..4 do I >< qs) qs
        circ.Fold().RenderHT("NoiseAmp")

#if INTERACTIVE
do 
    Script.NoiseAmp()        // If interactive, then run the routine automatically
#endif
