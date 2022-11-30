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

namespace FiveWordProblem
{
    internal class Program
    {
        //The dictionary List of Lists of Lists of 'wordnums'. This is a List of 'dictionaries' for each of the required character counts for each and
        //letter of the alphabet, so 26 in total (0-25). The full wordlist will be sorted into these dictionaries indexed by their least frequently
        //occuring letter. Each word will be stored as a custom variable I have named 'wordsnums'. This variable contains the string representation of
        //the word and the number representation of the word that we will later perform bitwise operations on.
        static List<List<List<wordsnums>>> dict = new List<List<List<wordsnums>>>();

        //ConcurrentBag is a type of List variable that allows for parallel processing. Results are not sorted in the order they are originally threaded,
        //so will come out pretty much randomly. The below is the initiating of the results and find5 lists. Results is just for the results as you might
        //have guessed. find5 is to store the hash versions of the combinations, which is then searched through to prevent repetitions of differently
        //ordered versions of the same combination. Early in my testing, I perceived it was more efficient to search through a list of hashes than it was
        //a list of strings so I done this. I didn't apply the same thorough testing I now use so should revisit. I believe it is faster initializing
        //these lists globally.
        static ConcurrentBag<string> results = new ConcurrentBag<string>();
        static ConcurrentBag<int> find5 = new ConcurrentBag<int>();

        static string anagram = "abcdefghijklmnopqrstuvwxyz";
        static uint anabin = 0;

        //Sets whether to remove anagrams or not. I would not use this is you wanted all the results, but Matt Parker and all the speediest methods I
        //believe used this as they are competing for speed. The output should be 538 results with anagrams removed and 831 with anagrams preserved. I
        //have taken the words_alpha_five list from another project and it is generating this number for both. In fact, I will be adding some words from
        //the other solutions Matt Parker said others had, which feature words not in this English dictionary.
        static bool anagrem = true;

        static int range1 = 5;
        static int range2 = 5;
        static int rangehigh = 5;
        static int rangelow = 5;
        static int wordsnum = 5;

        //Sets which operation to process: 1 = process1() without the method calls, 0 = process2() with the method calls. Anything else for test().
        static uint process = 1;

        //Whether to sort the results alphabetically. By default off as this adds processing...
        static bool sort = false;

        //Custom word and letter numbers. This will only work with method2() and dictionary[4], so the fourth digit in the commandline needs to be set
        //to 0 and the fifth digit in the commandline should be 4.
        static List<int> wordn = new List<int>();

        //I explain this more later.
        static List<uint> max = new List<uint>();

        //Initializes an array that will contain the letters of the alphabet and their order.
        static int[] bin = new int[26];

        //Just a test option for the command line.
        static bool test = false;

        //A counter for the iterations. The counter needs to be uncommented.
        static uint counter = 0;

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
            uint rep = 1;
            if (args.Length > 1)
            {
                rep = Convert.ToUInt32(args[1]);
            }

            List<double> elapsed = new List<double>();
            List<double> elapsed0 = new List<double>();
            List<double> elapsed1 = new List<double>();
            List<double> elapsed2 = new List<double>();
            List<double> elapsed3 = new List<double>();

            for (int i = 0; i < rep; i++)
            {
                Thread.Sleep(1);
                //Creating and starting the stopwatch.
                Stopwatch sw = new Stopwatch();
                Stopwatch sw2 = new Stopwatch();
                sw.Start();

                sw2.Start();

                wordn.Clear();

                //This is handling the various command line arguments. My first time doing this.
                uint dictionary = 0;
                if (args.Length > 0)
                {
                    if (args[0] != "") { anagram = args[0]; }
                    if (args.Length > 2)
                    {
                        if (args[2] == "0") { anagrem = false; }
                        if (args.Length > 3)
                        {
                            if (args[3] != "0") { sort = true; }
                            if (args.Length > 4)
                            {
                                process = Convert.ToUInt32(args[4]);
                                if (args.Length > 5)
                                {
                                    dictionary = Convert.ToUInt32(args[5]);
                                    if (args.Length > 6)
                                    {
                                        range1 = Convert.ToInt32(args[6]);
                                        if (args.Length > 7)
                                        {
                                            range2 = Convert.ToInt32(args[7]);
                                            if (args.Length > 8)
                                            {
                                                wordsnum = Convert.ToInt32(args[8]);
                                                if (args.Length > 9)
                                                {
                                                    for (int i2 = 9; i2 < args.Length; i2++)
                                                    { wordn.Add(Convert.ToInt32(args[i2])); }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //Initiating that thing that probably does nothing for nothings sake.
                //TimeBeginPeriod(1);

                //Sets the letter order. bin[0] is 'a', bin[25] is 'z'. So if I set bin[0] to 25, I am saying 'a' should be treated as the last letter
                //of the alphabet, and setting bin[25] to 0, it is saying 'z' should be set as the first letter of the alphabet. I have switched it from
                //an array to a list so that letters may be removed if they're not part of the anagram.
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

                anagram = anagram.Replace(" ", "").ToLower();

                anabin = ConvToUInt(0, anagram);

                int bits = countSetBits(anabin);

                int[] remove = unuseddigits(anabin, 26);

                for (int i2 = 0; i2 < remove.Count(); i2++)
                {
                    if (remove[i2] == -1) { break; }
                    bin[Array.IndexOf(bin, remove[i2])] = -1;
                }

                anabin = 67108863 ^ anabin;


                //This determines how many characters it needs to push the scan across. If it is 5*5, they use 25 letters so you need to push it once
                //and you add 1 to that as it is inclusive in the way it is implemented. I formerly made this custom, but I needed to free up space for
                //a better custom word length thing and having this custom was more of a testing thing and I am skeptical about my beliefs that more
                //combos might be found by customizing it.
                max.Clear();
                if (process != 0)
                {
                    for (int i2 = 0; i2 < 5; i2++)
                    {
                        wordn.Add(5);
                    }

                    for (int i2 = 0; i2 < 5; i2++)
                    { max.Add(2); }
                }
                else
                {
                    int sum = 0;
                    if (range1 != range2)
                    {
                        rangelow = 0;
                        rangehigh = 0;
                        if (range2 > range1)
                        {
                            rangelow = range1;
                            rangehigh = range2;
                        }
                        else
                        {
                            rangelow = range2;
                            rangehigh = range1;
                        }
                        for (int i2 = rangelow; i2 < rangehigh + 1; i2++)
                        {
                            wordn.Add(i2);
                        }

                    }
                    else
                    {
                        rangelow = range1;
                        rangehigh = range2;
                        wordn.Add(range1);
                    }

                    for (int i2 = 0; i2 < wordn.Count; i2++)
                    {
                        max.Add((uint)(anagram.Length % wordn[i2] + 1));
                    }
                }

                elapsed0.Add(sw2.Elapsed.TotalSeconds);
                sw2.Restart();

                //I believe it is faster to generate these lists as global variables above, then clear them each iteration.
                dict.Clear();

                //Remove anagrams from the words if this option is chosen. This option was used by Matt Parker to save time. It is no longer necessary,
                //but this is what people are doing for the competitive nature of it. If you were serious about the results, you would turn it off.

                //I have since put the file loading stuff into separate operations depending on whether the anagrams are chosen. My reasoning for this is
                //the only way I can think of to get Parallel.For to work with loading requires a ConcurrentBag variable. This list cannot be iterated
                //so the anagrams can't be removed from the list in an seemingly efficient way. On the flipside, if I converted the ConcurrentBag to a
                //list it would probably counter any potential gain from loading the file in parallel anyway. So for now, I will just try to integrate it
                //for the anagram non-removal mode. However, a problem just occurred to me: how to initiate the variable as either a ConcurrentBag or List
                //without at some stage converting them? Maybe it will require conversion between bag and list anyway.

                string filename = System.IO.Directory.GetCurrentDirectory() + @"\Files\dictionary[" + dictionary.ToString() + "].txt";

                if (File.Exists(filename))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(System.IO.Directory.GetCurrentDirectory() + @"\Files\dictionary[" + dictionary.ToString() + "].txt");
                    for (int i2 = 0; i2 < wordn.Max(); i2++)
                    {
                        dict.Add(new List<List<wordsnums>>());
                        for (int i3 = 0; i3 < 26; i3++)
                        {
                            dict[i2].Add(new List<wordsnums>());
                        }
                    }


                    //This checks the anagram, the letters of the alphabet if unchanged, to see if duplicate letters are being sought and if not then
                    //exclude them from the dictionary.
                    Boolean bits2 = false;
                    if (bits != anagram.Length) { bits2 = true; }

                    //Skips over the first commented line in the dictionary. This is for the while loop, it is pointless for Parallel.ForEach loop
                    //below. With Parallel.For I believe the word.Length = lettern thing will eliminate comments, at least if they are longer than the
                    //target word length. If they aren't they may trigger a crash.
                    sr.ReadLine();

                    //This is the lock for Parallel.For. Since running with Parallel.For and lock seemed to sacrifice a millisecond or two, I decided
                    //to comment out all that code but retain it for further testing.
                    //object _object = new object();

                    while (!sr.EndOfStream)
                    //Parallel.ForEach(File.ReadLines(filename), line =>
                    {
                        //The word variable for the while loop.
                        string word = sr.ReadLine();
                        //The word variable for Parallel.ForEach.
                        //string word = line;

                        //This check is needed in case you insert a larger dictionary which I wanted to allow the user to do.
                        if (wordn.Contains(word.Length))
                        {
                            //Since there are probably no words with capital letters in the file, this is unneeded.
                            //string word2 = word.ToLower();

                            uint bin = ConvToUInt(0, word);

                            //This check is needed for all 5 letter words to check whether they have duplicate letters. If they have less than 5 bits,
                            //it means at least 1 letter is a duplicate.
                            if (bin > 0 && ((bits2 || word.Length == countSetBits(bin)) && wordn.Contains(word.Length)))
                            {
                                int digs = lowestdigit(bin);
                                //if (!dict[digs].Any(c => c.bin == bin))
                                {
                                    //Lock for Parallel.For otherwise it will crash. This nullifies any point in using Parallel.For here. The problem
                                    //is you can't perform a parallel loop on a List variable, it needs to be one of the Concurrent variables. So
                                    //change it to a ConcurrentBag? The problem is you can't then remove anagrams, because although you can sort a
                                    //ConcurrentBag, you can't .RemoveAt like you can a list. It might work with one of the other Concurrent variables.
                                    //So why not just convert ConcurrentBag to List with .ToList()? Tested it, and it's too slow; it is far slower
                                    //than what you gain by using Parallel.ForEach. Same with scanning the List for anagrams to remove them before they
                                    //are inserted. It occurred to me to use a List variable for anagram removal and a ConcurrentBag for anagram non-
                                    //removal. The problem then is you need to initialize two list variables of different types, and the only way I
                                    //can think of to read those two different types of variables is to duplicate method1(), which makes this code even
                                    //more heinously long. Could put a whole lot through methods if I weren't considering speed, but C# seems a little
                                    //slow in handling methods. ConcurrentDictionary's might work as I am reading something now about that, but I never
                                    //figured out dictionary variables.

                                    //Lock is not needed if you're using a Concurrent list.
                                    //lock (_object)
                                    {
                                        //Just change .Add to .Enqueue if using a ConcurrentQueue.
                                        dict[word.Length - 1][digs].Add(new wordsnums { word = word, bin = bin });
                                    }
                                }
                            }
                        }
                    }//);
                }

                for (int i2 = 0; i2 < wordn.Count(); i2++)
                {
                    wordn[i2]--;
                }

                if (anagrem)
                {
                    for (int i2 = 0; i2 < dict.Count; i2++)
                    {
                        for (int i3 = 0; i3 < dict[i2].Count; i3++)
                        {
                            if (dict[i2][i3].Count > 0)
                            {
                                //Sorts the dictionary by the uint representations of the word. Every anagram of the same length should have the same number.
                                //Anagrams with double letters could have the same number as words of shorter lengths, but this is not important here as this
                                //problem only concerns 5 letter words.
                                dict[i2][i3].Sort(delegate (wordsnums c1, wordsnums c2) { return c1.bin.CompareTo(c2.bin); });
                                //How you would sort it if it were a Concurrent list.
                                //dict[i2].OrderBy(c => c.bin);

                                //Removes every word from the list that matches the number below it.
                                //It is not possible to identify the preceding value in the list with ConcurrentBag or ConcurrentQueue.
                                for (int i4 = dict[i2][i3].Count - 1; i4 > 0; i4--)
                                { if (dict[i2][i3][i4].bin == dict[i2][i3][i4 - 1].bin) { dict[i2][i3].RemoveAt(i4); } }
                            }
                        }
                    }
                }

                elapsed1.Add(sw2.Elapsed.TotalSeconds);
                sw2.Restart();

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

                sw2.Restart();

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

                elapsed2.Add(sw2.Elapsed.TotalSeconds);
                sw2.Restart();

                //if (process != 0)
                {
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
                }

                sw2.Stop();
                elapsed3.Add(sw2.Elapsed.TotalSeconds);

                //Stop the timer.
                sw.Stop();
                elapsed.Add(sw.Elapsed.TotalSeconds);

                //Write the output to console. Optional.
                Console.Write("\r\n(Timings do not include this data.)\r\n");
                Console.Write("Count: " + find5.Count().ToString() + "\r\n");

                if (counter > 0) { Console.Write("Iterations: " + counter.ToString() + "\r\n"); counter = 0; }

                Console.Write("Current: " + elapsed[elapsed.Count() - 1].ToString() + "s\r\n");

                if (rep > 1)
                {
                    Console.Write("Runs: " + elapsed.Count().ToString() + "\r\n");
                    Console.Write("Average: " + elapsed.Average().ToString("0.#######") + "s\r\n");
                }

                Console.Write("- presets: " + elapsed0.Average().ToString("0.#######") + "s\r\n");
                Console.Write("- dictionary: " + elapsed1.Average().ToString("0.#######") + "s\r\n");
                Console.Write("- processing: " + elapsed2.Average().ToString("0.#######") + "s\r\n");
                Console.Write("- output: " + elapsed3.Average().ToString("0.#######") + "s\r\n");

                if (rep > 1)
                {
                    Console.Write("Minimum: " + elapsed.Min().ToString() + "s\r\n");
                    Console.Write("Maximum: " + elapsed.Max().ToString() + "s\r\n\r\n");
                }

                //That thing that does nothing is now turned off.
                //TimeEndPeriod(1);
            }
        }

        private static void process1()
        {
            //This sets the first two letters for the first iteration. 0 would be 'a' and 1 would be 'b' in an A-Z alphabet. But because this is an
            //'alphabet' sorted by lowest frequency, these first two represent the least frequent letters.
            int[] next0 = unuseddigits(anabin, max[0]);

            //Parallel.For is for multithreaded processing. For whatever reason, doing two Parallel.For loops within one another seemed to tweak the
            //performance. I am not entirely clear on what the nextDegreeOfParallelism is, but I don't believe it is the same as how many threads
            //your processor has. Since there is just 2 iterations of the first loop, I have set that to 2. I seemed to get optimal performance out
            //of setting the other to 7 times the number of processors. If you wish to use Parallel.For, you need to be mindful at least that
            //variables need to be thread safe. Especially if you're going to alter variables, you may need to use ConcurrentBag/ConcurrentQueue/
            //ConcurrentDictionary variables. This is my first time actually getting Parallel.For to work at significantly improving speeds! The non-
            //parallel for loops are commented out and retained if you wish to experiment with them.

            //foreach (int a1 in next0)
            //Parallel.ForEach(next0, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, a1 =>
            Parallel.For(0, next0.Length, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, a1 =>
            {
                //foreach (wordsnums i1 in dict[wordn[0]][a1])
                Parallel.ForEach(dict[wordn[0]][next0[a1]], new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 7 }, i1 =>
                {
                    //Storing of the word/number combination.
                    wordsnums word0 = i1;
                    uint bin1 = anabin | word0.bin;
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
                        //So it starts at 1, then bit shifts to 10, then bit shifts to 100, then bit shifts to 1000, and so on. Like multiplying by
                        //10, except every 1 represents a value of 2 not 10. It will do this for 26 letters of the alphabet based on the order of
                        //least frequent to most frequent. Assuming a standard alphabet, and given next1[i] is the numeric identifier for the character
                        //in the alphabet, it is shifting left firstly with 'a' (0), 'b' (1), 'c' (2), etc. just adjusted to the the frequency
                        //'alphabet'.
                        int m = 1 << next1[i];
                        //The above, 'm' is the letter in binary. The 'bin1' value is the binary value of the combinations of words to this point. The
                        //& symbol means that it is performing a 'bit and' operation on the binary representation of the word. If the binary of two
                        //1's align, it results in a value of 1: it means that letter position is occupied (used). If the bin1 value is 0, then it
                        //results in 0 (for some reason) and then enters that number as the first missing letter in the array. Then it looks for
                        //another until i = 2. I recommend (something I haven't really done) using an online bitwise calculator to get a feel for the
                        //way these things work. I wish I had thought of that, but I kind of just brute forced it and ocassionally debugged it until
                        //I kind of figured it out for now.
                        if ((bin1 & m) == 0)
                        {
                            //Heck, this confused me for a bit. So it needs clarifying. The setting of the next1[i] value is done at the loop level
                            //above. It is continually set as it goes through the alphabet, 0-25. If a missing letter is found, a 1 is added to the
                            //'i' variable. This switches it from the first value (now permanently set) to the second value, to be set in the 'for'
                            //iteration. This second value (next[1]) gets set to the same value as the first one (next[0]) with next[i] = next[max1 - 1]
                            //to resume from where it was detected.
                            if (i == max[1] - 1) { break; }
                            else { i++; next1[i] = next1[i - 1]; }
                        }
                    }

                    //Goes for the Parellel.For loops above but the comments are becoming too heavy. Notice how each 'for' loop within a 'for' loop
                    //has an a2 and i2. The 'a#' loops are iterations through the next1 arrays, so each of the loops have a nextimum of 2 iterations
                    //if the max# values haven't been changed. But it carries through the previous iteration count, eg. int a2 = a1. So if a1 is
                    //shifted across 1, then so too is each subsequent operation and therefore each iteration is reduced to 1. I'm not sure, but this
                    //may potentially not grab all results?
                    for (int a2 = a1; a2 < next1.Length; a2++)
                    {
                        //This is the iteration through each dictionary for each word indexed by the least common letter. It is looking through the
                        //dictionaries that match the next available letter.
                        //for (int i2 = 0; i2 < dict[next1[a2]].Count; i2++)
                        foreach (wordsnums i2 in dict[wordn[1]][next1[a2]])
                        //Parallel.ForEach(dict[wordn[1]][next1[a2]], new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 7 }, i2 =>
                        {
                            wordsnums word1 = i2;
                            //This 'bitwise and' operation is checking to see if any letters from the new word that overlap the former combination.
                            if ((word1.bin & bin1) == 0)
                            {
                                //The below is performing a 'bitwise or' operation using the |. This is adding the two words onto one anothers bit
                                //representation. It will add a 1 next to each letter that was not represented in the previous word combination to
                                //this point. 1 + 1 will be 1 and 0 + 1 will also be 1. Since it is binary, there is no 2. There is also no way of
                                //accounting for duplicate characters, which means for anagrams (does not pertain to here) you need to do something
                                //else but it helps immensely as a check before looping through the letters of a word for instance.
                                uint bin2 = bin1 | word1.bin;
                                //Again, the unusueddigits() method call has been replaced by the code here to set the next2, next3, next4, etc.
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
                                    foreach (wordsnums i3 in dict[wordn[2]][next2[a3]])
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
                                                foreach (wordsnums i4 in dict[wordn[3]][next3[a4]])
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
                                                            foreach (wordsnums i5 in dict[wordn[4]][next4[a5]])
                                                            {
                                                                wordsnums word4 = i5;
                                                                if ((word4.bin & bin4) == 0)
                                                                {
                                                                    int hash = word0.word.GetHashCode() + word1.word.GetHashCode() + word2.word.GetHashCode() + word3.word.GetHashCode() + word4.word.GetHashCode();

                                                                    //This looks for this hash calculation in a list of stored calculations to see
                                                                    //if the combination has been entered before. If it hasn't (! means it does not
                                                                    //contain the hash), it then opens up a loop and adds the string version of that
                                                                    //result to results and the hash to find5.
                                                                    if (!find5.Contains(hash))
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

                                                                        results.Add(find2.ToString());
                                                                        //results.OrderBy(c => c);
                                                                        find5.Add(hash);
                                                                        find5.OrderBy(c => c);
                                                                        //Console.WriteLine(find3);
                                                                    }
                                                                }
                                                                //counter++;
                                                            }
                                                        }
                                                    }
                                                    //counter++;
                                                }
                                            }
                                        }
                                        //counter++;
                                    }
                                }
                            }
                            //counter++;
                        }//);
                    }
                    //counter++;
                });
            });
        }

        private static void process2()
        {
            //This (process2() and addw() are the recursive version of process1(). It appears to be significantly slower.
            //NOTE: It shouldn't be this slow! Something is awry!


            int count = 0;
            //for (int a1 = 0; a1 < 2; a1++)


            //int x = 0;
            for (int x = wordn.Count - 1; x > -1; x--)
            {
                int[] next0 = unuseddigits(anabin, max[x]);
                //for (int a1 = 0; a1 < next0.Length; a1++)
                Parallel.For(0, next0.Length, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, a1 =>
                {
                    if (next0[a1] > -1)
                    {
                        //foreach (wordsnums i1 in dict[wordn[x]][next0[a1]])
                        Parallel.ForEach(dict[wordn[x]][next0[a1]], new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 7 }, i1 =>
                        {
                            wordsnums[] word0 = new wordsnums[wordsnum];
                            word0[count] = i1;
                            uint bin1 = anabin | word0[count].bin;
                            if (wordsnum != 1)
                            {
                                addw(count, a1, bin1, word0);
                            }
                            else
                            { finalize(word0); }
                            //counter++;
                        });
                    }
                });
            }
        }

        private static void addw(int count, int start, uint bin, wordsnums[] word0)
        {
            count++;

            //int x = 0;
            for (int x = wordn.Count - 1; x > -1; x--)
            {
                int[] next = unuseddigits(bin, max[x]);
                for (int a = start; a < next.Length; a++)
                {
                    if (next[a] > -1)
                    {
                        //for (int i = 0; i < dict[next[a]].Count; i++)
                        foreach (wordsnums i in dict[wordn[x]][next[a]])
                        {
                            word0[count] = i;
                            if ((word0[count].bin & bin) == 0)
                            {
                                uint bin1 = bin | word0[count].bin;
                                if (count < wordsnum - 1)
                                {
                                    addw(count, a, bin1, word0);
                                }
                                else
                                {
                                    finalize(word0);
                                }
                            }
                            //counter++;
                        }
                    }
                }
            }
        }

        private static void finalize(wordsnums[] word0)
        {
            int hash = word0[0].word.GetHashCode();
            for (int i = 1; i < word0.Length; i++)
            { hash += word0[i].word.GetHashCode(); }

            if (!find5.Contains(hash))
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

                string find3 = find2.ToString();

                int i2 = 0;

                //everything up to now only checks whether the letters are in the anagram. It does not check whether the count
                //of the letters matches the letters in the anagram.
                List<char> array = anagram.ToList();
                for (i2 = 0; i2 < find3.Length; i2++)
                {
                    if (find3[i2] != ' ')
                    {
                        int index = array.IndexOf(find3[i2]);
                        if (index == -1) { i2 = -1; break; }
                        array.RemoveAt(index);
                    }
                }

                if (i2 > -1)
                {
                    results.Add(find3);
                    find5.Add(hash);
                    find5.OrderBy(c => c);
                }
                //Console.WriteLine(find3);
            }
        }

        private static uint ConvToUInt(uint r, string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                int val = word[i] - 'a';
                //Only applicable if the wordlist contained non-regular alphabet characters.
                if (bin[val] == -1) { return 0; }
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
            for (int i2 = 0; i2 < len; i2++)
            { nextdigit[i2] = -1; };
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
            if (nextdigit[i] == 26) { nextdigit[i] = -1; }
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

        }
    }
}
