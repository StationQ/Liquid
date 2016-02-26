@echo off
setlocal
for /f %%a in (foo.files) do (
    copy "%%a" foo.tmp
    sed -e "s:\(\(href\|src\)=.\)[.][.]/[.][.]:\1{{ site.github.url }}:" < foo.tmp > "%%a"
)
endlocal