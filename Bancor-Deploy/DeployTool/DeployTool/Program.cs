using System;
using System.Collections.Generic;

namespace DeployTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.Init("config.json");

            InitMethod();
            ShowMenu();

            StatrLoop();

            while (true)
            {
                System.Threading.Thread.Sleep(100);
            }
        }

        private static void StatrLoop()
        {
            while (true)
            {
                var line = Console.ReadLine().ToLower();
                if (line == "?" || line == "？" || line == "ls")
                {
                    ShowMenu();
                }
                else if (line == "")
                {
                    continue;
                }
                else if (allOperation.ContainsKey(line))
                {
                    var example = allOperation[line];
                    try
                    {
                        Console.WriteLine("[begin]" + example.Name);
                        example.Start();

                        Console.WriteLine("[end]" + example.Name);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("unknown line.");
                }
            }

        }

        private static void ShowMenu()
        {
            Console.WriteLine("===========================================================");
            foreach (var item in allOperation)
            {
                Console.WriteLine("type '" + item.Key + "' to Run: " + item.Value.Name);
                Console.WriteLine("===========================================================");
            }
            Console.WriteLine("type '?' to Get this list.");
            Console.WriteLine("===========================================================");
        }

        private static void InitMethod()
        {
            RegOperatione(new SetWhiteList());
            RegOperatione(new SetConnectWeight());
            RegOperatione(new SetMaxConnectWeight());
            RegOperatione(new SetConnectBalanceIn());
            RegOperatione(new SetSmartTokenSupplyIn());
            RegOperatione(new GetConnectBalanceBack());
            RegOperatione(new GetSmartTokenSupplyBack());
            RegOperatione(new Purchase());
            RegOperatione(new Sale());
            
            RegOperatione(new GetWhiteList());
            RegOperatione(new GetAssetInfo());
            RegOperatione(new CalculatePurchaseReturn());
        }

        public static Dictionary<string, IOperation> allOperation = new System.Collections.Generic.Dictionary<string, IOperation>();
        static void RegOperatione(IOperation operation)
        {
            allOperation[operation.ID.ToLower()] = operation;
        }
    }
}
