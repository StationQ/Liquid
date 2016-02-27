@echo off
setlocal
for /f %%a in (foo.files) do (
    rem echo #### %%a
    copy "%%a" foo.tmp 2>NUL 1>NUL
    rem sed -e "s:\(\(href\|src\)=.\)[.][.]/[.][.]:\1{{ site.github.url }}:" < foo.tmp > "%%a"
    rem grep "\(\(href\|src\)=.\)/\([a-zA-Z]\)" foo.tmp
    sed -e "s:\(\(href\|src\)=.\)\(/[a-zA-Z]\):\1{{ site.github.url }}\3:" < foo.tmp > "%%a"
)
endlocal