// Copyright (c) 2015,2016 Microsoft Corporation

#if INTERACTIVE
#r @"/home/andru/Liquid/bin/Liquid1.dll"
#else
namespace Microsoft.Research.Liquid // Tell the compiler our namespace
#endif

open System                         // Open any support libraries

open Microsoft.Research.Liquid      // Get necessary Liquid libraries
open Util                           // General utilites
open Operations                     // Basic gates and operations
open Tests                          // Just gets us the RenderTest call for dumping files

module Script =                     // The script module allows for incremental loading

    /// <summary>
    /// Performs an arbitrary rotation around X.
    /// </summary>
    /// <param name="theta">Angle to rotate by</param>
    /// <param name="qs">The head qubit of this list is operated on.</param>
    let rotX (theta:float) (qs:Qubits) =
        let gate (theta:float) =
            let nam     = "Rx" + theta.ToString("F2")
            new Gate(
                Name    = nam,
                Help    = sprintf "Rotate in X by: %f" theta,
                Mat     = (
                    let phi     = theta / 2.0
                    let c       = Math.Cos phi
                    let s       = Math.Sin phi
                    CSMat(2,[0,0,c,0.;0,1,0.,s;1,0,0.,s;1,1,c,0.])),
                Draw    = "\\gate{" + nam + "}"
                )
        (gate theta).Run qs

    /// <summary>
    /// Performs an arbitrary rotation around Y.
    /// </summary>
    /// <param name="theta">Angle to rotate by</param>
    /// <param name="qs">The head qubit of this list is operated on.</param>
    let rotY (theta:float) (qs:Qubits) =
        let gate (theta:float) =
            let nam     = "Ry" + theta.ToString("F2")
            new Gate(
                Name    = nam,
                Help    = sprintf "Rotate in Y by: %f" theta,
                Mat     = (
                    let phi     = theta / 2.0
                    let c       = Math.Cos phi
                    let s       = Math.Sin phi
                    CSMat(2,[0,0,c,0.;0,1,s,0.;1,0,-s,0.;1,1,c,0.])),                    
                Draw    = "\\gate{" + nam + "}"
                )
        (gate theta).Run qs

    // This operation creates the specialKet state (see below) from the |00...0> state.
    // WARNING! Do not use this function for anything else! It doesn't implement a unitary operation!
    let silly (qs:Qubits) =
        let gate =
            let nam     = "Silly"
            new Gate(
                Name    = nam,
                Help    = sprintf "Nothing useful",
                Mat     = (
                    let l2 = sqrt(1./4. + 1./(4.*sqrt(2.)))
                    let l3 = sqrt(1./4. - 1./(4.*sqrt(2.)))
                    let n1 = 1. / sqrt(2. + 2.*(sqrt(2.) - 1.)*(sqrt(2.) - 1.))
                    let n2 = 1. / sqrt(2. + 2.*(sqrt(2.) + 1.)*(sqrt(2.) + 1.))
                    CSMat(16,[0,0,0.5,0.;12,0,0.5,0.;1,0,-l2 * n1,0.;5,0,l2 * n1 * (-1. + sqrt(2.)),0.;9,0,l2 * n1 * (-1. + sqrt(2.)),0.;
                    13,0,l2 * n1,0.;2,0,-l3 * n2,0.;6,0,l3 * n2 * (-1. - sqrt(2.)),0.;10,0,l3 * n2 * (-1. - sqrt(2.)),0.;14,0,l3 * n2,0.;])),
                Draw    = "\\gate{" + nam + "}"
                )
        gate.Run qs

    // State designed to produce correlations between Alice and Bob's measurements in the E91 protocol but also correlate with
    // a third party (Eve) who is trying to learn Alice and Bob's outcomes.
    let specialKet =
        let k = Ket(4)
        let qs = k.Qubits
        silly qs
        k

// Implement your functions here
///////////////////////////////////////////
    
///////////////////////////////////////////


    // program entry point
    [<LQD>]
    let Lab_3()    =
        show "Lab 3"

#if INTERACTIVE
do Script.Lab_3()        // If interactive, then run the routine automatically
#endif
