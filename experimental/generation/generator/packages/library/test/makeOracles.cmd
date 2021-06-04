@echo off
setlocal

call lerna run build
set ds=..\..\cli\bin\run

call node %ds% dialog:generate:test transcripts/sandwich.transcript sandwich -o oracles
call node %ds% dialog:generate:test transcripts/addItemWithButton.transcript msmeeting-actions -o oracles

call node %ds% dialog:generate forms/unittest_transforms.form -o %temp%\unittest_transforms -t templates -t templates/override -t template:standard -T addOne
copy %temp%\unittest_transforms\unittest_transforms.dialog oracles

call node %ds% dialog:generate merge_data/sandwichMerge.form -p sandwichMerge -o %temp%\merge
call node %ds% dialog:generate merge_data/sandwichMerge-modified.form -p sandwichMerge -o %temp%\merge --merge
copy %temp%\merge\sandwichMerge.dialog oracles
