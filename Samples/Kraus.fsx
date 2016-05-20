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

    // These are the same gates that are defined in Operations, but are reproduced here to show
    // how a user could define their own Kraus Operator based Channels.
    // The two gates (AD and DP) are followed by the Kraus() defintion which is the source for the built-in
    // __Kraus() example. See the User's Manual for details on how to edit/define your own noise operators.

    /// <summary>
    /// Amplitude Damping Channel using two Kraus operators (always put the most probable one first)
    /// </summary>
    /// <param name="prob">Probability of damping</param>
    /// <param name="qs">The list of qubits that the Kraus operators touch</param>
    let AD (prob:float) (qs:Qubits) =
        let gate (prob:float) =
            Gate.Build("AmpDamp_" + prob.ToString("E2"),fun () ->
                new Gate(
                    Name    = sprintf "AD%.2g" prob,
                    Help    = sprintf "Amplitude Damping channel: %.2g" prob,
                    Kraus   = [
                         {tag=0;mat=CSMat(2,[(0,0,1.,0.);(1,1,sqrt(1.-prob),0.0)])}
                         {tag=1;mat=CSMat(2,[(0,1,sqrt(prob),0.0)])}
                        ],
                    Draw    = "\\gate{" + ("AD" + (prob.ToString("E2"))) + "}",
                    Op      = Channel("POVM")
                   ))
        (gate prob).Run qs

    /// <summary>
    /// Depolarizing channel using four Kraus operators (always put the most probable one first)
    /// </summary>
    /// <param name="prob">Probability of depolarizing in X,Y or Z</param>
    /// <param name="qs">The list of qubits that the Kraus operators touch</param>
    let DP (prob:float) (qs:Qubits) =
        let gate (prob:float) =
            let e0  = sqrt(1.-3.*prob/4.)
            let e1  = sqrt(prob/4.) 
            Gate.Build("DePol_" + prob.ToString("E2"),fun () ->
                new Gate(
                    Name    = sprintf "DP%.2g" prob,
                    Help    = sprintf "Depolarizing channel: %.2g" prob,
                    Kraus   =
                        [
                         {tag=0;mat=CSMat(2,[(0,0,e0, 0.);(1,1, e0,0.)])}   // I
                         {tag=1;mat=CSMat(2,[(0,1,e1, 0.);(1,0, e1,0.)])}   // X
                         {tag=2;mat=CSMat(2,[(0,1,0.,-e1);(1,0, 0.,e1)])}   // Y
                         {tag=3;mat=CSMat(2,[(0,0,e1, 0.);(1,1,-e1,0.)])}   // Z
                        ],
                    Draw    = "\\gate{" + ("DP" + (prob.ToString("E2"))) + "}",
                    Op      = Channel("POVM")
                   ))
        (gate prob).Run qs

    /// <summary>
    /// Show how to add noise to Teleport with Kraus operators
    /// </summary>
    /// <param name="nRuns">How many runs to gather statistics over</param>
    /// <param name="probAD">Probability of Amplitude Damping on any single qubit</param>
    /// <param name="probDP">Probability of Depolarizing noise on any single qubit</param>
    /// <param name="verbose">Output detailed stats and drawings</param>
    [<LQD>]
    let Kraus(nRuns,probAD,probDP,verbose) =

        // Teleport will require 3 qubits
        let ket         = Ket(3)
        let qs          = ket.Qubits

        // Build a noisy channel with amplitude damping and depolarizing noise
        let gateAD  = !< (AD probAD) qs
        let gateDP  = !< (DP probDP) qs
        let chan    = gateAD.OptimizeKraus(gateDP,"K")
        let nams    = [" I";" X";" Y";" Z";"DI";"DX";"DY";"DZ"]
        let dic     = SortedDictionary<int*string,int>()

        // Remember noise statistics
        let saveStat qId povm =
            let key = (qId,nams.[povm])
            if dic.ContainsKey(key) then
                    dic.[key]      <- dic.[key] + 1
            else    dic.[key]      <- 1

        // Make a set of noisy gates to use
        let makeNoisy (f:Qubits->unit) (qs:Qubits) =
            let g   = !< f qs
            let nam = g.Name + "n"
            let drw = 
                if g.Arity = 1 then g.Draw.Replace("}","n}")
                else sprintf "\\multigate{#%d}{%sn}" (g.Arity-1) g.Name
            let gn  =
                Gate.Build(nam,fun () -> new Gate(Name=nam,Qubits=g.Arity,Draw=drw,Op=WrapOp(fun (qs:Qubits) -> 
                    f       qs
                    for i in 0..g.Arity-1 do
                        let qs' = !!(qs,i) 
                        chan qs'
                        Native (fun (qs:Qubits) ->
                            let q       = qs.Head
                            let povm    = q.Ket.Symbol "POVM"
                            saveStat q.Id povm) qs'
                    )))
            gn.Run qs 

        let Hn (qs:Qubits)      = makeNoisy H qs
        let Xn (qs:Qubits)      = makeNoisy X qs
        let Zn (qs:Qubits)      = makeNoisy Z qs
        let CNOTn (qs:Qubits)   = makeNoisy CNOT qs

        // Teleport circuit with noise channel added
        let teleport (qs:Qubits) =
            Hn qs.Tail                      // EPR 2nd two qubits
            CNOTn qs.Tail
            CNOTn qs                        // Entangle first two qubits
            Hn qs
            M !!(qs,1)
            BC Xn qs.Tail                   // Conditionally apply X
            M !!(qs,0)
            BC Zn !!(qs,0,2)                // Conditionally apply Z

        // Dump out the generated circuit
        if verbose then
            let circ    = Circuit.Compile teleport qs
            circ.Dump()
            circ.Fold().RenderHT("TeleNoise0",0)
            circ.Fold().RenderHT("TeleNoise2",2,50.,100.)

        // Run the circuit N times
        for iter in 1..nRuns do
            let qs  = ket.Reset()
            X qs                            // Teleport a |1>
            teleport qs
            M !!(qs,2)                      // See what we got
            saveStat 3 qs.[2].Bit.v

        // Output the summary statistics
        if verbose then
            show "CSV,qId,POVM,Count"
            for kvp in dic do
                let qId,povm    = kvp.Key
                if qId < 3 then
                    show "CSV,%2d,%2s,%6d" qId povm kvp.Value
        let key1    = (3," X")
        let cnt1    = if dic.ContainsKey(key1) then dic.[key1] else 0
        show "CSV,rslt,%.4f,%.4f,%6d,%6d,%.1f" probAD probDP cnt1 nRuns (100.*float cnt1/float nRuns)


#if INTERACTIVE
do Script.Kraus(1000,0.02,0.02,true)        // If interactive, then run the routine automatically
#endif
