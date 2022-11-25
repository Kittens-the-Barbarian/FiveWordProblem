::The first command line number is the number of passes.
::The second command line number is whether to remove anagrams (1 or 0, true or false).
::The third command line number is whether to sort the results.
::The fourth command line number is whether to use the method call operation or the non-method call operation (1 for the faster non-method call).
::The fifth command line number is the number of the dictionary you wish to use.
::The sixth command line is the number of words.
::The next few numbers are the words and the word lengths: if you to find a 3 word combination of 7 letters, you would type in 7 7 7.
::[For the above, you will need to set the method call operation (0) and a full dictionary (currently 4). And why would you remove anagrams/sort
::unless you were gunning for 5 word problem speed records? So X 0 1 0 4 7 7 7]
"FiveWordProblem.exe" 1 1 0 1 0 5 5 5 5 5
pause