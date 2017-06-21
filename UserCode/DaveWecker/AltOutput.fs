namespace Microsoft.Research.Liquid

module AltOutput =
    open System
    open Util
    open Operations

    // Define an EPR function
    let EPR (qs:Qubits) = 
        H qs; CNOT qs

    // Sample Teleport circuit
    let Teleport (qs:Qubits) =
        let q0,q1,q2    = qs.[0],qs.[1],qs.[2]      // Extract 3 qubits

        // Give names to the first three qubits
        LabelL "Src" [q0]
        LabelL "\\ket{0}" [q1]
        LabelL "\\ket{0}" [q2]

        EPR[q1;q2]; CNOT qs; H qs               // EPR 1,2, then CNOT 0,1 and H 0
        M[q1]; BC X [q1;q2]                     // Conditionally apply X
        M[q0]; BC Z [q0;q2]                     // Conditionally apply Z
        LabelR "Dest" [q2]                      // Label output

    /// <summary>
    /// Take QFT
    /// </summary>
    /// <param name="qs">Qubits to take QFT of</param>
    let QFT (qs:Qubits) =
        let gate (qs:Qubits)    =
            let op (qs:Qubits) =
                let n   = qs.Length
                for aIdx in n-1..-1..0 do
                    let a   = qs.[aIdx]
                    H [a]
                    for k in 2..aIdx+1 do
                        let c   = qs.[aIdx-(k-1)]
                        Cgate (R k) [c;a]
            Gate.Build("QFT_" + qs.Length.ToString() ,fun () ->
                new Gate(
                    Qubits  = qs.Length,
                    Name    = "QFT",
                    Help    = "QFT",
                    Draw    = sprintf "\\multigate{#%d}{QFT}" (qs.Length-1),
                    Op      = WrapOp op
            ))
        (gate qs).Run qs

    // Dump a circuit out in QASM format
    let DumpQASM (title:string) (circ:Circuit) =
        let sb                  = System.Text.StringBuilder()   // Buffer to build output strings in
        let app (x:string)      = sb.Append x |> ignore         // Append to the output buffer

        let rec appWs (ws:int list) first  =                    // Output list of wires for a gate
            match ws with
            | w::ws    ->
                if not first then app "," 
                sprintf "q%d" w |> app
                appWs ws false
            | _        -> ()
        
        let mapName (nam:string) =                              // Map gate name to desired name
            match (nam.ToLower()) with
            |   "meas"     -> "measure"
            |   nam        -> nam

        let out()               =                               // Ouput the string buffer and clear out
            show "%O" sb
            sb.Length <- 0
        
        let outGate (g:Gate) (ws:int list) (pfx:bool) =         // Output a gate
            let arity   =
                if pfx then // Binary controlled gate 
                    sprintf "%8s    " ("c-" + (mapName g.Name)) |> app
                    g.Arity+1
                else        
                    sprintf "%8s    " (mapName g.Name) |> app
                    g.Arity

            // We could be handed a qubit list of any size... need to prune to real length for this gate
            let ws = Seq.take arity ws |> Seq.toList
            appWs ws true
            out()

        let rec dump (circ:Circuit) =                           // Recursively walk the circuit
            match circ with
            | Seq cs       ->                                   // Sequential gates
                for c in cs do dump c
            | Par cs       ->                                   // Parallel gates
                for c in cs do dump c
            | Apply(g,ws) when g.Op = GateOp.String   ->        // If it's a label, we'll ignore the gate
                ()                                              // We could accumulate these and label them for QASM
            | Apply(g,ws)   ->                                  // Apply a single gate
                outGate g ws false
            | Ext (g,ws,c) ->                                   // Extended gate (don't worry about inner)
                outGate g ws false
            | Wrap (g,ws,c2)  ->                                // Higher level meta gate (dump inner)
                dump c2
            | BitCon (g,ws,_,c2)  ->                            // Binary controlled gate
                match c2 with
                | Apply(g,ws2)      ->                          // Controlled gate
                    outGate g ws true                           // add a prefix to the name
                | Ext(g,ws,c)      ->                           // Controlled gate
                    outGate g ws true                           // add a prefix to the name
                | _                ->
                    outGate g ws true                           // This should be a sophisticated parser 
                                                                // for other types of nested gates that might be here
            | Empty        ->
                show "    nop"

        show "#"
        show "# Sample QASM output file"
        show "#"
        show "# Title: %s" title
        show "# Date:  %O" DateTime.Now
        show "#"
        show ""
        let wires   = circ.Wires()
        for w in wires do show "    qubit   q%d" w
        show ""
        dump circ
        show ""

    [<LQD>]
    let __DumpQASM() =
        let ket     = Ket(3)
        let circ    = Circuit.Compile Teleport ket.Qubits
        DumpQASM "Teleport.qasm" circ

        // Let's dump a QFT circuit
        let ket     = Ket(10)
        let circ    = Circuit.Compile QFT ket.Qubits
        DumpQASM "QFT.qasm" circ
