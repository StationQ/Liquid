## Sample code from Martin Roetteler
 
Examples:
* [Peres.fsx](#Peres): Source code for a simple POVM game
 
### <a name="Peres"></a>Peres.fsx
 
With the new POVM features added to LIQ<i>Ui</i>|&#x232A;, I felt it would be nice to show a simple game inspired by a quantum detection problem due to Holevo [1] and Peres/Wootters [2]. 
In the game, a player A thinks of a number (0,1 or 2) and the opponent, player B, tries to guess any number *but* the one chosen by player A. 
Classically, if you just made a guess, you'd have to ask two questions to be right 100% of the time. If instead, player A prepares a qubit with 0, 1, or 2 encoded into three single 
qubit states that are at an angle of 120 degrees with respect to each other and then hands the state to the opponent, then player B can apply a POVM consisting of 3 states 
that are perpendicular to the states chosen by player A. It can be shown that this allows B to be right 100% of the time with only 1 measurement, which is something that is 
not achievable with a von Neumann measurement on 1 qubit. 
 
See also the book [3, p.287] by Peres for a nice description of the optimal POVM. 
 
You can compile and run the sample with: `\Liquid\bin\Liquid.exe /s Peres.fsx Peres(100)`

[1] A. Holevo, “Information-theoretical aspects of quantum measurement,” Problems of Information Transmission, vol. 9, no. 2, pp. 110–118 (1973)

[2] A. Peres and W. K. Wootters, “Optimal detection of quantum information,” Phys. Rev. Lett.,
vol. 66, pp. 1119-1122, Mar. 1991.

[3] A. Peres, “Quantum Theory: Concepts and Methods,” Kluwer Academic Publishers, 2002. 
