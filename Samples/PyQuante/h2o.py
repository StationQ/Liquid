#!/usr/bin/env python

import sys
import math
from numpy import set_printoptions 
# from scipy import * 
# from scipy.linalg import * 
from PyQuante.hartree_fock import rhf 
from PyQuante.Molecule import Molecule 
from PyQuante.dft import dft

#from PyQuante.Basis.ccpvdz import basis_data           # orbs,consts: 25,14867
#from PyQuante.Basis.ccpvtz import basis_data           # orbs,consts: ? 
#from PyQuante.Basis.dzvp import basis_data             # orbs,consts: 19, 5465
#from PyQuante.Basis.lacvp import basis_data            # orbs,consts: ?
#from PyQuante.Basis.p321 import basis_data             # orbs,consts: 13, 1449
#from PyQuante.Basis.p631 import basis_data             # orbs,consts: 13, 1449
#from PyQuante.Basis.p631ppss import basis_data         # orbs,consts: 27, 20394
#from PyQuante.Basis.p631ss import basis_data           # orbs,consts: 25, 14868
#from PyQuante.Basis.p6311pp_2d_2p import basis_data    # orbs,consts: ?
#from PyQuante.1asis.p6311pp_3d_3p import basis_data    # orbs,consts: ?
#from PyQuante.Basis.p6311pp_3df_3pd import basis_data  # orbs,consts: ?
#from PyQuante.Basis.p6311ss import basis_data          # orbs,consts: 31, 34824
#from PyQuante.Basis.sto3g import basis_data            # orbs,consts: 7,168
from PyQuante.Basis.sto6g import basis_data            # orbs,consts: 7,168

from PyQuante.Ints import getbasis, getints 
from PyQuante.CI import TransformInts
from PyQuante.cints import ijkl2intindex 
from PyQuante.NumWrap import matrixmultiply, transpose 

dft = 0             # Do DFT?
single = 0          # Set to get detailed info for a single value

def H2O_Molecule(tst,info,auX,auZ):
    H2O = Molecule('H2O',
           [('O',  ( 0.0,  0.0, 0.0)),
            ('H',  ( auX,  0.0, auZ)),
            ('H',  (-auX,  0.0, auZ))],
           units='Bohr')

    # Get a better energy estimate
    if dft:
        print "# info=%s A.U.=(%g,%g) " % (info,auX,auZ)
        edft,orbe2,orbs2 = dft(H2O,functional='SVWN')

    bfs= getbasis(H2O,basis_data=basis_data) 
    #S is overlap of 2 basis funcs 
    #h is (kinetic+nucl) 1 body term 
    #ints is 2 body terms 
    S,h,ints=getints(bfs,H2O) 


    #enhf is the Hartee-Fock energy 
    #orbe is the orbital energies 
    #orbs is the orbital overlaps 
    enhf,orbe,orbs=rhf(H2O,integrals=(S,h,ints))
    enuke   = Molecule.get_enuke(H2O)

    # print "orbe=%d" % len(orbe)

    temp = matrixmultiply(h,orbs) 
    hmol = matrixmultiply(transpose(orbs),temp) 

    MOInts = TransformInts(ints,orbs) 

    if single:
         print "h = \n",h
         print "S = \n",S
         print "ints = \n",ints
         print "orbe = \n",orbe
         print "orbs = \n",orbs
         print ""
         print "Index 0: 1 or 2 in the paper, Index 1: 3 or 4 in the paper (for pqrs)"
         print ""
         print "hmol = \n",hmol
         print "MOInts:"
         print "I,J,K,L = PQRS order: Cre1,Cre2,Ann1,Ann2"

    if 1:
        print "tst=%d info=%s nuc=%.9f Ehf=%.9f" % (tst,info,enuke,enhf),

    cntOrbs    = 0
    maxOrb    = 0
    npts    = len(hmol[:])
    for i in xrange(npts):
        for j in range(i,npts):
            if abs(hmol[i,j]) > 1.0e-7:
                print "%d,%d=%.9f" % (i,j,hmol[i,j]),
        cntOrbs += 1
        if i > maxOrb: maxOrb = i
        if j > maxOrb: maxOrb = j

    nbf,nmo    = orbs.shape
    mos        = range(nmo)
    for i in mos:
        for j in xrange(i+1):
            ij      = i*(i+1)/2+j
            for k in mos:
                for l in xrange(k+1):
                    kl = k*(k+1)/2+l
                    if ij >= kl:
                        ijkl = ijkl2intindex(i,j,k,l)
                        if abs(MOInts[ijkl]) > 1.0e-7:
                            print "%d,%d,%d,%d=%.9f" % (l,i,j,k,MOInts[ijkl]),
            cntOrbs += 1
            if i > maxOrb: maxOrb = i
            if j > maxOrb: maxOrb = j
    print ""

    return (maxOrb,cntOrbs)

def doRun(tst,angleD,lenB): 
    info    = '{0:.1f},{1:.3f}'.format(angleD,lenB)
    rad     = angleD * math.pi / 180.0
    auX     = lenB * math.sin(rad/2.0)
    auZ     = lenB * math.cos(rad/2.0)
    return H2O_Molecule(tst,info,auX,auZ)

def main():
    anglesD     = [104.5225]
    lensA 	= [0.657213 + x/10.0 for x in xrange(0,50)]
    ary         = [(x,y) for x in anglesD for y in lensA]

    tst	= 0
    for angleD,lenA in ary:
	# print "%d: %.5f %.4f" % (tst,angleD,lenA)
	lenB	= lenA * 1.8897161646320723
	maxOrb,cntOrbs	= doRun(tst,angleD,lenB)
	tst	= tst + 1
    print "orbs: %d, constants: %d" % (maxOrb+1,cntOrbs)
    return
            
if __name__ == "__main__": main()
 
