namespace LabDos
{
    public class Foo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Bar? Bar { get; set; }
    }

    public class Bar
    {
        public decimal Value { get; set; }
        public DateTime Timestamp { get; set; }
        
        public Uri Address { get; set; }
        public List<string> Tags { get; set; }
        
    public Bar()
        {
            Tags = new List<string>();
        }
    }

    static class Program
    {
        static void Main()
        {
            var faker = new Faker();

            faker.RegisterGenerator(GenerateFoo);
            faker.RegisterGenerator(GenerateBar);
            faker.RegisterGenerator(GenerateList);

            
            var foo = faker.Create<Foo>();
            var rInt = faker.Create<int>();
            var list = faker.CreateList<string>(5);
            

            Console.WriteLine($"Foo: Id={foo.Id}, Name={foo.Name}, Bar.Value={foo.Bar!.Value}, Bar.Timestamp={foo.Bar.Timestamp}");

            Console.WriteLine("\nFaked Int: " + rInt);
            Console.WriteLine("\nList items:");
            foreach (var listik in list)
            {
                Console.WriteLine(listik);
            }
        }

        private static List<string> GenerateList(Faker faker)
        {
            return faker.CreateList<string>(5);
        }

        private static Foo GenerateFoo(Faker faker)
        {
            return new Foo
            {
                Id = faker.NextRandomInt(),
                Name = faker.NextRandomString(10),
                Bar = faker.Create<Bar>()
            };
        }

        private static Bar GenerateBar(Faker faker)
        {
            return new Bar
            {
                Value = (decimal)faker.NextRandomDouble(),
                Timestamp = faker.NextRandomDateTime(),
                Tags = faker.CreateList<string>(5)
            };
        }
    }
}