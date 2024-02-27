namespace LabUno
{
    public class Program
    {
        static void Main()
        {
            ITracer tracer = new MethodTracer();
            tracer.StartTrace();
            SumMethod();
            tracer.StopTrace();

            Sorter sortir = new Sorter();
            sortir.TraceAndSort();
        }

        static void SumMethod()
        {
            var sum = 0;
            for (int i = 1; i <= 1000000; i++)
            {
                sum += i;
            }
        }

        
    }

    public class Sorter
    {
        public void TraceAndSort()
        {
            ITracer tracer = new MethodTracer();
            tracer.StartTrace();
            SortMethod();
            tracer.StopTrace();    
        }
        
        static void SortMethod()
        {
            int[] numbers = { 4, 2, 7, 1, 9, 5, 3, 8, 6 };
            Array.Sort(numbers);
        }
    }
}