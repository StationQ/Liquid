// Copyright (c) 2015,2016 Microsoft Corporation

#r @"\Liquid\bin\Liquid1.dll" // This is show IntelliSense will work in Visual Studio        

namespace Microsoft.Research.Liquid // Tell the compiler our namespace

open Operations
open Util
open System
open System.Text.RegularExpressions
open System.Collections.Generic
open System.IO
open System.Text

open HamiltonianGates

module Script =                     // The script module allows for incremental loading

    /// <summary>
    /// CXI gate: Controlled XI on consecutive wires (used for Nathan example)
    /// </summary>
    /// <param name="theta">Strength of coupling</param>
    /// <param name="qs">Operate on first 2 qubits after PE</param>
    let CXI (theta:float) (qs:Qubits)  =
        let gate (theta:float) =
            new Gate(
                Name    = "XI",
                Draw    = "\\ctrl{#1}\\go[#1]\\multigate{#1}{" + (theta.ToString("E1")) + "XI}",
                Op      = WrapOp (fun qs ->
                            H qs.Tail
                            CRz theta 1.0 "" qs
                            H qs.Tail
                            )
            )
        (gate theta).Run qs

    /// <summary>
    /// CXX gate: Controlled XX on consecutive wires (used for Nathan example)
    /// </summary>
    /// <param name="theta">Strength of coupling</param>
    /// <param name="qs">Operate on first 2 qubits after PE</param>
    let CXX (theta:float) (qs:Qubits)  =
        let gate (theta:float) =
            new Gate(
                Name    = "XX",
                Draw    = "\\ctrl{#1}\\go[#1]\\multigate{#1}{" + (theta.ToString("E1")) + "XX}",
                Op      = WrapOp (fun qs ->
                            H qs.Tail
                            H qs.Tail.Tail
                            CNOT qs.Tail
                            CRz theta 1.0 "" !!(qs,0,2)
                            CNOT qs.Tail
                            H qs.Tail.Tail
                            H qs.Tail
                            )
            )
        (gate theta).Run qs

    /// <summary>
    /// CYY gate: Controlled YY on consecutive wires (used for Nathan example)
    /// </summary>
    /// <param name="theta">Strength of coupling</param>
    /// <param name="qs">Operate on first 2 qubits after PE</param>
    let CYY (theta:float) (qs:Qubits)  =
        let gate (theta:float) =
            new Gate(
                Name    = "YY",
                Draw    = "\\ctrl{#1}\\go[#1]\\multigate{#1}{" + (theta.ToString("E1")) + "YY}",
                Op      = WrapOp (fun qs ->
                            Ybasis qs.Tail
                            Ybasis qs.Tail.Tail
                            CNOT qs.Tail
                            CRz theta 1.0 "" !!(qs,0,2)
                            CNOT qs.Tail
                            YbasisAdj qs.Tail.Tail
                            YbasisAdj qs.Tail
                            )
            )
        (gate theta).Run qs

    /// <summary>
    /// CYY' gate: Controlled YY on consecutive wires (used for Nathan example)
    /// </summary>
    /// <param name="theta">Strength of coupling</param>
    /// <param name="qs">Operate on first 2 qubits after PE</param>
    let CYYAdj (theta:float) (qs:Qubits)  =
        let gate (theta:float) =
            new Gate(
                Name    = "YY'",
                Draw    = "\\ctrl{#1}\\go[#1]\\multigate{#1}{" + (theta.ToString("E1")) + "YY}",
                Op      = WrapOp (fun qs ->
                            YbasisAdj qs.Tail
                            YbasisAdj qs.Tail.Tail
                            CNOT qs.Tail
                            CRz theta 1.0 "" !!(qs,0,2)
                            CNOT qs.Tail
                            Ybasis qs.Tail.Tail
                            Ybasis qs.Tail
                            )
            )
        (gate theta).Run qs

    /// <summary>
    /// CXZ gate: Controlled XZ on consecutive wires (used for Nathan example)
    /// </summary>
    /// <param name="theta">Strength of coupling</param>
    /// <param name="qs">Operate on first 2 qubits after PE</param>
    let CXZ (theta:float) (qs:Qubits)  =
        let gate (theta:float) =
            new Gate(
                Name    = "XZ",
                Draw    = "\\ctrl{#1}\\go[#1]\\multigate{#1}{" + (theta.ToString("E1")) + "XZ}",
                Op      = WrapOp (fun qs ->
                            H qs.Tail
                            CNOT qs.Tail
                            CRz theta 1.0 "" !!(qs,0,2)
                            CNOT qs.Tail
                            H qs.Tail
                            )
            )
        (gate theta).Run qs

    /// <summary>
    /// CZZ gate: Controlled ZZ on consecutive wires (used for Nathan example)
    /// </summary>
    /// <param name="theta">Strength of coupling</param>
    /// <param name="qs">Operate on first 2 qubits after PE</param>
    let CZZ (theta:float) (qs:Qubits)  =
        let gate (theta:float) =
            new Gate(
                Name    = "ZZ",
                Draw    = "\\ctrl{#1}\\go[#1]\\multigate{#1}{" + (theta.ToString("E1")) + "ZZ}",
                Op      = WrapOp (fun qs ->
                            CNOT qs.Tail
                            CRz theta 1.0 "" !!(qs,0,2)
                            CNOT qs.Tail
                            )
            )
        (gate theta).Run qs

    /// <summary>
    /// CII gate: Controlled II on consecutive wires (used for Nathan example)
    /// </summary>
    /// <param name="theta">Strength of coupling</param>
    /// <param name="qs">Operate on first 2 qubits after PE</param>
    let CII (theta:float) (qs:Qubits)  =
        let gate (theta:float) =
            new Gate(
                Name    = "II",
                Draw    = "\\ctrl{#1}\\go[#1]\\multigate{#1}{" + (theta.ToString("E1")) + "II}",
                Op      = WrapOp (fun qs ->
                            CNOT qs.Tail
                            CGtheta -theta 1.0 !!(qs,0,2)
                            CNOT qs.Tail
                            )
            )
        (gate theta).Run qs

    // Base Unitary to test for linear algebra (Matrix is diagonal 1,2,4,8)
    let CUa0 deltaT (qs:Qubits) =
        let gate deltaT =
            // Multipled by 2 when Rz underneath
            let alpha   = deltaT * 15./4.
            let beta    = deltaT *  3./2.
            let gamma   = deltaT * -9./2.
            let delta   = deltaT * -5./2.
            new Gate(
                Name    = "CUa",
                Draw    = "\\multigate{#2}{CUa}",
                Op      = WrapOp (fun qs ->
                            CII alpha qs
                            CZZ beta qs
                            CRz gamma 1.0 "" !!(qs,0,1)
                            CRz delta 1.0 "" !!(qs,0,2)
                            )
            )
        (gate deltaT).Run qs

    // Base Unitary to test for linear algebra (Matrix is diagonal 1,2,4,8)
    let CUa0Adj deltaT (qs:Qubits) =
        let gate deltaT =
            // Multipled by 2 when Rz underneath
            let alpha   = deltaT * 15./4.
            let beta    = deltaT *  3./2.
            let gamma   = deltaT * -9./2.
            let delta   = deltaT * -5./2.
            new Gate(
                Name    = "CUa",
                Draw    = "\\multigate{#2}{CUa}",
                Op      = WrapOp (fun qs ->
                            Adj (CRz delta 1.0 "") !!(qs,0,2)
                            Adj (CRz gamma 1.0 "") !!(qs,0,1)
                            CZZ beta qs
                            CII alpha qs
                            )
            )
        (gate deltaT).Run qs

    /// <summary>
    /// Controlled rotation gate
    /// </summary>
    /// <param name="k">Rotation by 2^k</param>
    /// <param name="qs">Qubits ([0]=Control [1]=rotated)</param>
    let CR (k:int) (qs:Qubits) = 
        let gate (k:int)  =
            Gate.Build("CR_" + k.ToString() ,fun () ->
                new Gate(
                    Qubits  = qs.Length,
                    Name    = "CR",
                    Help    = "Controled R gate",
                    Draw    = sprintf "\\ctrl{#1}\\go[#1]\\gate{R%d}" k,
                    Op      = WrapOp (fun (qs:Qubits) -> Cgate (R k) qs)
            ))
        (gate k).Run qs

    /// <summary>
    /// Inverse Controlled rotation gate
    /// </summary>
    /// <param name="k">Rotation by 2^k</param>
    /// <param name="qs">Qubits ([0]=Control [1]=rotated)</param>
    let CRAdj (k:int) (qs:Qubits) = 
        let gate (k:int)  =
            Gate.Build("CR'_" + k.ToString() ,fun () ->
                new Gate(
                    Qubits  = qs.Length,
                    Name    = "CR'",
                    Help    = "Controled R' gate",
                    Draw    = sprintf "\\ctrl{#1}\\go[#1]\\gate{R%d^\\dagger}" k,
                    Op      = WrapOp (fun (qs:Qubits) -> Cgate (Adj (R k)) qs)
            ))
        (gate k).Run qs


    /// <summary>
    /// Run phase kick-back on multiple phase qubits
    /// </summary>
    /// <param name="cUa">Controlled by single phase qubit(=0) for 1 Trotter step</param>
    /// <param name="trotterN">Trotter# to use</param>
    /// <param name="qsPE">Phase qubits (0=MSB)</param>
    /// <param name="qsU">Unitary qubits</param>
    let PEkick (cUa:Qubits -> unit) trotterN (qsPE:Qubits) (qsU:Qubits) =
        let ket             = qsU.Head.Ket
        let qCntPE          = qsPE.Length
        let gp              = GrowPars(false,0,0,1)
        for bit in 0..qCntPE-1 do
            let qPE     = qsPE.[qCntPE-(bit+1)]
            let qs      = qPE :: qsU
 
            H qs
            if trotterN = 1 then
                let loops   = pown 2 bit
                for loop in 1..loops do cUa qs
            else
                let circ    = Circuit.Compile cUa qs
                let circ'   = circ.GrowGates ket
                let loops   = trotterN * pown 2 bit
                for loop in 1..loops do circ'.Run qs

    /// <summary>
    /// Run inverse phase kick-back on multiple phase qubits
    /// </summary>
    /// <param name="cUa">Controlled by single phase qubit(=0) for 1 Trotter step</param>
    /// <param name="trotterN">Trotter# to use</param>
    /// <param name="qsPE">Phase qubits (0=MSB)</param>
    /// <param name="qsU">Unitary qubits</param>
    let PEkickAdj (cUa2:Qubits -> unit) trotterN (qsPE:Qubits) (qsU:Qubits) =
        let ket             = qsU.Head.Ket
        let qCntPE          = qsPE.Length
        let gp              = GrowPars(false,0,0,1)
        for bit in qCntPE-1..-1..0 do
            let qPE     = qsPE.[qCntPE-(bit+1)]
            let qs      = qPE :: qsU

            if trotterN = 1 then
                let loops   = pown 2 bit
                for loop in 1..loops do 
                    cUa2 qs
            else
                let circ    = Circuit.Compile cUa2 qs
                let circ'   = circ.GrowGates ket
                let loops   = trotterN * pown 2 bit
                for loop in 1..loops do circ'.Run qs
            H qs

    /// <summary>
    /// Do inverse QFT (bits are reversed on exit)
    /// </summary>
    /// <param name="qsPE">PE qubits, 0=MSB</param>
    let QFTAdj (qsPE:Qubits) =
        let gate =
            let op (qs:Qubits) = 
                for bit in 0..qs.Length-1 do
                    let qPE = qs.[bit]
                    for ctrl in 0..bit-1 do
                        let k       = 1+bit-ctrl
                        let qCtrl   = qs.[ctrl]
                        let qs      = [qCtrl;qPE]
                        CRAdj k qs
                    H [qPE]
            Gate.Build("QFT'_" + qsPE.Length.ToString() ,fun () ->
                new Gate(
                    Qubits  = qsPE.Length,
                    Name    = "QFT'",
                    Help    = "QFT'",
                    Draw    = sprintf "\\multigate{#%d}{QFT'}" (qsPE.Length-1),
                    Op      = WrapOp op
            ))
        gate.Run qsPE

    /// <summary>
    /// Do QFT (bits are reversed on exit)
    /// </summary>
    /// <param name="qsPE">PE qubits, 0=LSB</param>
    let QFT (qsPE:Qubits) =
        let gate =
            let op (qs:Qubits) = 
                for bit in qs.Length-1..-1..0 do
                    let qPE = qs.[bit]
                    H [qPE]
                    for ctrl in bit-1..-1..0 do
                        let k       = 1+bit-ctrl
                        let qCtrl   = qs.[ctrl]
                        let qs      = [qCtrl;qPE]
                        CR k qs
            Gate.Build("QFT_" + qsPE.Length.ToString() ,fun () ->
                new Gate(
                    Qubits  = qsPE.Length,
                    Name    = "QFT",
                    Help    = "QFT",
                    Draw    = sprintf "\\multigate{#%d}{QFT}" (qsPE.Length-1),
                    Op      = WrapOp op
            ))
        gate.Run qsPE

    /// <summary>
    /// For now eigen invert is just a qubit reversal
    /// </summary>
    /// <param name="qs">Eigenvalues to invert</param>
    let Recip (qs:Qubits) =
        let gate =
            let op (qs:Qubits) =
                for i in 0..(qs.Length/2)-1 do
                    let j   = (qs.Length-1) - i
                    if i < j then SWAP !!(qs,i,j)
            Gate.Build("Recip_" + qs.Length.ToString() ,fun () ->
                new Gate(
                    Qubits  = qs.Length,
                    Name    = "Recip",
                    Help    = "Recip",
                    Draw    = sprintf "\\multigate{#%d}{Recip}" (qs.Length-1),
                    Op      = WrapOp op
            ))
        gate.Run qs

    /// <summary>
    /// Do the binary rotation
    /// </summary>
    /// <param name="pwr">Power of denominator of rotation</param>
    /// <param name="qs">Anc + PE qubits</param>
    let BinRot pwr (qs:Qubits) =
        let gate pwr =
            let op (qs:Qubits) =
                let qAnc            = qs.Head
                let qsPE            = qs.Tail
                let mult1           = Math.PI / Math.Pow(2.0,pwr)
                for bit in 0..qsPE.Length-1 do
                    let theta       = mult1 * pown sqrt2 bit
                    let qs          = [qsPE.[bit];qAnc]
                    CRy -theta 1.0 "" qs
            new Gate(
                Qubits  = qs.Length,
                Name    = "BinRot",
                Help    = "BinRot",
                Draw    = sprintf "\\multigate{#%d}{BinRot}" (qs.Length-1),
                Op      = WrapOp op
            )
        (gate pwr).Run qs

    ////////////////////////////////////////////////////////////////////////////
    
    [<LQD>]
    let QLSA() =

        // User paramaters
        let a           = 1.                        // A Matrix parameters
        let b           = 2.
        let c           = 3.
        let d           = 4.
        let bVec        = [1./sqrt 2.;1./sqrt 2.]   // B vector parameters
        let qCntPE      = 4                         // PE qubits
        let trotterN    = 1
        let eMax        = 16.
        let eMin        =  0.

        // Compute deltaT for phase estimation
        let omega       = eMax-eMin
        let tTotal      = (2.0 * Math.PI)/omega
        let deltaT      = tTotal / (float trotterN)
        
        let qCntUa      = 2
        let cUa         = CUa0  deltaT
        let cUa'        = CUa0Adj deltaT
        
        let qCnt    = qCntPE+qCntUa+1               // Total qubit count (with ancilla)
        let gp      = GrowPars(false,0,0,1)         // Used for sparseToDense

        let rec doIter r iter =

            // Initial registers (PE and Ua)
            let ket                     = Ket(qCntPE+qCntUa)
            
            // Add an Ancilla at the end 
            let qAnc                    = ket.Add Zero
            let qs                      = ket.Qubits

            // Get the qubits for each register
            let qsPE                    = !!(qs,[0..qCntPE-1])
            let qsU                     = !!(qs,[qCntPE..qCnt-2])

            // Start all Ua states with 0.5 probability (|00> |01> |10> |11>)
            H >< qsU

            let bin qCnt x =
                let rec loop tst rslt =
                    if tst = 0 then rslt+">"
                    elif x &&& tst = tst then loop (tst/2) (rslt + "1")
                    else loop (tst/2) (rslt + "0")
                loop (1<<<(qCnt-1)) "|"

            if r = 0 && iter = 0 then
                show "================== Initial probs:"
                Array.iteri (fun i p -> if p > 1.0e-5 then show "PE joint prob %12s = %.9f" (bin qsPE.Length i) p) (ket.Probs(qsPE))
                Array.iteri (fun i p -> if p > 1.0e-5 then show " U joint prob %12s = %.9f" (bin qsU.Length i) p) (ket.Probs(qsU))

                let opRender (qs:Qubits) =
                    let qsPE    = Seq.take qCntPE qs |> Seq.toList
                    let qsU     = Seq.skip qCntPE qs |> Seq.take qCntUa |> Seq.toList
                    let qAnc    = qs.[qs.Length-1]
                    List.iteri (fun i q -> LabelL (sprintf "PE%d" i) [q]) qsPE
                    List.iteri (fun i q -> LabelL (sprintf "B%d" i) [q]) qsU
                    LabelL "Anc" [qAnc]
                    PEkick cUa 1 qsPE qsU
                    QFTAdj qsPE
                    Recip qsPE
                    let r               = 2
                    let pwr             = float (qCnt+r-2)/4.0
                    BinRot pwr (qAnc :: qsPE)
                    M [qAnc]
                    QFT qsPE
                    PEkickAdj cUa' 1 qsPE qsU
                let k'      = ket.Copy()
                let circ    = Circuit.Compile opRender k'.Qubits
                circ.RenderHT("QLSA0",0,33.,100.)
                circ.RenderHT("QLSA9",9,20.,33.)
                let circ'   = circ.RemoveRedund()
                circ'.RenderHT("Redund")

            // Phase Estimate
            PEkick cUa trotterN qsPE qsU
            QFTAdj qsPE

            if r = 0 && iter = 0 then
                // Fake measure the bits to check if the  PE worked correctly (0=LSB)
                let ms  = 
                    ket.Probs(qsPE)
                    |> Array.mapi (fun i p -> i,p)
                    |> Array.sortBy (fun (i,p) -> -p)
                show "Top 4 eigenvalues in state vector:"
                for top in 0..3 do
                    if top < ms.Length then
                        let phase,prob  = ms.[top]
                        let phi         = (float phase) / (float (1<<<qCntPE))
                        let phi2p   = phi * (2.0*Math.PI)
                        let eigen   = 
                            let eigen   = omega * phi
                            if eigen >= eMin && eigen <= eMax then eigen
                            elif eigen > eMax then eigen - omega
                            else eigen+omega
                        show "  %d:PHI = %10.8f = %10.8f radians eigen = %12.8f (prob = %7.5f)" top phi phi2p eigen prob

            // Invert the eigenvalues
            Recip qsPE

            // Binary rotation expansion (getting theta_j right is critical)
            let pwr             = float (qCnt+r-2)/4.0
            BinRot pwr (qAnc :: qsPE)
            
            // Give up if we failed
            M [qAnc]
            if qAnc.Bit = Zero && iter < 499 then doIter r (iter+1)

            else
                // Inverse Phase Estimate
                QFT qsPE
                PEkickAdj cUa' trotterN qsPE qsU

                show "%2d: MEASURED a %d for r =%4d (1/2^%.2f) %s" iter qAnc.Bit.v r pwr
                    (if qAnc.Bit = One then "" else "   (#### DIDN'T CONVERGE, GIVING UP!!!!! ####)")

                show "================== Final probs:"
                Array.iteri (fun i p -> if p > 1.0e-5 then show "PE joint prob %12s = %.9f" (bin qsPE.Length i) p) (ket.Probs(qsPE))
                let vs  =
                    Array.mapi (fun i p -> 
                        if p > 1.0e-5 then show " X joint prob %12s = %.9f" (bin qsU.Length i) p
                        p
                        ) (ket.Probs qsU)
                show "CSV,%d,%.2f,%d,%.5f,%.5f,%.5f,%.5f" qAnc.Bit.v pwr iter vs.[0] vs.[2] vs.[1] vs.[3]

        show "CSV,Good,r,Iters,p0,p1,p2,p3"
        for r in 0..24 do doIter r 0
 
        show "Done" 
  