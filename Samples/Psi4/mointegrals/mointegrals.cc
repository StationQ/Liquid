#include <libplugin/plugin.h>
#include "psi4-dec.h"
#include <libdpd/dpd.h>
#include "psifiles.h"
#include <libpsio/psio.hpp>
#include <libtrans/integraltransform.h>
#include <libmints/wavefunction.h>
#include <libparallel/parallel.h>
#include <liboptions/liboptions.h>
#include <libmints/mints.h>
#include <libmints/mintshelper.h>
#include <libiwl/iwl.hpp>
#include <libciomr/libciomr.h>

// This allows us to be lazy in getting the spaces in DPD calls
#define ID(x) ints.DPD_ID(x)

INIT_PLUGIN

namespace psi{ namespace mointegrals{

extern "C" int
read_options(std::string name, Options &options)
{
    if (name == "MOINTEGRALS" || options.read_globals()) {
        /*- The amount of information printed
            to the output file -*/
        options.add_int("PRINT", 0);
        options.add_int("TEST", 0);
        options.add_bool("ERASE", true);
        options.add_str("FILNAM", "mol.dat");
        options.add_double("INFO1", 0.0);
        options.add_double("INFO2", 0.0);
    }

    return true;
}


// Access of a row/col in a lower triangular matrix
int rc(int r,int c) {
    if (c<=r) {
	return(r*(r+1)/2+c);
    }
    else {
	return(c*(c+1)/2+r);
    }
}

extern "C" PsiReturnType
mointegrals(Options &options)
{
    /*
     * This plugin shows a simple way of obtaining MO basis integrals, directly from a DPD buffer.  It is also
     * possible to generate integrals with labels (IWL) formatted files, but that's not shown here.
     */
    int print 		= options.get_int("PRINT");
    int test		= options.get_int("TEST");
    double info1	= options.get_double("INFO1");
    double info2	= options.get_double("INFO2");
    std::string filNam	= options.get_str("FILNAM"); 
    bool erase		= options.get_bool("ERASE");

    // Grab the global (default) PSIO object, for file I/O
    boost::shared_ptr<PSIO> psio(_default_psio_lib_);

    // Now we want the reference (SCF) wavefunction
    boost::shared_ptr<Wavefunction> wfn = Process::environment.wavefunction();
    if(!wfn) throw PSIEXCEPTION("SCF has not been run yet!");

    // Quickly check that there are no open shell orbitals here...
    int nirrep  = wfn->nirrep();

    // For now, we'll just transform for closed shells and generate all integrals.
    std::vector<boost::shared_ptr<MOSpace> > spaces;
    spaces.push_back(MOSpace::all);
    IntegralTransform ints(wfn, spaces, IntegralTransform::Restricted);
    ints.transform_tei(MOSpace::all, MOSpace::all, MOSpace::all, MOSpace::all);

    // Use the IntegralTransform object's DPD instance, for convenience
    dpd_set_default(ints.get_dpd_id());

    // Do the one body terms
    int nmo  = Process::environment.wavefunction()->nmo();
    int ntri = nmo * (nmo + 1) / 2;
    double *mo_ints = init_array(ntri);
    IWL::read_one(psio.get(), PSIF_OEI, PSIF_MO_OEI, mo_ints, ntri, 0, 0, outfile);
    //print_array(mo_ints, nmo, outfile);

    // Sort by diagonal Fock operators
    double *mo_fock = init_array(ntri);
    IWL::read_one(psio.get(), PSIF_OEI, PSIF_MO_FOCK, mo_fock, ntri, 0, 0, outfile);
    std::vector<std::pair<double, int> > srt;
    for (int i=0; i<nmo; ++i) {
	srt.push_back(std::make_pair(mo_fock[rc(i,i)],i));
    }
    std::sort(srt.begin(), srt.end());

    // Save the permutation
    int* perm	= init_int_array(ntri);
    for (int i=0; i<srt.size(); ++i) {
        //fprintf(outfile,"@@@DBG Perm[%2d] = %2d %8.4f\n",i,srt[i].second,srt[i].first);
    	perm[srt[i].second] = i;
    }

    std::string	mode	= "a";
    if (erase) mode = "w";
    FILE* fp	= fopen(filNam.c_str(),mode.c_str());
    fprintf(fp,"tst=%d info=%.4f,%.4f",test,info1,info2);
    fprintf(fp," nuc=%.12f",wfn->molecule()->nuclear_repulsion_energy());
    fprintf(fp," Ehf=%.12f",wfn->reference_energy());
    int r = 0;
    int c = 0;
    for (int i=0; i<ntri; i++) {
	int idx	    = rc(r,c);
	double v    = mo_ints[i];
	if (fabs(v) > 1.0e-15) { // Was 1e-6 ... way too big 1e-12 might be Ok
	    int i=perm[c];
	    int j=perm[r];
    	    fprintf(fp," %d,%d=%.12g",i,j,v);
	}
    	if (++c > r) { c=0;r++; }
    }
    free(mo_ints);
    
    /*
     * Now, loop over the DPD buffer, printing the integrals
     */
    dpdbuf4 K;
    psio->open(PSIF_LIBTRANS_DPD, PSIO_OPEN_OLD);
    // To only process the permutationally unique integrals, change the ID("[A,A]") to ID("[A>=A]+")
    global_dpd_->buf4_init(&K, PSIF_LIBTRANS_DPD, 0, ID("[A,A]"), ID("[A>=A]+"),
                  ID("[A>=A]+"), ID("[A>=A]+"), 0, "MO Ints (AA|AA)");

    //fprintf(outfile,"@@@DBG: K Matrix:\n");
    //global_dpd_->buf4_print(&K,outfile,1);
    
    for(int h = 0; h < nirrep; ++h){
        global_dpd_->buf4_mat_irrep_init(&K, h);
        global_dpd_->buf4_mat_irrep_rd(&K, h);
        for(int pq = 0; pq < K.params->rowtot[h]; ++pq){
            int p = K.params->roworb[h][pq][0];
            int q = K.params->roworb[h][pq][1];
            for(int rs = 0; rs < K.params->coltot[h]; ++rs){
                int r 	= K.params->colorb[h][rs][0];
                int s 	= K.params->colorb[h][rs][1];
		double v = K.matrix[h][pq][rs];

		if (fabs(v) >= 1.0e-15) {
		    fprintf(fp," %d,%d,%d,%d=%.12g",
		    	perm[p],perm[r],perm[s],perm[q],v);
		}
	    }        
	}
        global_dpd_->buf4_mat_irrep_close(&K, h);
    }
    fprintf(fp,"\n");
    global_dpd_->buf4_close(&K);
    psio->close(PSIF_LIBTRANS_DPD, PSIO_OPEN_OLD);
    fclose(fp);
    free(perm);

    return Success;
}

extern "C" PsiReturnType
hi()
{
    printf("@@DBG: HI!\n");
    return Success;
}


}} // End Namespaces
