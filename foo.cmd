@echo off
setlocal
for /f %%a in (foo.files) do (
    rem echo #### %%a
    copy "%%a" foo.tmp 2>NUL 1>NUL
    rem sed -e "s:\(\(href\|src\)=.\)[.][.]/[.][.]:\1{{ site.github.url }}:" < foo.tmp > "%%a"
    rem sed -e "s:\(\(href\|src\)=.\)\(/[a-zA-Z]\):\1{{ site.github.url }}\3:" < foo.tmp > "%%a"
    sed -e "s/'UA-6[0-9]*-1'/'UA-73250818-1'/" < foo.tmp > "%%a"
    rem grep "'UA-6[0-9]*-1'" foo.tmp
)
endlocal