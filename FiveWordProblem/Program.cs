//I am a late bloomer in programming. Ii is a long story, but I needed it to try and get out of a mentally abusive situation that ended in my
//brothers suicide in January 2021. I'm not the most intelligent person, I find learning about this stuff extremely intimidating and extremely
//hard, but I am pushing on despite my limitations. I am not educated in any way in programming. I have certainly never earnt a cent from programming.
//In fact, this is my first in many areas: it is my first GitHub submission; it is my first program that I have successfully implemented multi-
//threading in (through Parallel.For); it is what started me on the path of using bitwise (kind of worked on this and an anagram program in tandem);
//it is my first C# console app (until now, had been using WinForms only); and it is the first time I have ever submitted code publicly.
//
//I do not find learning easy. I tried watching tutorials, but they don't move at the pace I need them to. They spend too long on uninteresting stuff,
//then go too quickly in other areas, and frankly I think I learn better without distraction. I do not learn by reading, it bores me. I only ever
//read bits and pieces here and there when I need them. While I have been programing in C# for about 2 years now, much of that time has been obsessing
//on problems not strictly related to programming (for example, I have been wrestling over arbitrary precision decimal root operations for aeons), so
//I do not consider I would have what would amount to anywhere near 2 years worth of programming experience in C#.
//
//I am contributing this late for a few reasons: 1) it is the only C# contender; 2) it seems I may have gotten it efficient, but who knows what it will
//test at (given some CPU's seem to start up slower going by my two laptops) so might as well share what I have learnt; 3) I am not good at my syntax,
//maybe this can be significantly condensed, but I do feel verbose programming like mine may be easier for others who're not professionals who don't
//have years of education in the area to understand and I'm hoping this will help others who look at the other code submissions and start to seizure
//and bleed from the nose as I do. I think the main purpose of this is as a teaching aid.
//
//Regards, Kirk Dressler.

using System.Text;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using static System.Net.Mime.MediaTypeNames;

namespace FiveWordProblem
{
    internal class Program
    {
        //The dictionary List of List of 'wordnums'. This is a List of 'dictionaries' for each letter of the alphabet, so 26 in total (0-25). The
        //full wordlist will be sorted into these dictionaries indexed by their least frequently occuring letter. Each word will be stored as a custom
        //variable I have named 'wordsnums'. This variable contains the string representation of the word and the number representation of the world
        //that we will later perform bitwise operations on.
        static List<List<wordsnums>> dict = new List<List<wordsnums>>();

        //ConcurrentBag is a type of List variable that allows for parallel processing. Results are not sorted in the order they are originally threaded,
        //so will come out pretty much randomly. The below is the initiating of the results and find5 lists. Results is just for the results as you might
        //have guessed. find5 is to store the hash versions of the combinations, which is then searched through to prevent repetitions of differently
        //ordered versions of the same combination. Early in my testing, I perceived it was more efficient to search through a list of hashes than it was
        //a list of strings so I done this. I didn't apply the same thorough testing I now use so should revisit. I believe it is faster initializing
        //these lists globally.
        static ConcurrentBag<string> results = new ConcurrentBag<string>();
        static ConcurrentBag<int> find5 = new ConcurrentBag<int>();

        //Sets whether to remove anagrams or not. I would not use this is you wanted all the results, but Matt Parker and all the speediest methods I
        //believe used this as they are competing for speed. The output should be 538 results with anagrams removed and 831 with anagrams preserved. I
        //have taken the words_alpha_five list from another project and it is generating this number for both. In fact, I will be adding some words from
        //the other solutions Matt Parker said others had, which feature words not in this English dictionary.
        static bool anagrem = true;

        //Sets which operation to process: 1 = process1() without the method calls, 0 = process2() with the method calls. Anything else for test().
        static uint process = 1;

        //Whether to sort the results alphabetically. By default off as this adds processing...
        static bool sort = false;

        //Custom word and letter numbers. This will only work with method2() and dictionary[4], so the fourth digit in the commandline needs to be set
        //to 0 and the fifth digit in the commandline should be 4. More may be needed, see .bat file.
        static int wordn = 5;
        static uint lettern = 5;

        //I explain this more later.
        static List<uint> max = new List<uint>();

        //Initializes an array that will contain the letters of the alphabet and their order.
        static int[] bin = new int[26];

        //Just a test option for the command line.
        static bool test = false;

        //I have been getting different times when running the release build (this should be built in release mode of course to get significantly better
        //performance) with Visual Studio 2022 running and running the .exe without. In general, after 1000 tests, running without Visual Studio is
        //slower for some reason. But I see evidence it might be faster, or as fast, in the early passes and it has a faster initial boot run. For some
        //reason, the average decreases when run from Visual Studio, but increases when run without. Saw the below solution for people who experienced
        //faster runtimes in a profiler. But tests show this has little or no benefit to this program. I will keep it in anyway as the tests did show
        //maybe 1ms difference and in any event didn't make it slower in my tests.

        /// <summary> native TimeBeginPeriod() from Windows API.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"),
         System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
        private static extern uint TimeBeginPeriod(uint uMilliseconds);

        /// <summary> native TimeEndPeriod() from Windows API.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"),
         System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
        private static extern uint TimeEndPeriod(uint uMilliseconds);

        //To new programmers, and it is pretty new to myself even, a struct/class seems to be basically a user custom variable that possesses
        //multiple variables. If you need to create a variable that holds two or more properties, you do this. This struct in this case has
        //variables for string (the string representation of the word) and uint (the unsigned integer representation of the word, eg. ints that can't
        //have a negative value). It is the uint value that we use bitwise operations on. I was getting up to 5ms faster times using structs over
        //classes.
        public struct wordsnums
        {
            public string word { get; set; }
            public uint bin { get; set; }
        }

        static void Main(string[] args)
        {
            List<double> elapsed = new List<double>();

            //These variables set the amount of letters each next# array picks up of the available letters. By default, it is 2 for each of the 4
            //added words to to which it pertains. I have a theory that if you grab more letters, you could grab more combinations, but it may
            //only apply to larger dictionaries. I just basically want this here in case I want to test things out and the curious results I had
            //with my even zanier dictionary. You can ignore this and I may get rid of it after testing stuff out. But if you're interested,
            //basically what happens is for the first word, you consume 5 letters of the alphabet. That leaves 21 remaining that you can then use
            //for searching your second word. If you look for the first available letter, then that word will take up 5 more letters leaving 16,
            //and then 11, and then 6. Not sure what the full combination is, it isn't 21-16-11-6, because it stores the preceding value.

            for (int i = 0; i < wordn; i++)
            { max.Add(2); }

            //This is handling the various command line arguments. My first time doing this.
            uint rep = 1;
            uint dictionary = 0;
            if (args.Length > 0)
            {
                rep = Convert.ToUInt32(args[0]);
                if (args.Length > 1)
                {
                    if (args[1] == "0") { anagrem = false; }
                    if(args.Length > 2)
                    {
                        if (args[2] != "0") { sort = true; }
                        if (args.Length > 3)
                        {
                            process = Convert.ToUInt32(args[3]);
                            if (args.Length > 4)
                            {
                                dictionary = Convert.ToUInt32(args[4]);
                                if (args.Length > 5)
                                {
                                    wordn = Convert.ToInt32(args[5]);
                                    for (int i = 0; i < wordn - 5; i++)
                                    { max.Add(2); }
                                    max.RemoveRange(wordn, max.Count - wordn);
                                    if (args.Length > 6)
                                    {
                                        lettern = Convert.ToUInt32(args[6]);
                                        if (args.Length > 7)
                                        {
                                            if (args[7] != "0") { test = true; }
                                            for (int i = 0; i < wordn; i++)
                                            {
                                                if (args.Length > i + 8)
                                                {
                                                    max[i] = Convert.ToUInt32(args[i + 8]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }                           
                        }
                    }
                }
            }

            wordn--;

            //if(test)
            {
            }

            for (int i = 0; i < rep; i++)
            {
                Thread.Sleep(1);
                //Creating and starting the stopwatch.
                Stopwatch sw = new Stopwatch();
                sw.Start();

                //Initiating that thing that probably does nothing for nothings sake.
                //TimeBeginPeriod(1);

                //Sets the letter order. bin[0] is 'a', bin[25] is 'z'. So if I set bin[0] to 25, I am saying 'a' should be treated as the last letter
                //of the alphabet, and setting bin[25] to 0, it is saying 'z' should be set as the first letter of the alphabet.
                bin[0] = 25;
                bin[1] = 7;
                bin[2] = 14;
                bin[3] = 13;
                bin[4] = 24;
                bin[5] = 5;
                bin[6] = 9;
                bin[7] = 12;
                bin[8] = 22;
                bin[9] = 2;
                bin[10] = 8;
                bin[11] = 17;
                bin[12] = 11;
                bin[13] = 18;
                bin[14] = 21;
                bin[15] = 10;
                bin[16] = 0;
                bin[17] = 20;
                bin[18] = 23;
                bin[19] = 16;
                bin[20] = 19;
                bin[21] = 4;
                bin[22] = 6;
                bin[23] = 1;
                bin[24] = 15;
                bin[25] = 3;

                //I believe it is faster to generate these lists as global variables above, then clear them each iteration.
                dict.Clear();
                if (File.Exists(System.IO.Directory.GetCurrentDirectory() + @"\Files\dictionary[" + dictionary.ToString() + "].txt"))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(System.IO.Directory.GetCurrentDirectory() + @"\Files\dictionary[" + dictionary.ToString() + "].txt");
                    for (int i2 = 0; i2 < 26; i2++)
                    {
                        dict.Add(new List<wordsnums>());
                    }
                    uint ind = 0;

                    //Skips over the first commented line in the dictionary.
                    sr.ReadLine();

                    while (!sr.EndOfStream)
                    {
                        string word = sr.ReadLine();

                        //This check is needed in case you insert a larger dictionary which I wanted to allow the user to do.
                        if (word.Length == lettern)
                        {
                            //Since there are probably no words with capital letters in the file, this is unneeded.
                            //string word2b = word.ToLower();
                            uint bin = ConvToUInt(0, word);

                            //This check is needed for all 5 letter words to check whether they have duplicate letters. If they have less than 5 bits,
                            //it means at least 1 letter is a duplicate.
                            if (countSetBits(bin) == lettern)
                            {
                                int digs = lowestdigit(bin);
                                dict[digs].Add(new wordsnums { word = word, bin = bin });
                                ind++;
                            }
                        }
                    }
                }

                //Remove anagrams from the words if this option is chosen. This option was used by Matt Parker to save time. It is no longer necessary,
                //but this is what people are doing for the competitive nature of it. If you were serious about the results, you would turn it off.
                if (anagrem)
                {
                    for (int i2 = 0; i2 < dict.Count; i2++)
                    {
                        //Sorts the dictionary by the uint representations of the word. Every anagram of the same length should have the same number.
                        //Anagrams with double letters could have the same number as words of shorter lengths, but this is not important here as this
                        //problem only concerns 5 letter words.
                        dict[i2].Sort(delegate (wordsnums c1, wordsnums c2) { return c1.bin.CompareTo(c2.bin); });
                        //Removes every word from the list that matches the number above it.
                        //Changed this to count down, to remove items from the top of the list first.
                        for (int i3 = dict[i2].Count - 1; i3 > 0; i3--) { if (dict[i2][i3].bin == dict[i2][i3 - 1].bin) { dict[i2].RemoveAt(i3); } }
                    }
                }

                /*
                //This is used to output the reduced wordlist to console for creating a more efficient dictionary. Not to be used in speed testing.
                StringBuilder bd0 = new StringBuilder();
                foreach (List<wordsnums> lst in dict)
                {
                    foreach (wordsnums wrd in lst)
                    {
                         bd0.Append(String.Join("\r\n", wrd.word) + "\r\n");
                    }
                }
                //Outputting this file to console caused me to miss a lot of words. It was cut short and caused an hour+ of problem solving. My best
                //guess: console won't output the over 10,000 words, just around 9,000 of them? Something happened with the console anyway, such just
                //set a breakpoint at break; and retrieve the string from the variable in debug and replace the \r\n with enters using RegEx in a
                //program like TED Notepad. It is better it is sorted in that program too.
                string output = bd0.ToString();
                break;
                */

                results.Clear();
                find5.Clear();

                //Process the results. This is the main part of the program.
                if (process == 1)
                {
                    process1();
                }
                else
                if (process == 0)
                {
                    process2();
                }
                else
                {
                    test1();
                }

                //Building the results into a StringBuilder variable, and sort them if the option is set.
                StringBuilder bd = new StringBuilder();
                if (sort)
                {
                    List<string> results2 = results.ToList();
                    results2.Sort();

                    foreach (string str in results2)
                    {
                        if (results2.Count > 0)
                        {
                            bd.Append(String.Join("\r\n", str) + "\r\n");
                        }
                    }
                }
                else
                {
                    foreach (string str in results)
                    {
                        if (results.Count > 0)
                        {
                            bd.Append(String.Join("\r\n", str) + "\r\n");
                        }
                    }
                }

                //Output the results.
                Console.Write(bd.ToString());

                //Stop the timer.
                sw.Stop();
                elapsed.Add(sw.Elapsed.TotalSeconds);

                //Write the output to console. Optional.
                Console.Write("Count: " + results.Count().ToString() + "\r\n");
                Console.Write("Runs: " + elapsed.Count().ToString() + "\r\n");
                Console.Write("Current: " + elapsed[elapsed.Count()-1].ToString() + "\r\n");
                Console.Write("Average: " + elapsed.Average().ToString("0.#######") + "\r\n");
                Console.Write("Minimum: " + elapsed.Min().ToString() + "\r\n");
                Console.Write("Maximum: " + elapsed.Max().ToString() + "\r\n");

                //That thing that does nothing is now turned off.
                //TimeEndPeriod(1);
            }
        }

        private static void process1()
        {
            //This sets the first two letters for the first iteration. 0 would be 'a' and 1 would be 'b' in an A-Z alphabet. But because this is an
            //'alphabet' sorted by lowest frequency, these first two represent the least frequent letters.
            int[] next0 = new int[max[0]];
            for (int i = 0; i < next0.Length; i++)
            {
                next0[i] = i;
            }

            //Parallel.For is for multithreaded processing. For whatever reason, doing two Parallel.For loops within one another seemed to tweak the
            //performance. I am not entirely clear on what the nextDegreeOfParallelism is, but I don't believe it is the same as how many threads
            //your processor has. Since there is just 2 iterations of the first loop, I have set that to 2. I seemed to get optimal performance out
            //of setting the other to 7 times the number of processors. If you wish to use Parallel.For, you need to be mindful at least that
            //variables need to be thread safe. Especially if you're going to alter variables, you may need to use ConcurrentBag/ConcurrentQueue/
            //ConcurrentDictionary variables. This is my first time actually getting Parallel.For to work at significantly improving speeds! The non-
            //parallel for loops are commented out and retained if you wish to experiment with them.
            //for (int a1 = 0; a1 < 2; a1++)
            Parallel.For(0, next0.Length, new ParallelOptions { MaxDegreeOfParallelism = next0.Length }, a1 =>
            {
                //for (int i1 = 0; i1 < dict[next0[a1]].Count; i1++)
                //Parallel.For(0, dict[next0[a1]].Count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 7 }, i1 =>
                Parallel.ForEach(dict[next0[a1]], new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 7 }, i1 =>
                {
                    //Storing of the word/number combination.
                    wordsnums word0 = i1;
                    uint bin1 = word0.bin;
                    //The next1 arrays are arrays that store the next two least frequently used letters in the alphabet that are still available
                    //from the remaining letters after discounting the words that have already been amassed. The second one 'pushes' it across one
                    //so that it grabs all 26 letters of the alphabet always, because otherwise it would only look for the first 25 and would stop
                    //at the first letter it fails to match a valid combination with. Whether this grabs all combinations as people claim? I don't
                    //know, I have a feeling it doesn't, but it probably does for the dictionary that was used, which is larger than reasonable.
                    //Instead of using the unuseddigits method, I think it is slightly faster, even if ugly, to just repeat the code. Kept the method
                    //calls anyway, just commented out.
                    //int[] next1 = unuseddigits(bin1, max[1]);
                    int[] next1 = new int[max[1]];
                    int i = 0;
                    for (next1[i] = 0; next1[i] < 26; next1[i]++)
                    {
                        //This is bitshifting to the left to detect a value of 1. Since bit shifting pertains to bits, this is working on the binary.
                        //So it starts at 1, then bit shifts to 10, then bit shifts to 100, then bit shifts to 1000, and so on. It will do this for
                        //26 letters of the alphabet based on the order of least frequent to most frequent. Assuming a standard alphabet, and given
                        //next1[i] is the numeric identifier for the character in the alphabet, it is shifting left firstly with 'a' (0), 'b' (1), 'c'
                        //(2), etc. just adjusted to the the frequency 'alphabet'.
                        int m = 1 << next1[i];
                        //The above, 'm' is the letter in binary. The 'bin1' value is the binary value of the combinations of words to this point. The
                        //& symbol means that it is performing a 'bit and' operation on the binary representation of the word. If the binary of two
                        //1's align, it results in a value of 1: it means that letter position is occupied (used). If the bin1 value is 0, then it
                        //results in 0 (for some reason) and then enters that number as the first missing letter in the array. Then it looks for
                        //another until i = 2.
                        if ((bin1 & m) == 0)
                        {
                            //Heck, this confused me for a bit. So it needs clarifying. The setting of the next1[i] value is done at the loop level
                            //above. It is continually set as it goes through the alphabet, 0-25. If a missing letter is found, a 1 is added to the
                            //'i' variable. This switches it from the first value (now permanently set) to the second value, to be set in the 'for'
                            //iteration. This second value (next[1]) gets set to the same value the first one (next[0]) with next[i] = next[max1 - 1].
                            if (i == max[1] - 1) { break; }
                            else { i++; next1[i] = next1[i - 1]; }
                        }
                    }

                    //Goes for the Parellel.For loops above but the comments are becoming too heavy. Notice how each 'for' loop within a 'for' loop
                    //has an a2 and i2. The 'a#' loops are iterations through the next1 arrays, so each of the loops have a nextimum of 2 iterations
                    //if the max# values haven't been changed. But it carries through the previous iteration count, eg. int a2 = a1. So if a1 is
                    //shifted across 1, then so too is each subsequent operation and therefore each iteration is reduced to 1. This may potentially not
                    //grab all results?
                    for (int a2 = a1; a2 < next1.Length; a2++)
                    {
                        //This is the iteration through each dictionary for each word indexed by the least common letter. It is looking through the
                        //dictionaries that match the next available letter.
                        //for (int i2 = 0; i2 < dict[next1[a2]].Count; i2++)
                        foreach (wordsnums i2 in dict[next1[a2]])
                        {
                            wordsnums word1 = i2;
                            //This 'bitwise and' operation is checking to see if any letters from the new word that overlap the former combination.
                            if ((word1.bin & bin1) == 0)
                            {
                                //The below is performing a 'bitwise or' operation using the |. This is adding the two words onto one anothers bit
                                //representation. It will add a 1 next to each letter that was not represented in the previous word combination to
                                //this point. 1 + 1 will be 1 and 0 + 1 will also be 1. Since it is binary, there is no 2. There is also no way of
                                //accounting for duplicate characters, which means for anagrams (does not pertain to here) you need to do something
                                //else but it helps immensely as a check for anagrams before looping through the letters of a word for instance.
                                uint bin2 = bin1 | word1.bin;
                                //Again, the unusuedDigits() method call has been replaced by the code here to set the next2, next3, next4, etc.
                                //arrays.
                                //int[] next2 = unuseddigits(bin2, max[2]);
                                int[] next2 = new int[max[2]];
                                i = 0;
                                for (next2[i] = 0; next2[i] < 26; next2[i]++)
                                {
                                    int m = 1 << next2[i];
                                    if ((bin2 & m) == 0)
                                    {
                                        if (i == max[2] - 1) { break; }
                                        else { i++; next2[i] = next2[i - 1]; }
                                    }
                                }

                                for (int a3 = a2; a3 < next2.Length; a3++)
                                {
                                    //for (int i3 = 0; i3 < dict[next2[a3]].Count; i3++)
                                    foreach (wordsnums i3 in dict[next2[a3]])
                                    {
                                        wordsnums word2 = i3;
                                        if ((word2.bin & bin2) == 0)
                                        {
                                            uint bin3 = bin2 | word2.bin;
                                            //int[] next3 = unuseddigits(bin3, max[3]);
                                            int[] next3 = new int[max[3]];
                                            i = 0;
                                            for (next3[i] = 0; next3[i] < 26; next3[i]++)
                                            {
                                                int m = 1 << next3[i];
                                                if ((bin3 & m) == 0)
                                                {
                                                    if (i == max[3] - 1) { break; }
                                                    else { i++; next3[i] = next3[i - 1]; }
                                                }
                                            }

                                            for (int a4 = a3; a4 < next3.Length; a4++)
                                            {
                                                //for (int i4 = 0; i4 < dict[next3[a4]].Count; i4++)
                                                foreach (wordsnums i4 in dict[next3[a4]])
                                                {
                                                    wordsnums word3 = i4;
                                                    if ((word3.bin & bin3) == 0)
                                                    {
                                                        uint bin4 = bin3 | word3.bin;
                                                        //int[] next4 = unuseddigits(bin4, max[4]);
                                                        int[] next4 = new int[max[4]];
                                                        i = 0;
                                                        for (next4[i] = 0; next4[i] < 26; next4[i]++)
                                                        {
                                                            int m = 1 << next4[i];
                                                            if ((bin4 & m) == 0)
                                                            {
                                                                if (i == max[4] - 1) { break; }
                                                                else { i++; next4[i] = next4[i - 1]; }
                                                            }
                                                        }

                                                        for (int a5 = a4; a5 < next4.Length; a5++)
                                                        {
                                                            //for (int i5 = 0; i5 < dict[next4[a5]].Count; i5++)
                                                            foreach (wordsnums i5 in dict[next4[a5]])
                                                            {
                                                                wordsnums word4 = i5;
                                                                if ((word4.bin & bin4) == 0)
                                                                {
                                                                    //if it reaches this point, it has identified 5 words that do not share any two
                                                                    //letters. It then begins the admittedly slow operation of adding the words into
                                                                    //a list, sorting that list, then storing that combination into a list that ca
                                                                    //then be searched.
                                                                    List<string> find = new List<string>();
                                                                    find.Add(word0.word);
                                                                    find.Add(word1.word);
                                                                    find.Add(word2.word);
                                                                    find.Add(word3.word);
                                                                    find.Add(word4.word);
                                                                    find.Sort();
                                                                    
                                                                    //This stringbuilder operation did seem to improve the results a few milliseconds.
                                                                    //If you are new to programming, strings are immutable: they cannot be edited.
                                                                    //When you appear to edit a string, you are in fact creating new strings. So for
                                                                    //every += or + in a string, you are doing something that needs to continually
                                                                    //make copies of that string. StringBuilder makes it more efficient. It is
                                                                    //especially crucial for much larger strings, but here it appears to save a few
                                                                    //milliseconds you wouldn't otherwise notice despite needing to convert back into
                                                                    //a string?
                                                                    StringBuilder find2 = new StringBuilder();
                                                                    find2.Append(find[0]);
                                                                    find2.Append(" ");
                                                                    find2.Append(find[1]);
                                                                    find2.Append(" ");
                                                                    find2.Append(find[2]);
                                                                    find2.Append(" ");
                                                                    find2.Append(find[3]);
                                                                    find2.Append(" ");
                                                                    find2.Append(find[4]);

                                                                    //This is what the above StingBuilder operation would look like if you were just
                                                                    //editing the string. I am keeping it here to perhaps test more thoroughly later
                                                                    //on.
                                                                    //string find2 = find[0] + " " + find[1] + " " + find[2] + " " + find[3] + " " + find[4];
                                                                    
                                                                    //Because find2 was set as a StringBuilder, it needs to output it with .ToString().
                                                                    string find3 = find2.ToString();

                                                                    //This performs a hash calculation on the word combination.
                                                                    int hash = find3.GetHashCode();

                                                                    //This looks for this hash calculation in a list of stored calculations to see
                                                                    //if the combination has been entered before. If it hasn't (! means it does not
                                                                    //contain the hash), it then opens up a loop and adds the string version of that
                                                                    //result to results and the hash to find5.
                                                                    if (!find5.Contains(hash))
                                                                    {
                                                                        results.Add(find3);
                                                                        find5.Add(hash);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            });
        }

        private static void process2()
        {
            //This is the recursive version of the above. It appears to be significantly slower.
            int[] next0 = new int[max[0]];
            for (int i = 0; i < next0.Length; i++)
            {
                next0[i] = i;
            }

            int count = 0;
            //for (int a1 = 0; a1 < 2; a1++)
            Parallel.For(0, next0.Length, new ParallelOptions { MaxDegreeOfParallelism = next0.Length }, a1 =>
            {

                //for (int i1 = 0; i1 < dict[next0[a1]].Count; i1++)
                //Parallel.For(0, dict[next0[a1]].Count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 7 }, i1 =>
                Parallel.ForEach(dict[next0[a1]], new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 7 }, i1 =>
                {
                    wordsnums[] word0 = new wordsnums[wordn + 1];
                    word0[count] = i1;
                    uint bin1 = word0[count].bin;
                    addw(count, a1, bin1, word0);
                });
            });
        }

        private static void addw(int count, int start, uint bin, wordsnums[] word0)
        {
            count++;
            int[] next = unuseddigits(bin, max[count]);
            for (int a = start; a < next.Length; a++)
            {
                //for (int i = 0; i < dict[next[a]].Count; i++)
                foreach (wordsnums i in dict[next[a]])
                {
                    word0[count] = i;
                    if ((word0[count].bin & bin) == 0)
                    {
                        uint bin1 = bin | word0[count].bin;
                        if (count < wordn)
                        {
                            addw(count, a, bin1, word0);
                        }
                        else
                        {
                            List<string> find = new List<string>();
                            for (int i0 = 0; i0 < word0.Length; i0++)
                            { find.Add(word0[i0].word); }
                            find.Sort();
                            StringBuilder find2 = new StringBuilder();
                            find2.Append(find[0]);
                            for (int i0 = 1; i0 < find.Count; i0++)
                            {
                                find2.Append(" ");
                                find2.Append(find[i0]);
                            }

                            //string find2 = find[0] + " " + find[1] + " " + find[2] + " " + find[3] + " " + find[4];

                            string find3 = find2.ToString();
                            int hash = find3.GetHashCode();

                            if (!find5.Contains(hash))
                            {
                                results.Add(find3);
                                find5.Add(hash);
                            }
                        }
                    }
                }
            }
        }
        private static uint ConvToUInt(uint r, string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                int val = word[i] - 'a';
                //Only applicable if the wordlist contained non-regular alphabet characters.
                //if (val > -1 && val < 26)
                {
                    r |= (uint)1 << bin[val];
                }
                //else
                //{ return 0; }
            }
            return r;
        }

        private static int[] unuseddigits(uint bin, uint len)
        {
            int[] nextdigit = new int[len];
            int i = 0;
            for (nextdigit[i] = 0; nextdigit[i] < 26; nextdigit[i]++)
            {
                int m = 1 << nextdigit[i];
                if ((bin & m) == 0)
                {
                    if (i == len - 1) { break; }
                    else { i++; nextdigit[i] = nextdigit[i - 1]; }
                }
            }
            return nextdigit;
        }

        private static int lowestdigit(uint bin)
        {
            int nextdigit = 0;
            for (nextdigit = 0; nextdigit < 26; nextdigit++)
            {
                int m = 1 << nextdigit;
                if ((bin & m) != 0)
                    break;
            }
            return nextdigit;
        }


        //I chose a more unusual to me version of counting set bits as this seemed to be one of the two best methods for that
        //and I want to try to understand the logic.
        static int[] num_to_bits = new int[16] { 0, 1, 1, 2, 1, 2, 2,
3, 1, 2, 2, 3, 2, 3, 3, 4 };
        static int countSetBits(uint n)
        {
            uint nibble = 0;
            if (0 == n)
                return num_to_bits[0];

            nibble = n & 0xf;

            return num_to_bits[nibble] + countSetBits(n >> 4);
        }

        private static void test1()
        {
            //This is the main method without comments that I use for testing. I wanted to test something about not remembering the previous iteration
            //for the a# loops and I couldn't immediately think of a way to do it that didn't involve added in 'if's that get looped through millions
            //of times without doing up a new method. But I do feel it is potentially a good idea to have a testing area like this. What I have changed:
            //int a2 = a1, int a3 = a2, int a4 = a3, and int a5 = a4. Doing this is sometimes producing more than 538/831 results and I need to
            //investigate.

            int[] next0 = new int[max[0]];
            for (int i = 0; i < next0.Length; i++)
            {
                next0[i] = i;
            }

            //for (int a1 = 0; a1 < 2; a1++)
            Parallel.For(0, next0.Length, new ParallelOptions { MaxDegreeOfParallelism = next0.Length }, a1 =>
            {
                //for (int i1 = 0; i1 < dict[next0[a1]].Count; i1++)
                //Parallel.For(0, dict[next0[a1]].Count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 7 }, i1 =>
                Parallel.ForEach(dict[next0[a1]], new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 7 }, i1 =>
                {
                    //Storing of the word/number combination.
                    wordsnums word0 = i1;
                    uint bin1 = word0.bin;
                    //int[] next1 = unuseddigits(bin1, max[1]);
                    int[] next1 = new int[max[1]];
                    int i = 0;
                    for (next1[i] = 0; next1[i] < 26; next1[i]++)
                    {
                        int m = 1 << next1[i];
                        if ((bin1 & m) == 0)
                        {
                            if (i == max[1] - 1) { break; }
                            else { i++; next1[i] = next1[i - 1]; }
                        }
                    }

                    for (int a2 = 0; a2 < next1.Length; a2++)
                    {
                        //for (int i2 = 0; i2 < dict[next1[a2]].Count; i2++)
                        foreach (wordsnums i2 in dict[next1[a2]])
                        {
                            wordsnums word1 = i2;
                            if ((word1.bin & bin1) == 0)
                            {
                                uint bin2 = bin1 | word1.bin;
                                //int[] next2 = unuseddigits(bin2, max[2]);
                                int[] next2 = new int[max[2]];
                                i = 0;
                                for (next2[i] = 0; next2[i] < 26; next2[i]++)
                                {
                                    int m = 1 << next2[i];
                                    if ((bin2 & m) == 0)
                                    {
                                        if (i == max[2] - 1) { break; }
                                        else { i++; next2[i] = next2[i - 1]; }
                                    }
                                }

                                for (int a3 = 0; a3 < next2.Length; a3++)
                                {
                                    //for (int i3 = 0; i3 < dict[next2[a3]].Count; i3++)
                                    foreach (wordsnums i3 in dict[next2[a3]])
                                    {
                                        wordsnums word2 = i3;
                                        if ((word2.bin & bin2) == 0)
                                        {
                                            uint bin3 = bin2 | word2.bin;
                                            //int[] next3 = unuseddigits(bin3, max[3]);
                                            int[] next3 = new int[max[3]];
                                            i = 0;
                                            for (next3[i] = 0; next3[i] < 26; next3[i]++)
                                            {
                                                int m = 1 << next3[i];
                                                if ((bin3 & m) == 0)
                                                {
                                                    if (i == max[3] - 1) { break; }
                                                    else { i++; next3[i] = next3[i - 1]; }
                                                }
                                            }

                                            for (int a4 = 0; a4 < next3.Length; a4++)
                                            {
                                                //for (int i4 = 0; i4 < dict[next3[a4]].Count; i4++)
                                                foreach (wordsnums i4 in dict[next3[a4]])
                                                {
                                                    wordsnums word3 = i4;
                                                    if ((word3.bin & bin3) == 0)
                                                    {
                                                        uint bin4 = bin3 | word3.bin;
                                                        //int[] next4 = unuseddigits(bin4, max[4]);
                                                        int[] next4 = new int[max[4]];
                                                        i = 0;
                                                        for (next4[i] = 0; next4[i] < 26; next4[i]++)
                                                        {
                                                            int m = 1 << next4[i];
                                                            if ((bin4 & m) == 0)
                                                            {
                                                                if (i == max[4] - 1) { break; }
                                                                else { i++; next4[i] = next4[i - 1]; }
                                                            }
                                                        }

                                                        for (int a5 = 0; a5 < next4.Length; a5++)
                                                        {
                                                            //for (int i5 = 0; i5 < dict[next4[a5]].Count; i5++)
                                                            foreach (wordsnums i5 in dict[next4[a5]])
                                                            {
                                                                wordsnums word4 = i5;
                                                                if ((word4.bin & bin4) == 0)
                                                                {
                                                                    List<string> find = new List<string>();
                                                                    find.Add(word0.word);
                                                                    find.Add(word1.word);
                                                                    find.Add(word2.word);
                                                                    find.Add(word3.word);
                                                                    find.Add(word4.word);
                                                                    find.Sort();

                                                                    StringBuilder find2 = new StringBuilder();
                                                                    find2.Append(find[0]);
                                                                    find2.Append(" ");
                                                                    find2.Append(find[1]);
                                                                    find2.Append(" ");
                                                                    find2.Append(find[2]);
                                                                    find2.Append(" ");
                                                                    find2.Append(find[3]);
                                                                    find2.Append(" ");
                                                                    find2.Append(find[4]);

                                                                    //string find2 = find[0] + " " + find[1] + " " + find[2] + " " + find[3] + " " + find[4];

                                                                    string find3 = find2.ToString();

                                                                    int hash = find3.GetHashCode();

                                                                    if (!find5.Contains(hash))
                                                                    {
                                                                        results.Add(find3);
                                                                        find5.Add(hash);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            });
        }
    }
}
