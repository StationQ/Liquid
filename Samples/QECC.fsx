// Copyright (c) 2015,2016 Microsoft Corporation

#if INTERACTIVE
#r @"..\bin\Liquid1.dll"            // Tell fsi where to find the Liquid dll
#else
namespace Microsoft.Research.Liquid // Tell the compiler our namespace
#endif

open System                         // Open any support libraries

open Microsoft.Research.Liquid      // Get necessary Liquid libraries
open Util                           // General utilites
open Operations                     // Basic gates and operations
open Tests                          // Just gets use the RenderTest call for dumping files

module Script =                     // The script module allows for incremental loading

    // Teleport for Stabilizers and QECC
    let tele0 (qs:Qubits)   = teleport qs; M [qs.[2]]
    let tele1 (qs:Qubits)   = X qs; teleport qs; M [qs.[2]]

    [<LQD>]
    let QECC() =
        let tCnt    = 3         // Target Count
        let cCnt    = 7         // Code Count per target qubit
        let aCnt    = 6         // Ancilla count

        show "Dumping tiny example (single X gate)"
        let k       = Ket(1)
        let circ    = Circuit.Compile (fun qs -> X qs) k.Qubits
        let s7      = Steane7(circ)
        let s7c     = s7.Circuit
        s7c.Fold().Render("QECC_tin.htm","svg",1)
        s7c.Fold().Render("QECC_tin.tex","tikz",1)

        let k       = Ket(tCnt)
        let tgtC0   = Circuit.Compile tele0 k.Qubits
        let tgtC1   = Circuit.Compile tele1 k.Qubits
        let qs      = k.Reset(tCnt)
        tele0 qs
        show "tele0 returns: [%d%d] => %d" qs.[0].Bit.v qs.[1].Bit.v qs.[2].Bit.v
        let qs      = k.Reset(tCnt)
        tele1 qs
        show "tele1 returns: [%d%d] => %d" qs.[0].Bit.v qs.[1].Bit.v qs.[2].Bit.v
        show "##############################################"

        tgtC0.Fold().Render("QECC_tgt.htm")
        let s7      = Steane7(tgtC0)
        let prep    = s7.Prep
        let qs3     = Ket(cCnt).Qubits
        let prepC   = Circuit.Compile prep qs3
        prepC.Fold().Render("QECC_prp.htm")
        let syn     = s7.Syndrome
        let qs5     = Ket(aCnt+cCnt).Qubits
        let synC    = Circuit.Compile syn qs5
        synC.Fold().Render("QECC_syn.htm")
        let s7C     = s7.Circuit
        show "Dumping QECC circuit to log"
        s7C.Dump()
        s7C.Fold().RenderHT("QECC_min",0,100.0,33.0)
        s7C.Fold().RenderHT("QECC_all",1,50.0,33.0)

        show ""
        show "   folded agressive parallel gateCount"
        show "                                %5d" (s7C.GateCount())
        show "                       X        %5d" (s7C.GateCount(true))
        show "     X                          %5d" (s7C.Fold().GateCount())
        show "     X                 X        %5d" (s7C.Fold().GateCount(true))
        show "     X        X                 %5d" (s7C.Fold(true).GateCount())
        show "     X        X        X        %5d" (s7C.Fold(true).GateCount(true))
        show ""

        let _               = k.Reset()
        let stab            = Stabilizer(tgtC1,k)
        stab.Run()
        let _,b0            = stab.[0]
        let _,b1            = stab.[1]
        let _,b2            = stab.[2]
        show "tele1 stabilizer: [%d%d] => %d" b0.v b1.v b2.v
        show ""
        show "=== Final State: "
        stab.ShowState showInd 0
        stab.Gaussian()
        show "=== After Gaussian: "
        stab.ShowState showInd 0
        show ""

    #if FALSE
        s7.Ket.TraceRun   <- 2
        s7C.Run s7.Ket.Qubits
    #endif

        for inp in [Zero;One] do
            let tgtC        = if inp = Zero then tgtC0 else tgtC1
            let s7          = Steane7(tgtC)
            let s7C         = s7.Circuit
            let ket         = s7.Ket
            ket.TraceRun   <- 0
            for iter in 0..9 do
                s7.NumFixed        <- 0
                let prob            = 5.0e-3
                let errC,stats      = 
                    if iter = 0 then s7C,[0;0;0] else 
                        let rec genErrC() =
                            let errC,stats  = s7.Inject prob
                            if List.sum stats > 0 then errC,stats
                            else genErrC()
                        genErrC() 
                let tag     = sprintf "%s%d" (inp.ToString().Substring(0,3)) iter
                let fName   = sprintf "QECC_err_%s.htm" tag
                errC.Fold().Render(fName,"svg",0)

                let stab            = Stabilizer(errC,ket)
                stab.Run()

                if iter = 0 then
                    show ""
                    show "=== Final State: "
                    stab.ShowState showInd 0
                    stab.Gaussian()
                    show "=== After Gaussian: "
                    stab.ShowState showInd 0
                    show ""

                let bit0,dist0  = s7.Log2Phys 0 |> s7.Decode
                let bit1,dist1  = s7.Log2Phys 1 |> s7.Decode
                let bit2,dist2  = s7.Log2Phys 2 |> s7.Decode
                show "LOOP[%s]: InjectedXYZ(%d,%d,%d) Fixes=%d (%4O,%4O,%4O) dist=(%d,%d,%d)%s" 
                    tag
                    stats.[0] stats.[1] stats.[2]
                    s7.NumFixed
                    bit0 bit1 bit2
                    dist0 dist1 dist2
                    (if bit2 <> inp then " <====== BAD" else "")
        show "doQECC Done"


#if INTERACTIVE
do Script.QECC()        // If interactive, then run the routine automatically
#endif
