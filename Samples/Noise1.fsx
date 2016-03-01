// Copyright (c) 2015,2016 Microsoft Corporation

#if INTERACTIVE
#r @"..\bin\Liquid1.dll"         // Tell fsi where to find the Liquid dll
#else
namespace Microsoft.Research.Liquid // Tell the compiler our namespace
#endif

open System                         // Open any support libraries
open System.Text                    // to get StringBuilder
open System.Collections.Generic     // Gets us a SortedDictionary

open Microsoft.Research.Liquid      // Get necessary Liquid libraries
open Util                           // General utilites
open Operations                     // Basic gates and operations

module Script =                     // The script module allows for incremental loading
    [<LQD>]
    let Noise1(depth:int,iters:int,prob:float) =
        let ket     = Ket(1)
        let circ    = Circuit.Compile (fun (qs:Qubits) -> 
            for i in 1..depth do I qs) ket.Qubits
        let s7      = Steane7(circ)
        let s7C     = s7.Circuit
        s7C.Dump()
        let ket     = s7.Ket
        let qsAnc   = !!(ket.Qubits,[0..5])
        let qsPhy   = !!(ket.Qubits,[6..12])
        let prep    = Circuit.Compile (fun (qs:Qubits) -> s7.Prep qs) qsPhy
        let prep'   = prep.Reverse()

        // Create noise model
        let mkM (p:float) (g:string) (mx:int) = {Noise.DefaultNoise p with gate=g;maxQs=mx}
        let models      = [
            mkM prob        "X"         1
            mkM prob        "Z"         1
            mkM prob        "H"         1
            mkM prob        "Meas"      1
            mkM prob        "CNOT"      2
            mkM prob        "CZ"        2
            mkM (prob/1.)   "I"         1
            mkM 0.0         "Reset0"    1
        ]

        let noise           = Noise(s7C,ket,models)
        noise.LogGates     <- false     // Show each gate execute?
        ket.TraceRun       <- 0         // 1=log 2=console
        noise.TraceWrap    <- false
        noise.TraceNoise   <- false
        noise.NoNoise      <- ["S7:Prep"]
        noise.ECgates      <- ["S7:Prep";"S7:Syn"]

        // [#gate,EC events] = good, total
        let hist    = SortedDictionary<int*int,int*int>()
        let sw      = Diagnostics.Stopwatch()
        sw.Start()
        let rec doIter iter good =
            if iter <= iters then
                let qs      = ket.Reset()
                noise.Run ket
                //noise.Dump(showInd,0,true)

                // Add up the noise events
                let gateCnt = List.sumBy (fun (m:NoiseModel) -> m.gateEvents.events) models
                let ecCnt   = List.sumBy (fun (m:NoiseModel) -> m.ecEvents.events) models
                let key     = gateCnt,ecCnt
                prep'.Run qs
                M !!(qs,6)
                let success = if qs.[6].Bit = Zero then 1 else 0
                let good    = good + success

                if hist.ContainsKey key then
                    let g,t = hist.[key]
                    hist.[key]     <- ((g+success),(t+1))
                else hist.[key]    <- (success,1)

                if sw.Elapsed.TotalSeconds >=  15.0 then
                    let err     = (float good) / (float iter)
                    show "     ,%2d,%.2e,%4d,%4d,%.2f" depth prob good iter err
                    sw.Restart()
                doIter (iter+1) good 
            else good
        let good    = doIter 1 0
        let err     = (float good) / (float iters)
        show "FINAL,%2d,%9.2e,%4d,%4d,%.2f" depth prob good iters err
        show ""
        show "HIST,%2s,%9s,%4s,%4s,%4s,%4s,%4s" "#" "prob" "gate" "ec" "good" "all" "frac"
        for kvp in hist do
            let gEv,eEv = kvp.Key
            let g,t     = kvp.Value
            show "HIST,%2d,%9.2e,%4d,%4d,%4d,%4d,%4.2f" depth prob gEv eEv g t ((float g) / (float t))


#if INTERACTIVE
do 
    Script.Noise1(1,500,1.0e-2)        // If interactive, then run the routine automatically
#endif
