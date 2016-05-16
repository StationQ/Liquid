## Quantum Challenge 1

This folder contains the winning submissions for the first [MSR Quantum Challenge](http://research.microsoft.com/en-us/projects/liquid/challenge.aspx), held in the spring of 2016.

The Challenge attracted a large number of contestants, with entries from all across the world.
In the end, the judges selected one first prize winner and four second prize winners.
Submissions were judged based on their originality, depth of use of LIQ<i>Ui</i>|&#x232A;, and general readability.

## First Prize

### [Simulating Dynamical Input-Output Quantum Systems with LIQ<i>Ui</i>|&#x232A;](/QuantumChallenge/QC_1/Simulating_Dynamical_Input-Output_Quantum_Systems.pdf)

_Author: Thien Nguyen, Australian National University_

The author demonstrates how a quantum computer can be used to simulate open quantum systems. He applies this to an investigation of a mechanism for building a quantum memory: Controlled Reversible Inhomogeneous Broadening (CRIB). This is applicable to many quantum hardware approaches, including NV centers and quantum dots. Dissipation, which is what distinguishes open quantum systems, is emulated using an innovative gadget based on amplitude-damping noise. The paper provides a readable introduction to the physics involved in open quantum systems and CRIB, as well as a nice overview of the LIQ<i>Ui</i>|&#x232A; simulation.

## Second Prizes

The second prize winners are listed in alphabetical (title) order.

### [Catching Nature in the Act: Real-Time Imaging of Quantum Systems with LIQ<i>Ui</i>|&#x232A;](/QuantumChallenge/QC_1/Catching_Nature_In_The_Act.pdf)

_Author: Ali A. Husain, University of Illinois at Urbana-Champaign_

The author implemented the Heisenberg model using the Hamiltonian simulator, and devised a means of computing two-spin correlation functions using the simulation. The reviewers were impressed by the investigation of the scaling and error susceptibility of the model. Results for both the ferromagnetic and anti-ferromagnetic case are presented and are compared to theory. The paper includes nice visualizations of spin propagation; a video was also provided demonstrating the evolution.

### [Quantum Neural Networks: A Hamiltonian Complexity Approach](/QuantumChallenge/QC_1/Quantum_Neural_Networks.pdf)

_Author: Johannes Bausch, University of Cambridge_

The author developed an F# library that implements a quantum neural network, including both training and classification algorithms. The network takes a set of binary training vectors and builds a spin Hamiltonian. The energy of this Hamiltonian represents a classification. The author demonstrates his library by building a quantum network that recognizes the colors red and blue, given an RGB input. Also included is a nice analysis of the effectiveness of different types of interaction terms in the trained Hamiltonian on the quality of classification and on the training time required.

### [Solving maximally-constrained 1-SAT problems with oracular access](/QuantumChallenge/QC_1/Solving_Maximally-Constrained_1-SAT_Problems.pdf)

_Authors: Vojtěch Havlíček, ETH Zurich; Antony Milne and Andrew Simmons, Imperial College London_

The authors demonstrate how the family of maximally constrained 1-SAT problems displays a striking separation between classical and quantum query complexity with oracular access. They optimize the qubit usage of existing quantum algorithms, implement their scheme fully using LIQ<i>Ui</i>|&#x232A;, and investigate the scaling of the computational resources required. Renderings from LIQ<i>Ui</i>|&#x232A; are integrated along with simulation results and backing theory to present their case.

### [Testing quantum state engineering protocols via LIQ<i>Ui</i>|&#x232A; simulations](/QuantumChallenge/QC_1/Testing_Quantum_State_Engineering_Protocols.pdf)

_Author: Andras Pal Gilyen, Centrum Wiskunde & Informatica_

The author demonstrates some unusual state preparation protocols using post selection. Post selection in quantum algorithms is a critical element of the quantum toolbox, allowing non-unitary operations to be implemented. The first protocol is used to generate the Mandelbrot set using a quantum circuit. Some beautiful visualizations are included. The second protocol implements the quantum Moser-Tardos algorithm to prepare the ground state of a frustration-free Hamiltonian. The author uses the second protocol to test the scaling of the algorithm in the presence of non-commutating Hamiltonian terms.

