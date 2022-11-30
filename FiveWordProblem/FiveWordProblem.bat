::The first command line is the anagram (works only with method 0).
::The second command line is the number of passes.
::The third command line is whether to remove anagrams (0 = no anagrams, 1 = anagrams).
::The fourth command line is whether to sort the results.
::The fifth command line number is whether to use the method call operation or the non-method call operation (0 for the faster non-method call).
::The sixth command line number is the number of the dictionary you wish to use.
::The seventh command line is the number of words.
::The next few numbers are the words and the word lengths (only applicable to method 0). The first number is the minimum of a range, the next is
::the maximum of a range and the third is the number of words. Still a work in progress.

::I AM STILL YET TO FIGURE OUT THE CORRECT 'PUSH' VALUES FOR MULTI-LENGTH ANAGRAMS TO GRAB ALL RESULTS. YOU CAN MANUALLY CHANGE THE PUSH VALUES BY
::ENTERING 5 ADDITIONAL NUMBERS AFTER. THE PUSH FOR THE STAND-UP MATHS PROBLEM IS 2-2-2-2-2, SO YOU ENTER 2 2 2 2 2 AT THE END OF THE COMMAND LINE.
::BUT THIS IS THE DEFAULT: IT IS TELLING IT TO CONSIDER THE NEXT 2 AVAILABLE CHARACTERS, BECAUSE IT IS PERMITTED TO ONLY SKIP ONE. SO EVERY ADDED
::WORD CONSIDERS TWO POSSIBLE CHARACTERS AND THE CORRESPONDING DICTIONARY FOR THAT CHARACTER, AND IF 1 IS PUSHED TO 2 AT ANY STAGE THE ONES AFTER IT
::ARE ALL EFFECTIVELY REDUCED TO 1. SINCE THAT IMPLIES THE 1 BEFORE IT HASN'T PUSHED TO 2, THAT MEANS IT WILL PROCEED LIKE THIS: 2-1-1-1-1, 1-2-1-1-1,
::1-1-2-1-1, 1-1-1-2-1 AND 1-1-1-1-2. 26-26-26-26-26 WILL ALWAYS CONSIDER ALL 26 CHARACTERS IF THE ANAGRAM IS 26 UNIQUE LETTERS LONG. IF THE ANAGRAM IS
::5 UNIQUE LETTERS LONG, THEN IT IS 5-5-5-5-5. BUT THIS MAY BE OVERKILL.

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