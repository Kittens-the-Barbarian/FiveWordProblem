::The first command line number is the number of passes.
::The second command line number is whether to remove anagrams (1 or 0, true or false).
::The third command line number is whether to sort the results.
::The fourth command line number is whether to use the method call operation or the non-method call operation (1 for the faster non-method call).
::The fifth command line number is the number of the dictionary you wish to use.
::TEST: The sixth command line is the number of words.
::TEST: The seventh command line is the number of letters.
::[For the above two, you will need to set the method call operation (0) and a full dictionary (currently 4). For instance, if you wanted to look
::For combinations of 3 words of 6 letters each, you would need X X X 0 4 3 6 ... In order to properly scan across, because 6*3 is 18, you probably
::need to shift right 6 for each word and you add 1 to that, so X X X 0 4 3 6 X 7 7 7. It will probably work without the last 3 numbers as it should
::insert them by default. However, some combinations involving the full (default) shift are far too time consuming.]
::The eight command line number is a test option. Change it to whatever you would like to test.
::Numbers 9-13 are experimental.
"FiveWordProblem.exe" 1 1 0 1 0 5 5 0 2 2 2 2 2
pause