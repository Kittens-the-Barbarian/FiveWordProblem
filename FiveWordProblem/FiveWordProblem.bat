::The first command line is the anagram (works only with method 0).
::The second command line is the number of passes.
::The third command line is whether to remove anagrams (0 = no anagrams, 1 = anagrams).
::The fourth command line is whether to sort the results.
::The fifth command line number is whether to use the method call operation or the non-method call operation (0 for the faster non-method call).
::The sixth command line number is the number of the dictionary you wish to use.
::The seventh command line is the number of words.
::The next few numbers are the words and the word lengths (only applicable to method 0). The first number is the minimum of a range, the next is
::the maximum of a range and the third is the number of words. Still a work in progress.

::NOTE: MULTI-LENGTH/MULTI-WORD ANAGRAMS IN THIS PROGRAM ARE INFERIOR TO SOMETHING I DONE UP ELSEWHERE. I WILL PROBABLY BE SEEKING TO REMOVE IT
::OR REPLACE IT IN THE FUTURE. IT IS WORKING, IT IS JUST 10 TIMES SLOWER THAN IN MY OTHER PROGRAM BECAUSE A BITWISE THINGS NEEDS TO BE REMOVED
::AND I'M NOT CONFIDENT IN THE RESULTS.

::The below will generate single word anagrams from the selected input of 6-6 letters.
::"FiveWordProblem.exe" "teaser" 1 1 1 1 4 6 6 1

::The below will generate single word anagrams from the selected input of 3-6 letters.
::"FiveWordProblem.exe" "teaser" 1 1 1 1 4 6 6 1

::The below will generate two word anagrams from the selected input of 2-4 letters.
::"FiveWordProblem.exe" "teaser" 1 1 1 1 4 2 4 2

::The default method is the five word problem from the Stand-Up Maths videos. Looks for 5 words of 5 letters (range of 5-5) with anagrams turned
::off, an optimised dictionary of only 5 letter words and the quick non-method call operation.
"FiveWordProblem.exe" "abcdefghijklmnopqrstuvwxyz" 1 0 0 0 0 5 5 5

pause