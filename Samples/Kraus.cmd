@echo off
rem #######################################################################
rem Sample script to run a batch of Teleports with Kraus operator noise
rem "grep" and "tee" are Unix style commands that can be obtained off the Internet
rem #######################################################################

setlocal
del kraus.csv
set intvl=.00,.02,.04,.06,.08,.10,.12,.14,.16,.18
for %%a in (%intvl%) do (
    for %%b in (%intvl%) do (
        ..\bin\liquid.exe "__Kraus(1000,%%a,%%b,false)" | grep CSV | tee -a kraus.csv
    )
)

endlocal