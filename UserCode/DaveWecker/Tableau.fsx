// Copyright (c) 2015,2016 Microsoft Corporation

#r @"\Liquid\bin\Liquid1.dll" // This is show IntelliSense will work in Visual Studio        

namespace Microsoft.Research.Liquid // Tell the compiler our namespace

open Operations
open Util
open System
open System.Text

module Tableau =
    // Define an EPR function
    let EPR (qs:Qubits) = 
        H qs; CNOT qs

    // Sample Teleport circuit
    let teleport (qs:Qubits) =
        let q0,q1,q2    = qs.[0],qs.[1],qs.[2]      // Extract 3 qubits

        // Give names to the first three qubits
        LabelL "Src" [q0]
        LabelL "\\ket{0}" [q1]
        LabelL "\\ket{0}" [q2]

        EPR[q1;q2]; CNOT qs; H qs               // EPR 1,2, then CNOT 0,1 and H 0
        M[q1]; BC X [q1;q2]                     // Conditionally apply X
        M[q0]; BC Z [q0;q2]                     // Conditionally apply Z
        LabelR "Dest" [q2]                      // Label output

    // Teleport a |1>
    let tele1 (qs:Qubits)   = X qs; teleport qs; M [qs.[2]]

    // Output string building
    let sb              = StringBuilder()
    let app (x:string)  = sb.Append x |> ignore

    // Tableau helper functions
    let pw                      = Array.init 32 (fun i -> 1 <<< i)
    let tst(vs:int[,],i,idx,bit)    = vs.[i,idx] &&& bit <> 0
    let msk                     = 0x1F
     
    // Stand alone version of Stabilizer.ShowState
    let showState (f:int->string->unit) level n (rs:int[]) (xs:int[,]) (zs:int[,])  =
        let out()   = sb.ToString() |> f level; sb.Length <- 0
        for i in 0..2*n-1 do
            if i = n then
                out()
                for j in 0..n do app "-"
            out()
            if rs.[i] = 2 then app "-" else app "+"
            for j in 0..n-1 do
                let jIdx    = j >>>5
                let jBit    = pw.[j&&&msk]
                match tst(xs,i,jIdx,jBit),tst(zs,i,jIdx,jBit) with
                | false,false  -> app "."
                | true,false   -> app "X"
                | true,true    -> app "Y"
                | false,true   -> app "Z"
        out()

    [<LQD>]
    let Tableau() =
        let k       = Ket(3)
        let tgtC1   = Circuit.Compile tele1 k.Qubits
        let qs      = k.Qubits
        let stab     = Stabilizer(tgtC1,k)
        stab.Run()
        show "Internal version:"
        stab.ShowState showInd 0
        show "Script version:"
        let n,rs,xs,zs  = stab.Tableau
        showState showInd 0 n rs xs zs
