// Copyright (c) 2015,2016 Microsoft Corporation

#r @"\Liquid\bin\Liquid1.dll" // This is show IntelliSense will work in Visual Studio        

namespace Microsoft.Research.Liquid // Tell the compiler our namespace

open Operations
open Util
open System
open System.Text.RegularExpressions
open System.Collections.Generic
open System.IO

module QuAMGates =

    /// <summary>
    /// Samp gate: 2 Qubit operator to define a sample
    /// </summary>
    /// <param name="p">Sample index (>= 1)</param>
    /// <param name="qs">Operate on first 2 qubits</param>
    let SAMP (p:int) (qs:Qubits)  =
        let gate (p:int) =
            new Gate(
                Name    = "SAMP",
                Help    = "QuAM Sample gate",
                Draw    = "\\ctrl{#1}\\go[#1]\\gate{SAMP]",
                Mat     = (
                    let sq n d  = (float n) / (float d) |> sqrt
                    let v1      = sq (p-1) p
                    let v2      = 1.0 / (sq p 1)
                    CSMat(4,[   (0,0,1.,0.)
                                (1,1,1.,0.)
                                (2,2, v1,0.); (2,3,-v2,0.)
                                (3,2, v2,0.); (3,3, v1,0.)
                    ]))
        )
        (gate p).Run qs

    /// <summary>
    /// Controlled NOT gate with 0 control
    /// </summary>
    /// <param name="qs"> Use first two qubits for gate</param>
    let CNOT0 (qs:Qubits) =
        let gate =
            Gate.Build("CNOT0",fun () ->
                new Gate(
                    Name    = "CNOT0",
                    Help    = "Controlled NOT with 0 control",
                    Mat     = (
                        CSMat(4,[(0,1,1.,0.);(1,0,1.,0.);(2,2,1.,0.);(3,3,1.,0.)])),
                    Draw    = "\\ctrlo{#1}\\go[#1]\\targ"
            ))
        gate.Run qs

    /// <summary>
    /// Controlled NOT gate with 1 control
    /// </summary>
    /// <param name="qs"> Use first two qubits for gate</param>
    let CNOT1 (qs:Qubits) =
        let gate =
            Gate.Build("CNOT1",fun () ->
                new Gate(
                    Name    = "CNOT1",
                    Help    = "Controlled NOT with 1 control",
                    Mat     = (
                        CSMat(4,[(0,0,1.,0.);(1,1,1.,0.);(2,3,1.,0.);(3,2,1.,0.)])),
                    Draw    = "\\ctrl{#1}\\go[#1]\\targ"
            ))
        gate.Run qs

    /// <summary>
    /// Toffoli gate when |00> on first two
    /// </summary>
    /// <param name="qs"> Use first three qubits for gate</param>
    let CCNOT00 (qs:Qubits) =
        let gate =
            Gate.Build("CCNOT00",fun () ->
                new Gate(
                    Name    = "CCNOT00",
                    Help    = "Toffoli if |00>",
                    Mat     = (
                        let m       = CSMat(8)
                        m.[0,0]    <- Complex.Zero
                        m.[0,1]    <- Complex.One
                        m.[1,0]    <- Complex.One
                        m.[1,1]    <- Complex.Zero
                        m),
                    Draw    = "\\ctrlo{#1}\\go[#1]\\ctrlo{#2}\\go[#2]\\targ"
            ))
        gate.Run qs

    /// <summary>
    /// Toffoli gate when |01> on first two
    /// </summary>
    /// <param name="qs"> Use first three qubits for gate</param>
    let CCNOT01 (qs:Qubits) =
        let gate =
            Gate.Build("CCNOT01",fun () ->
                new Gate(
                    Name    = "CCNOT01",
                    Help    = "Toffoli if |01>",
                    Mat     = (
                        let m       = CSMat(8)
                        m.[2,2]    <- Complex.Zero
                        m.[2,3]    <- Complex.One
                        m.[3,2]    <- Complex.One
                        m.[3,3]    <- Complex.Zero
                        m),
                    Draw    = "\\ctrlo{#1}\\go[#1]\\ctrl{#2}\\go[#2]\\targ"
            ))
        gate.Run qs

    /// <summary>
    /// Toffoli gate when |10> on first two
    /// </summary>
    /// <param name="qs"> Use first three qubits for gate</param>
    let CCNOT10 (qs:Qubits) =
        let gate =
            Gate.Build("CCNOT10",fun () ->
                new Gate(
                    Name    = "CCNOT10",
                    Help    = "Toffoli if |10>",
                    Mat     = (
                        let m       = CSMat(8)
                        m.[4,4]    <- Complex.Zero
                        m.[4,5]    <- Complex.One
                        m.[5,4]    <- Complex.One
                        m.[5,5]    <- Complex.Zero
                        m),
                    Draw    = "\\ctrl{#1}\\go[#1]\\ctrlo{#2}\\go[#2]\\targ"
            ))
        gate.Run qs

    /// <summary>
    /// Toffoli gate when |11> on first two
    /// </summary>
    /// <param name="qs"> Use first three qubits for gate</param>
    let CCNOT11 (qs:Qubits) =
        let gate =
            Gate.Build("CCNOT11",fun () ->
                new Gate(
                    Name    = "CCNOT11",
                    Help    = "Toffoli if |11>",
                    Mat     = (
                        let m       = CSMat(8)
                        m.[6,6]    <- Complex.Zero
                        m.[6,7]    <- Complex.One
                        m.[7,6]    <- Complex.One
                        m.[7,7]    <- Complex.Zero
                        m),
                    Draw    = "\\ctrl{#1}\\go[#1]\\ctrl{#2}\\go[#2]\\targ"
            ))
        gate.Run qs

    // Bit extractor from data to qubit ordering
    let getBit numBits idx bits =
        if bits &&& (1 <<< (numBits-(idx+1))) <> 0 then 1 else 0

    /// <summary>
    /// FLIP gate: High level QuAM gate
    /// </summary>
    /// <param name="prv">Previous pattern (n bits long)</param>
    /// <param name="cur">Current pattern (n bits long)</param>
    /// <param name="qs">3 registers: x=n qubits, g=n-1 qubits,, c=2 qubits</param>
    let FLIP (prv:int) (cur:int) (qs:Qubits)  =

        let gate (prv:int) (cur:int) =
            let op (qs:Qubits) =

                // Pull out register lengths
                let xCnt    = (qs.Length-1) / 2
                let gCnt    = xCnt-1
                let cCnt    = 2

                // Pull out register qubits
                let xs      = !!(qs,[0..xCnt-1])
                let gs      = !!(qs,[xCnt..xCnt+gCnt-1])
                let cs      = !!(qs,[xCnt+gCnt..xCnt+gCnt+cCnt-1])
            
                for j in 0..xCnt-1 do
                    let prvBit  = getBit xCnt j prv
                    let curBit  = getBit xCnt j cur
                    if prvBit <> curBit then
                        CNOT0 [cs.[1];xs.[j]]      
                CNOT0 !!(cs,1,0)
        
            new Gate(
                Name    = "FLIP",
                Help    = "QuAM FLIP gate",
                Draw    = sprintf "\\multigate{#%d}{FLIP}" (qs.Length-1),
                Op      = WrapOp op
               )
        (gate prv cur).Run qs

    let doA bits qs =
        let f   =
            match bits with
            | 0    -> CCNOT00
            | 1    -> CCNOT10
            | 2    -> CCNOT01
            | 3    -> CCNOT11
            | _    -> failwithf "doA got illegal index: %d" bits
        f qs

    /// <summary>
    /// AND gate: High level QuAM gate
    /// </summary>
    /// <param name="cur">Current sample bits</param>
    /// <param name="qs">3 registers: x=n qubits, g=n-1 qubits,, c=2 qubits</param>
    let AND (cur:int) (qs:Qubits)  =
        let gate cur =
            let op (qs:Qubits) =

                // Pull out register lengths
                let xCnt    = (qs.Length-1) / 2
                let gCnt    = xCnt-1
                let cCnt    = 2

                // Pull out register qubits
                let xs      = !!(qs,[0..xCnt-1])
                let gs      = !!(qs,[xCnt..xCnt+gCnt-1])
                let cs      = !!(qs,[xCnt+gCnt..xCnt+gCnt+cCnt-1])

                let bit0    = getBit xCnt 0 cur
                let bit1    = getBit xCnt 1 cur          
                let bits    = bit1*2 + bit0
                doA bits [xs.[0];xs.[1];gs.[0]]
                for k in 2..xCnt-1 do
                    let bit0    = getBit xCnt k cur
                    let bits    = bit0 + 2
                    doA bits [xs.[k];gs.[k-2];gs.[k-1]]

            new Gate(
                Name    = "AND",
                Help    = "QuAM AND gate",
                Draw    = sprintf "\\multigate{#%d}{AND}" (qs.Length-1),
                Op      = WrapOp op
               )
        (gate cur).Run qs

    /// <summary>
    /// AND' gate: High level QuAM gate
    /// </summary>
    /// <param name="cur">Current sample bits</param>
    /// <param name="qs">3 registers: x=n qubits, g=n-1 qubits,, c=2 qubits</param>
    let ANDAdj (cur:int) (qs:Qubits)  =
        let gate cur =
            let op (qs:Qubits) =

                // Pull out register lengths
                let xCnt    = (qs.Length-1) / 2
                let gCnt    = xCnt-1
                let cCnt    = 2

                // Pull out register qubits
                let xs      = !!(qs,[0..xCnt-1])
                let gs      = !!(qs,[xCnt..xCnt+gCnt-1])
                let cs      = !!(qs,[xCnt+gCnt..xCnt+gCnt+cCnt-1])

                for k in xCnt-1 .. -1 .. 2 do
                    let bit0    = getBit xCnt k cur
                    let bits    = bit0 + 2
                    doA bits [xs.[k];gs.[k-2];gs.[k-1]]
                let bit0    = getBit xCnt 0 cur
                let bit1    = getBit xCnt 1 cur          
                let bits    = bit1*2 + bit0
                doA bits [xs.[0];xs.[1];gs.[0]]

            new Gate(
                Name    = "AND'",
                Help    = "QuAM AND' gate",
                Draw    = sprintf "\\multigate{#%d}{AND^\dagger}" (qs.Length-1),
                Op      = WrapOp op
               )
        (gate cur).Run qs

    /// <summary>
    /// LOAD gate: High level QuAM gate to load one pattern
    /// </summary>
    /// <param name="p">Total number of patterns to store</param>
    /// <param name="prv">Previous sample bits</param>
    /// <param name="cur">Current sample bits</param>
    /// <param name="qs">3 registers: x=n qubits, g=n-1 qubits,, c=2 qubits</param>
    let LOAD (p:int) (prv:int) (cur:int) (qs:Qubits)  =
        let gate p prv cur =
            let op (qs:Qubits) =

                // Pull out register lengths
                let xCnt    = (qs.Length-1) / 2
                let gCnt    = xCnt-1
                let cCnt    = 2

                // Pull out register qubits
                let xs      = !!(qs,[0..xCnt-1])
                let gs      = !!(qs,[xCnt..xCnt+gCnt-1])
                let cs      = !!(qs,[xCnt+gCnt..xCnt+gCnt+cCnt-1])

                FLIP prv cur qs
                SAMP p cs
                AND cur qs
                CNOT1 [gs.[gCnt-1];cs.[0]]
                ANDAdj cur qs
            new Gate(
                Name    = "LOAD",
                Help    = "QuAM LOAD gate",
                Draw    = sprintf "\\multigate{#%d}{LOAD}" (qs.Length-1),
                Op      = WrapOp op
               )
        (gate p prv cur).Run qs

    /// <summary>
    /// Identity with items flipped (for Grover's algorithm)
    /// </summary>
    /// <param name="flips">List of indicies to flip signs</param>
    /// <param name="qs">Qubits to define identity for</param>
    let IFLIP (flips:int list) (qs:Qubits) =
        let gate (flips:int list) =
            new Gate(
                Name    = "IFLIP",
                Help    = "Identity with items flipped",
                Mat     = (
                    let m   = CSMat(1 <<< qs.Length)
                    for i in flips do m.[i,i] <-  Complex(-1.0,0.0)
                    m),
                Draw    = sprintf "\\multigate{#%d}{IFLIP}" (qs.Length-1)
            )
        (gate flips).Run qs

    /// <summary>
    /// Identity negated with items flipped (for Grover's algorithm)
    /// </summary>
    /// <param name="flips">List of indicies to flip signs</param>
    /// <param name="qs">Qubits to define identity for</param>
    let InFLIP (flips:int list) (qs:Qubits) =
        let gate (flips:int list) =
            new Gate(
                Name    = "InFLIP",
                Help    = "Identity negated with items flipped",
                Mat     = (
                    let m   = CSMat(1 <<< qs.Length)
                    let cm1 = Complex(-1.0,0.0)
                    for x,y in m.Filled() do m.[x,y]   <- cm1
                    for i in flips do m.[i,i]          <- Complex.One
                    m),
                Draw    = sprintf "\\multigate{#%d}{InFLIP}" (qs.Length-1)
            )
        (gate flips).Run qs

    /// <summary>
    /// Grover diffusion operator
    /// </summary>
    /// <param name="qs">Qubits to define Grover for</param>
    let GDiff (qs:Qubits) =
        let gate =
            let nam = "GDiff" + qs.Length.ToString()
            Gate.Build(nam,fun () ->
                new Gate(
                    Name    = nam,
                    Help    = nam + " Grover diffusion operator",
                    Draw    = sprintf "\\multigate{#%d}{GDiff}" (qs.Length-1),
                    Op      = WrapOp (fun qs ->
                        H >< qs
                        InFLIP [0] qs
                        H >< qs
                    )
                ))
        gate.Run qs

    /// <summary>
    /// Grover step
    /// </summary>
    /// <param name="pats">Patterns that we're searching for</param>
    /// <param name="qs">Qubits to apply Grover step</param>
    let GStep (pats:int list) (qs:Qubits) =
        let gate (pats:int list) =
            new Gate(
                Name    = "GStep",
                Help    = "Grover step",
                Draw    = sprintf "\\multigate{#%d}{GStep}" (qs.Length-1),
                Op      = WrapOp (fun qs ->
                    IFLIP pats qs
                    GDiff qs
                )
            )
        (gate pats).Run qs

    // Statistics for Grover iteration (set to Some to have stats accumulated for that many steps)
    let mutable GroverStats = None : (int*float)[] option

    /// <summary>
    /// Modified Grover's search for QuAM
    /// </summary>
    /// <param name="pats">Patterns that we've loaded</param>
    /// <param name="poss">Patterns that we're searching for</param>
    /// <param name="qs">Qubits to search</param>
    let Grover (pats:int list) (poss:int list) (qs:Qubits) =
        let GroverSave iter (qs:Qubits) =
            match GroverStats with
            | None     -> ()
            | Some gs  ->
                let v       = qs.[0].Ket.Single()
                let idxs    = v.NonZeros(256,1.e-4)
                let info    = 
                    List.map (fun (idx:uint64) -> (int idx),(v.[idx].MCC)) idxs
                    |> List.maxBy (fun (i,v) -> v)
                gs.[iter] <-  info
        
        let gate (pats:int list) (poss:int list) =

            // Optimal Grover steps (or forced by stats)
            let stats,numSteps =
                match GroverStats with
                | None     -> false,Math.Sqrt(float(1 <<< qs.Length) / (float pats.Length)) + 0.5 |> int
                | Some gs  -> true,gs.Length

            new Gate(
                Name    = "Grover",
                Help    = "Grover search",
                Draw    = sprintf "\\multigate{#%d}{Grover}" (qs.Length-1),
                Op      = WrapOp (fun qs ->
                    GStep poss qs
                    if stats then Native(fun qs -> GroverSave 0 qs) qs
                    GStep pats qs
                    if stats then Native(fun qs -> GroverSave 1 qs) qs
                    for i in 3..numSteps do 
                        GStep poss qs
                        if stats then Native(fun qs -> GroverSave (i-1) qs) qs
                ))
        (gate pats poss).Run qs


module Script =
    open QuAMGates

    let rnd         = Random()

    let init n (pats:int list) (qs:Qubits) =
        let p           = pats.Length 
        if p > (1 <<< n) then failwithf "Loading %d patterns of %d bits?" p n

        let names   = List.init qs.Length (fun i ->
            if i < n then sprintf "x%d" i
            elif i < 2*n-1 then sprintf "g%d" (i-n)
            else sprintf "c%d" (i+1-2*n)
            )
        Label >!< (names,qs)
        let rec doLoad prv (ptns:int list) =
            let p   = ptns.Length
            match ptns with
            | []               -> ()
            | cur::ptns        ->
                LOAD p prv cur qs
                doLoad cur ptns
        doLoad 0 pats
        X !!(qs,qs.Length-1)    // Final Ancilla fix up

    let dumpProb n (ket:Ket) (title:string) =
        show "======= DUMPSTATE: %s" title
        let v       = ket.Single()
        let idxs    = v.NonZeros(256,1.e-4)
        let maxIdx  = 1 <<< n |> float
        let info    = 
            List.map (fun (idx:uint64) -> (int idx),(v.[idx].MCC)) idxs
            |> List.sortBy (fun (i,v) -> -(v+(maxIdx-(float i))/1.e+8))
        let rec showItm cnt info =
            match info with
            | []               -> ()
            | _ when cnt >= 20 -> ()
            | (idx,v)::info    ->
                let xs  = idx >>> (n+1)
                show "0x%02x: %4.1f%% (0x%06x)" xs (v*100.0) idx                    
                showItm (cnt+1) info
        showItm 0 info

    ////////////////////////////////////////////////////////////////////////////

    [<LQD>]
    let QuAM() =  // 4bit key, 4 bit val

        let n           = 8             // Max bits in input patterns
        let pats        = [
            for i in 0..15 ->
                let j   = rnd.Next(16)
                i <<< 4 ||| j
            ]
                    
        let ket     = Ket(2*n+1)
        let qs      = ket.Qubits
        let circ    = Circuit.Compile (init n pats) qs

        circ.Render("QuAM1.htm")
        //circ.Dump()
        ket.TraceRun   <- 0   // 0 = None, 1 = Log, 2 = Console, 2 = States
        circ.Run qs
        dumpProb n ket "Loaded patterns"

        let xs          = !!(qs,[0..n-1])

        let key         = 6
        let poss        = [for i in 0..15 -> key <<< 4 ||| i]

        // Gather statistics for a forced number of steps
        GroverStats    <- Array.create 17 (0,0.0) |> Some

        let circ        = Circuit.Compile (Grover pats poss) xs
        //circ.Dump()
        circ.Fold().Render("QuAM2.htm")
        ket.TraceRun   <- 0
        circ.Run xs
        let gs          = GroverStats.Value
        for iter in 0..gs.Length-1 do
            let idx,v   = gs.[iter]
            let xs      = idx >>> (n+1)
            show "    Grover[%2d]: 0x%02x: %4.1f%% (0x%06x)" iter xs (v*100.0) idx                    
        show ""
        sprintf "Searched for key: %x" key |> dumpProb n ket
