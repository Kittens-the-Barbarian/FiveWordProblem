::The first command line number is the number of passes.
::The second command line number is whether to remove anagrams (1 or 0, true or false).
::The third command line number is whether to sort the results.
::The fourth command line number is whether to use the method call operation or the non-method call operation (1 for the faster non-method call).
::The fifth command line number is the number of the dictionary you wish to use.
::The sixth command line is the number of words.
::The seventh command line is the number of letters.
::[For the above two, you will need to set the method call operation (0) and a full dictionary (currently 4). And why would you remove anagrams/sort
::unless you were gunning for 5 word problem speed records? For instance, if you wanted to look for combinations of 3 words of 6 letters each, you
::should use X 0 1 0 4 3 6 X with only the first and last numbers, marked by X, unaffected.]
::The eight command line number is a test option. Change it to whatever you would like to test.
::Numbers 9-13 are experimental.
"FiveWordProblem.exe" 1 1 0 1 0 5 5 0
pause