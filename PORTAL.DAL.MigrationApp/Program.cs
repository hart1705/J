using System;

namespace PORTAL.DAL.MigrationApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //FetchChildren(0, 155);
            getUpperNumberConnected(155);
            Console.ReadKey();
        }

        private static void FetchChildren(int parent, int lastChild)
        {
            bool continueGenerate = true;
            int depth = 0;
            //if (parent == lastChild) return;
            while (continueGenerate)
            {
                parent = PrintChild(depth, parent, lastChild, out continueGenerate);
                depth += 1;
                Console.WriteLine();
            }
        }

        private static int PrintChild(int depth, int parentNo, int lastChildNo, out bool stop)
        {
            stop = false;
            int lastChild = 0;
            int newParent = (parentNo * 5) + 1;
            int cDepth = ComputeDepth(depth);
            for (int j = 0; j < cDepth; j++)
            {
                
                lastChild = (parentNo * 5) + (j + 1);
                if (lastChild > lastChildNo) break;
                Console.Write($" {lastChild} ");
            }
            stop = !(lastChild >= lastChildNo);
            return newParent;
        }

        private static int ComputeDepth(int depth)
        {
            int count = 5;
            for (int i = 0; i < depth; i++)
            {
                count = count * 5;
            }
            return count;
        }

        public static int getUpperNumberConnected(double lastNumber)
        {
            double connectNumber = (lastNumber / 5) - 0.2;
            string numWithdecimal = connectNumber.ToString();
            string[] number = numWithdecimal.Split(".");
            int num = int.Parse( number[0]);

            Console.WriteLine(connectNumber);
            Console.Write(num);

            return 0;
        }
    }
}
