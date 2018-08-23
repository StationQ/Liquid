type Steane7 private(_dummy,tgt:Circuit) =
    inherit QECC(6,7,tgt)
    let aCnt    = 6
    let cCnt    = 7

    let mutable numFixed    = 0

    // Here are the logical 0 and 1 codes (for decoding)
    let logical0 = [0x00;0x55;0x33;0x66;0x0F;0x5A;0x3C;0x69]
    let logical1 = List.map (fun c -> c ^^^ 0x7F) logical0

    /// Prep gate for Steane7
    let prep (qs:Qubits) =
        let nam = "S7:Prep"
        let gate (qs:Qubits)    =

            // Create logical |0> prep circuit
            let op (qs:Qubits)  =
                let xH i    = H [qs.[i]]
                let xC i j  = CNOT [qs.[i];qs.[j]]

                xH 6;   xC 6 3; xH 5;   xC 5 2; xH 4
                xC 4 1; xC 5 3; xC 4 2; xC 6 0; xC 6 1
                xC 5 0; xC 4 3

            Gate.Build(nam,fun () ->
                new Gate(
                    Qubits  = qs.Length,
                    Name    = nam,
                    Help    = "Prepare logical 0 state",
                    Draw    = sprintf "\\multigate{#%d}{%s}" (qs.Length-1) nam,
                    Op      = WrapOp op
            ))
        (gate qs).Run qs

    // Fix up a syndrome measurement
    let fix (syn:int) (f:Qubits->unit) (qs:Qubits) =
        let nam             = "S7 Fix"
        let gate (syn:int) (f:Qubits->unit) (qs:Qubits) =
            let parent          = !< f qs
            let op (qs:Qubits)  =
                let b0  = qs.[0].Bit.v
                let b1  = qs.[1].Bit.v
                let b2  = qs.[2].Bit.v
                let bs  = (b0 <<< 2) + (b1 <<< 1) + b2

                // If we match the syndrome, then do the parent
                if bs = syn then 
                    numFixed   <- numFixed + 1
                    //show "@@@       Fix syndrome=%d (%c) on qubit=%d" 
                    //    syn (if qs.[0].Id = 0 then 'Z' else 'X') qs.[3].Id
                    true
                else false
            new Gate(
                Name    = nam,
                Help    = "Fix syndrome measurements",
                Draw    = sprintf "\\cwx[#3]\\control%s\\go[#1]\\control%s\\go[#2]\\control%s"
                            (if syn &&& 4 <> 0 then "" else "o")
                            (if syn &&& 2 <> 0 then "" else "o")
                            (if syn &&& 1 <> 0 then "" else "o")
                            ,
                Op      = BCOp(3,op),
                Parent  = Some parent
            )
        (gate syn f qs).Run qs
            
    // Syndrome measurement
    let synd (qs:Qubits) =
        let nam = "S7:Syn"
        let gate (qs:Qubits)    =

            // Syndrome operations (assume first 6 qubits are ancilla)
            let op (qs:Qubits)  =
                let xH  i           = H [qs.[i]]
                let xXs i js        = 
                    for j in js do
                        CNOT [qs.[i];qs.[j]]
                let xZs i js        = 
                    for j in js do
                        Cgate Z [qs.[i];qs.[j]]
                let xM  i           = M [qs.[i]]
                let xFX syn i       = fix syn X !!(qs,[3;4;5;i])
                let xFZ syn i       = fix syn Z !!(qs,[0;1;2;i])
                let xR  i           = Reset Zero [qs.[i]]

                // Debug dump info
                let dmp (nam:string) =
                    let sb  = StringBuilder()
                    NativeDbg(fun (qs:Qubits) ->
                        sb.Length <- 0
                        for i in 0..5 do
                            match qs.[i].Bit with
                                | Zero -> '0'
                                | One  -> '1'
                                | _    -> '?'
                            |> sb.Append |> ignore
                        show "@@@ Syndrome(%O): %s" sb nam
                        qs.Head.Ket.Dump(showLogInd,3)) qs

                // Measure syndrome
                for i in 0..5 do xH i
                xXs 0 [9;10;11;12]
                xXs 1 [7;8;11;12]
                xXs 2 [6;8;10;12]
                xZs 3 [9;10;11;12]
                xZs 4 [7;8;11;12]
                xZs 5 [6;8;10;12]

                for i in 0..5 do xH i
                for i in 0..5 do xM i

                // Error correct
                for syn in 1..7 do
                    xFX syn (5+syn)
                    xFZ syn (5+syn)

                // Reset ancilla back to zero
                for a in 0..5 do xR a

            Gate.Build(nam,fun () ->
                new Gate(
                    Qubits  = qs.Length,
                    Name    = nam,
                    Help    = "Measure/Fix Syndrome",
                    Draw    = sprintf "\\multigate{#%d}{%s}" (qs.Length-1) nam,
                    Op      = WrapOp op
            ))
        (gate qs).Run qs

    // Compute bit best distance between codes
    let bestCode measured =
        let best logical =
            let rec dist a b v =
                if a = 0 && b = 0 then v
                elif (a &&& 1) ^^^ (b &&& 1) <> 0 then 
                     dist (a>>>1) (b>>>1) (v+1)
                else dist (a>>>1) (b>>>1) v
            List.mapi (fun i c -> i,(dist c measured 0)) logical 
            |> List.minBy (fun (i,d) -> d)
                
        // Find min distance to a logical 0 code
        let best0,dist0 = best logical0
        let best1,dist1 = best logical1
        if dist0 <= dist1 then Zero,dist0 else One,dist1

    /// <summary>
    /// Sample class to do a Steane 7 bit code [[7,1,3]] (derived from QECC)
    /// </summary>
    /// <param name="tgt">Target circuit to convert to QECC</param>
    new(tgt:Circuit) = Steane7(0,tgt)

    /// <summary>
    /// Get prep circuit for logical |0>
    /// </summary>
    override s.Prep qs = prep qs

    /// <summary>
    /// Get syndrome circuit
    /// </summary>
    override s.Syndrome qs = synd qs

    /// <summary>
    /// My replacement gates
    /// </summary>
    override s.Replace(g:Gate) = 
        base.Replace g

    /// <summary>
    /// Decode a coded bit measurement into a logical bit
    /// </summary>
    /// <param name="qs">Physical qubits to decode</param>
    /// <returns>Logical Bit value,distance</returns>
    override s.Decode (qs:Qubits) =
        let measured    = base.GetMeasured qs
        bestCode measured
        
    /// How many fixups were done?
    member s.NumFixed
        with get()  = numFixed
        and  set v  = numFixed <- v

    /// Test QECC on 1 qubit with X,Y and Z injection
    static member Test1() =
        show "Steane7: Performing all single gate injections on all wires of 1 logical qubit"

        let tgt     = Circuit.Compile (fun qs -> M qs) (Ket(1).Qubits)
        let s       = Steane7(tgt)
        let k       = s.Ket
        k.TraceRun <- 0

        let gX      = Gate.GetGate X k.Qubits
        let gY      = Gate.GetGate Y k.Qubits
        let gZ      = Gate.GetGate Z k.Qubits

        for gate in [gX;gY;gZ] do
            for wire in 6..12 do
                let qs              = k.Reset(k.Qubits.Length)
                s.NumFixed         <- 0
                let rec walk (c:Circuit) =
                    match c with
                    | Seq cs           -> List.map (fun c -> walk c) cs |> Seq
                    | Par cs           -> List.map (fun c -> walk c) cs |> Par
                    | Wrap(g,ws,c2) when g.Name = "S7_Prep" -> Seq[c;Apply(gate,[wire])]
                    | _                -> c                    
                let errC    = walk s.Circuit
                let gNam    =
                    if gate = gX then "X"
                    elif gate = gY then "Y"
                    else "Z"
                let nam     = sprintf "%s%02d" gNam wire
                errC.Run qs
                let bit0,dist0  = s.Decode (s.Log2Phys 0)
                show "%s: Fixes=%d (%4A) dist=%d" nam s.NumFixed bit0 dist0 
        show "Steane7: Test done"
        show ""
