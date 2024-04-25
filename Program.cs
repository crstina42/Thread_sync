using System;
using System.Threading;
using Thread_sync;

namespace Thread_sync
{
    internal class Program
    {


        private static WinApiClass.CRITICAL_SECTION section;
        private static double a;
        private static double b;
        private static double c;
        private static double delta;
        private static double x1;
        private static double x2;

        private static IntPtr abcReadyEvent;
        private static IntPtr deltaReadyEvent;
        private static IntPtr x1ReadyEvent;

        static uint CalculateDelta(IntPtr val)
        {
            

            // Așteptăm ca coeficienții a, b, c să fie gata
            Console.WriteLine("Introduceti valorile pentru a, b si c:");
            a = double.Parse(Console.ReadLine());
            b = double.Parse(Console.ReadLine());
            c = double.Parse(Console.ReadLine());


            WinApiClass.WaitForSingleObject(abcReadyEvent, WinApiClass.INFINITE);

            delta = b * b - 4 * a * c;


            WinApiClass.SetEvent(deltaReadyEvent);

            return 0;
        }
        static uint CalculateX1(IntPtr val)
        {
            WinApiClass.WaitForSingleObject(deltaReadyEvent, WinApiClass.INFINITE);

            if (delta >= 0)
            {
                x1 = (-b + Math.Sqrt(delta)) / (2 * a);
            }
            else
            {
                Console.WriteLine("Ecuatia nu are solutii reale.");
                x1 = double.NaN;
            }

            WinApiClass.SetEvent(x1ReadyEvent);
            return 0;
        }
        static uint CalculateX2(IntPtr val)
        {
            WinApiClass.WaitForSingleObject(deltaReadyEvent, WinApiClass.INFINITE);
            if (delta >= 0)
            {
                x2 = (-b - Math.Sqrt(delta)) / (2 * a);
            }
            else
            {
                Console.WriteLine("Ecuatia nu are solutii reale.");
                x1 = double.NaN;
            }
         
            return 0;
        }

        static void Main(string[] args)
        {

            WinApiClass.InitializeCriticalSection(out section);
            uint threadId;

            // Initialize events
            abcReadyEvent = WinApiClass.CreateEvent(IntPtr.Zero, true, false, "a b c Ready");
            deltaReadyEvent = WinApiClass.CreateEvent(IntPtr.Zero, true, false, "delta Ready");
            x1ReadyEvent = WinApiClass.CreateEvent(IntPtr.Zero, true, false, "x1 Ready");

            // Create threads for calculating delta, x1, and x2
            IntPtr deltaThread = (IntPtr)WinApiClass.CreateThread(IntPtr.Zero, 0, CalculateDelta, IntPtr.Zero, 0, out threadId);
            IntPtr x1Thread = (IntPtr)WinApiClass.CreateThread(IntPtr.Zero, 0, CalculateX1, IntPtr.Zero, 0, out threadId);
            IntPtr x2Thread = (IntPtr)WinApiClass.CreateThread(IntPtr.Zero, 0, CalculateX2, IntPtr.Zero, 0, out threadId);

            // Set ABC values ready event
            WinApiClass.SetEvent(abcReadyEvent);

            // Wait for threads to finish
            WinApiClass.WaitForSingleObject(deltaThread, WinApiClass.INFINITE);
            WinApiClass.WaitForSingleObject(x1Thread, WinApiClass.INFINITE);
            WinApiClass.WaitForSingleObject(x2Thread, WinApiClass.INFINITE);

            // Wait for threads to finish
            WinApiClass.WaitForSingleObject(deltaThread, WinApiClass.INFINITE);
            WinApiClass.WaitForSingleObject(x1Thread, WinApiClass.INFINITE);
            WinApiClass.WaitForSingleObject(x2Thread, WinApiClass.INFINITE);

            // Display solutions
            Console.WriteLine("Solutiile ecuatiei sunt:");
            Console.WriteLine($"x1 = {x1}");
            Console.WriteLine($"x2 = {x2}");

            WinApiClass.DeleteCriticalSection(ref section);

            // Clean up critical section and events


            Thread.Sleep(3000);
        }

    }

}
