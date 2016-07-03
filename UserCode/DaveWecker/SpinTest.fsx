// Copyright (c) 2015,2016 Microsoft Corporation

#r @"\Liquid\bin\Liquid1.dll" // This is show IntelliSense will work in Visual Studio        

namespace Microsoft.Research.Liquid // Tell the compiler our namespace

open Operations
open Util
open System
open System.Text
open System.Collections.Generic
open System.IO

module SpinTest =
    /// <summary>
    /// Executes a set of simulation runs for a spin Hamiltonian.
    /// </summary>
    /// <param name="trotter">The Trotter number to use.</param>
    /// <param name="schedule">The annealing schedule to use.
    /// Each entry in the list is a tuple whose first entry is a time step and whose second entry is an 
    /// array of annealing values.
    /// Annealing values for time steps in between entries are computed by linearly interpolating
    /// between those for the previous and next entries.
    /// The list must be in ascneding order by time step.</param>
    /// <param name="res">The resolution of the simulation, in time steps. 
    /// Larger values may increase speed but will reduce the granularity of output.
    /// Note that this must evenly divide the final time in the annealing schedule.</param>
    /// <param name="spin">The actual Hamiltonian to be simulated.</param>
    let Test(trotter:int,schedule:(int*float[]) list,res,spin:Spin) =
        let sb      = StringBuilder()
        let prv     = ref ""
        let tFinal  = List.map (fun x -> match x with a,_ -> a) schedule |> List.max
        if tFinal % res <> 0 then failwith (sprintf "Error in Spin.Test(): time should be a multiple of res=%A" res)
  
        spin.Prep()
        spin.trotterN     <- trotter
        spin.time         <- 0

        for loop in 1..(tFinal/res) do
            spin.Run(res,schedule) |> ignore
            let curPcnt     = int (spin.time * 100 / tFinal)
            let cur         =
                sb.Length  <- 0
                List.iter (fun (q:Qubit) -> 
                    let p   = q.Prob1
                    let c   =
                        if   p >= 0.7 then '1'
                        elif p >= 0.6 then '+'
                        elif p >= 0.4 then '.'
                        elif p >= 0.3 then '-'
                        else               '0'
                    sb.Append c |> ignore
                ) (spin.Ket.Qubits)
                sb.ToString()
            let e,e2 = spin.EnergyExpectation(true)
            let S    = spin.Ket.Entropy(2,spin.Ket.Count/2)
            show "%3d%%: %s [<H>=%3.3f Stdev %2.3f] [S_mid=%3.3f]" curPcnt cur e e2 S

            prv := cur

            if loop = 1 then
                showLog "Dump of full to log:"
                spin.lastRawCirc.Dump()
                showLog "Dump of grown to log:"
                spin.currentCirc.Dump()
                let cF  = spin.lastRawCirc.Fold()
                show "Rendered spin.htm and spinG.htm (.tex as well)"
                cF.Render("spin.htm","svg",9,100.,100.)
                cF.Render("spin.tex","tikz",9,50.,50.)
                let cF  = spin.currentCirc.Fold()
                cF.RenderHT("spinG",9,100.,100.)

        // Final results
        spin.Run(1,schedule) |> ignore
        let e,e2 = spin.EnergyExpectation(true)
        let S    = spin.Ket.Entropy(2,spin.Ket.Count/2)
        show "100%%: %s" !prv
        show "<H>=%3.9f Stdev=%2.6f Midpoint S=%3.6f" e e2 S
        show "Final probs:"
        for q in spin.Ket.Qubits do show "    %8.5f" q.Prob1
        showLog "Dumping final state:"
        spin.Ket.Dump()

    [<LQD>]
    let Ferro() =
        let sCnt            = 12
        let h0              = 0.0
        let hn              = 0.0
        let J               = 1.0
        let res             = 1
        let lst             = sCnt-1
        let hs              = Dictionary<int,float>()
        let Js              = Dictionary<int*int,float>()
        let gammalambda     = [(100,0.0,1.0)]
        hs.[0]           <- 1.0
        hs.[lst]         <- -1.0
        for i in 0..lst-1 do Js.[(i,i+1)] <- J
        let sched           = List.map (fun x -> match x with a,b,c -> (a,[|b;c|])) gammalambda
        let spin            = Spin(hs,Js)

        Test(4,sched,res,spin)
