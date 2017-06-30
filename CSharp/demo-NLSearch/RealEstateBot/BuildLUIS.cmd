@echo off
if exist Dialogs\RealEstate.json goto generate
..\core\Search.Tools.Extract\bin\%1\netcoreapp1.0\win10-x64\Search.Tools.Extract.Exe realestate-sample listings 5D990AEFD184FB9DD91B50C9F72CAA03 -g dialogs\histogram.json -h dialogs\histogram.json -af description -ak da8ffa26e92049078d15882b878a8da6 -o dialogs\RealEstate.json -a city,region,district,type,status -dc price -dn price -dk description

:generate
..\core\Search.Tools.Generate\bin\%1\netcoreapp1.0\win10-x64\Search.Tools.Generate.exe Dialogs\RealEstate.json -tf ..\core\Search.Tools.Generate\SearchTemplate.json -o Dialogs\RealEstateModel.json 
echo add -u
