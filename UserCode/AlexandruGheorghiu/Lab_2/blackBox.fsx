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

module BlackBox =                     // The script module allows for incremental loading


    /// <summary>
    /// Helper function to convert a number to binary (as a list of booleans). 
    /// </summary>
    /// <param name="num">Number to convert to binary</param>
    /// <param name="numBits">Number of bits in the representation</param>
    let numToBools (num:int) (numBits:int) =
        let mutable tempNum = num 
        let mutable lst = []
        for i in 1..numBits do
            lst <- [(tempNum % 2) = 1] @ lst
            tempNum <- tempNum / 2 
        lst

    /// <summary>
    /// Function from Lab_1 which flips the phase of the |00..0> state (leaving other basis states unchanged)
    /// </summary>
    /// <param name="qs">Qubits</param>
    let phaseFlip (qs:Qubits) =
        let gate =
            let nam     = "Phlip"
            new Gate(
                Name    = nam,
                Help    = sprintf "Flip phase of |00..0> state",
                Mat     = (
                    let n = qs.Length
                    let pow2n = 1 <<< n
                    let mutable lst = [(0,0,-1.0,0.0)]
                    for i in 1 .. (pow2n - 1) do
                        lst <- lst @ [(i,i,1.0,0.0)]
                    CSMat(pow2n, lst)),
                Draw    = "\\gate{" + nam + "}"
                )
        gate.Run qs

    /// <summary>
    /// Same as phaseFlip but flips the phase of a particulare state |x>
    /// </summary>
    /// <param name="qs">Qubits</param>
    let xPhaseFlip (x:int) (qs:Qubits) =
        let lst = numToBools x (qs.Length)
        for i in 0 .. (qs.Length - 1) do
            if (lst.[i]) then
                X [qs.[i]]
        phaseFlip qs
        for i in 0 .. (qs.Length - 1) do
            if (lst.[i]) then
                X [qs.[i]]

    /// <summary>
    /// Quantum oracle for some function f. Implements the unitary U_f |x>|y> -> |x>|f(x) xor y> 
    /// </summary>
    /// <param name="qs">List of qubits on which unitary acts</param>
    /// <param name="f">Oracle function to implement as unitary</param>
    let unitaryFn (qs:Qubits) (f:(Boolean list) -> Boolean) =
        let lim = qs.Length - 1
        let numInputs = (1 <<< lim)
        let mutable sparseElems = []
        for i in 0..(numInputs - 1) do
            let lst = numToBools i lim
            if (f lst) then
                sparseElems <- sparseElems @ [(i <<< 1,(i <<< 1) + 1,1.,0.); ((i <<< 1) + 1,i <<< 1,1.,0.)]
            else
                sparseElems <- sparseElems @ [(i <<< 1, i <<< 1, 1., 0.); ((i <<< 1) + 1, (i <<< 1) + 1, 1., 0.)] 
        let gate =
            new Gate(
                Name    = "U_f",
                Help    = sprintf "Apply U_f for some function f",
                Mat     = CSMat(1 <<< qs.Length, sparseElems),
                    Draw    = "\\multigate{#" + lim.ToString() + "}{U_f}"
                )
        gate.Run qs

    // Constant functions
    let constant0 (qs:Qubits) =
        unitaryFn qs (fun _ -> false)
    let constant1 (qs:Qubits) =
        unitaryFn qs (fun _ -> true)

    // Balanced functions
    let bitParity (qs:Qubits) =
        unitaryFn qs (fun lst -> List.fold (fun acc elem -> (<>) acc elem) false lst)    
    let numParity (qs:Qubits) =
        unitaryFn qs (fun lst ->
                        let rev = List.rev lst
                        rev.Head = true)


//////////////////////////////////////////////////////////////////////////////
// Deutsch-Josza oracles

    // Constant 0 function for 5 qubit input 
    let fun1 (qs:Qubits) =
        if (qs.Length = 5) then
            constant0 qs
        else
            failwith "Incorrect number of qubits"

    // Constant 0 function for 7 qubit input
    let fun2 (qs:Qubits) =
        if (qs.Length = 7) then
            constant0 qs
        else
            failwith "Incorrect number of qubits"

    // Bit parity (balanced function) for 10 qubit input
    let fun3 (qs:Qubits) =
        if (qs.Length = 10) then
            bitParity qs
        else
            failwith "Incorrect number of qubits"

    // Constant 0 function for 6 qubit input
    let fun4 (qs:Qubits) =
        if (qs.Length = 6) then
            constant0 qs
        else
            failwith "Incorrect number of qubits"

    // Parity of binary number (balanced function) for 8 qubit input
    let fun5 (qs:Qubits) =
        if (qs.Length = 8) then
            numParity qs
        else
            failwith "Incorrect number of qubits"
//////////////////////////////////////////////////////////////////////////////
// Elitzur-Vaidman bomb detector oracles    

    let candidate1 (qs:Qubits) = I qs
    let candidate2 (qs:Qubits) = CNOT qs
    let candidate3 (qs:Qubits) = CNOT qs
    let candidate4 (qs:Qubits) = CNOT qs
    let candidate5 (qs:Qubits) = I qs            
    let candidate6 (qs:Qubits) = CNOT qs
//////////////////////////////////////////////////////////////////////////////
// Grover oracles    
// Note that oracles are of the form U_f |x> = (-1)^f(x) |x>

    let oracle1 (qs:Qubits) = (xPhaseFlip 2 qs)
    let oracle2 (qs:Qubits) = (xPhaseFlip 7 qs)
    let oracle3 (qs:Qubits) = (xPhaseFlip 6 qs)
    let oracle4 (qs:Qubits) = (xPhaseFlip 0 qs)
    let oracle5 (qs:Qubits) = (xPhaseFlip 15 qs)

    let bigOracle (qs:Qubits) = (xPhaseFlip 1147 qs)

    // program entry point
    [<LQD>]
    let blackBox()    =
        show ""

#if INTERACTIVE
do BlackBox.blackBox()        // If interactive, then run the routine automatically
#endif
