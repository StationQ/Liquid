namespace Microsoft.Research.Liquid

module UserSample =
    open System
    open Util
    open Operations
    //open Native             // Support for Native Interop
    //open HamiltonianGates   // Extra gates for doing Hamiltonian simulations
    //open Tests              // All the built-in tests

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
                    CSMat(2,[0,0,c,0.;0,1,-s,0.;1,0,s,0.;1,1,c,0.])),
                Draw    = "\\gate{" + nam + "}"
                )
        (gate theta).Run qs

    [<LQD>]
    let __UserSample() =
         show "This module is a good place to put compiled user code"

module Main =
    open App

    /// <summary>
    /// The main entry point for Liquid.
    /// </summary>
    [<EntryPoint>]
    let Main _ =
        RunLiquid ()
