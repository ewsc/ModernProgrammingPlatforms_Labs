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

            var foo = faker.Create<Foo>();

            Console.WriteLine($"Foo: Id={foo.Id}, Name={foo.Name}, Bar.Value={foo.Bar!.Value}, Bar.Timestamp={foo.Bar.Timestamp}");

            Console.WriteLine("Bar.Tags:");
            foreach (var tag in foo.Bar.Tags)
            {
                Console.WriteLine(tag);
            }
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