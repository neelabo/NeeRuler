$makepri = "C:\Program Files (x86)\Windows Kits\10\bin\x64\makepri.exe"

& $makepri new /pr Resources\ /cf priconfig.xml/ /of resources.pri /in NeeView /o
& $makepri dump /if resources.pri /of resources.pri.xml /o
