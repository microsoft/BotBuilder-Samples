@echo off
setlocal

call lerna run build
set ds=..\..\cli\bin\run

call node %ds% dialog:generate:test transcripts/sandwich.transcript sandwich -o oracles
call node %ds% dialog:generate:test transcripts/addItemWithButton.transcript msmeeting-actions -o oracles
call node %ds% dialog:generate forms/unittest_temperature_with_units.form -o %temp%\unittest_temperature_with_units -t templates -t template:standard -T addOne
copy %temp%\unittest_temperature_with_units\unittest_temperature_with_units.dialog oracles