::The first command line number is the number of passes.
::The second command line number is whether to remove anagrams (1 or 0, true or false).
::The third command line number is whether to sort the results. Not applicable if the method below is set to 0 because some custom operations
::can be very long, and to reassure the user stuff is going on the results are output immediately to screen.
::The fourth command line number is whether to use the method call operation or the non-method call operation (1 for the faster non-method call).
::The fifth command line number is the number of the dictionary you wish to use.
::The sixth command line is the number of words.
::The next few numbers are the words and the word lengths. The first number is the minimum of a range, the next is the maximum of a range and the
::third is the number of words. The last value only works if both of the numbers for the range are the same. Otherwise the number of words is the
::maximum part of the range minus the minimum part of the range, inclusive. Still a work in progress and needs improvement.
"FiveWordProblem.exe" 1 1 0 1 0 5 5 5
pause