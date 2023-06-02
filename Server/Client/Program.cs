using Client.Commands;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            bool first_start = true;
            List<Audit> errors = new List<Audit>();
            do
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(">>> ");
                Console.ForegroundColor = ConsoleColor.White;
                var command = Console.ReadLine();
                command = command.Trim();
                command = Regex.Replace(command, @"\s+", " ");
                var command_splited = command.Split(' ');
                if (command_splited[0].ToLower() == "send")
                {
                    try
                    {
                        string isSentMessage = new Command().SendCommand(out errors);
                        foreach (var err in errors)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(err);
                            Console.ForegroundColor= ConsoleColor.White;
                        }
                        if (isSentMessage == "Send failed")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("CLIENT: ");
                            Console.WriteLine(isSentMessage);
                            Console.ForegroundColor = ConsoleColor.White;
                            continue;
                        }
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("CLIENT: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(isSentMessage);
                        
                    }
                    catch (FaultException<NoFileException> e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR: "+e.Detail.Message);
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }
                    first_start = false;
                }
                else if (command_splited[0].ToLower() == "get")
                {
                    if (first_start)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("! Please first send files to server using command Send !");
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }
                    if (command_splited.Count() > 4)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("! Maximum number of params is 3 (min,max,stand) !");
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }
                    if (command_splited.Count() < 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("! Minimum number of params is 1 (min,max,stand) !");
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }
                    bool valid_params = true;
                    bool min = false;
                    bool max = false;
                    bool stand = false;
                    foreach(var param in command_splited)
                    {
                        switch (param.ToLower())
                        {
                            case "min":
                                min = true;
                                break;
                            case "max": 
                                max = true; 
                                break;
                            case "stand":
                                stand = true;
                                break;
                            case "get":
                                break;
                            default:
                                valid_params = false;
                                break;
                        }
                    }
                    if (valid_params)
                    {
                        new Command().GetCommand(min, max, stand);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("! Wrong parameters !");
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }
                    

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("! Not an existing command !");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }
                
            } while (true);

        }
    }
}
