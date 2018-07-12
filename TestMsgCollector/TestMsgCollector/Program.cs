using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestMsgCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            var cntr = 0;
            char ans;
            bool first = true;
            List<string> TestCollection = new List<string>();
            Console.WriteLine("===================================================" +
                             "\n=========== HL7 Test Collection Creator ===========" +
                             "\n===================================================");

            Console.Write("::: Complete the prompts to generate a new message\n" +
                          "::: set that contains the messages you want.\n");
            Console.Write("\nWhat is the name of the file you want to search:\n>> ");
            var src = Console.ReadLine();
            if(!File.Exists(src))
            {
                Console.WriteLine("\n***  File does not exist.  Aborting.  ***\n");
                return;
            }

            var msglist = File.ReadAllText(src).Split('\n');

            Console.Write("What is the name of the file you want to create:\n>> ");
            var dst = Console.ReadLine();

            Console.Write("\nWhat should the messages contain?\nFor example, you can enter a message trigger, like A03 or A31\n>> ");

            do
            {
                if (!first)
                {
                    Console.Write("\nWhat should the messages contain?\n>> ");
                }
                first = false;

                var trig = Console.ReadLine();
                Console.Write("How many messages that contain {0} would you like to add:\n>> ", trig);
                var num = Convert.ToInt32(Console.ReadLine());

                foreach (string line in msglist)
                {
                    if (line.Contains(trig))
                    {
                        TestCollection.Add(line);
                        cntr++;
                        if (cntr >= num)
                        {
                            Console.WriteLine("\n\tMessages added successfully.");
                            break;
                        }
                    }
                }

                if (cntr == 0)
                    Console.WriteLine("\n\tNo messages found that contain {0}.", trig);
                else if (cntr < num)
                    Console.WriteLine("\n\tOnly {0} messages found that contain {1}. Messages added.", cntr, trig);

                Console.Write("\nContinue (y/n)? ");
                ans = Console.ReadKey().KeyChar;
                cntr = 0;

            } while(ans == 'Y' || ans == 'y');

            File.WriteAllLines(dst, TestCollection);
            Console.Write("\n\nFile written successfully.  Press any key to exit.\n");
            Console.ReadKey();
        }
    }
}