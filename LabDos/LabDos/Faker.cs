using System.Reflection;

namespace LabDos
{
    public class Faker
    {
        private readonly Dictionary<Type, Func<Faker, object>> _generators;
        private readonly HashSet<Type> _processedTypes;

        public Faker()
        {
            _generators = new Dictionary<Type, Func<Faker, object>>();
            _processedTypes = new HashSet<Type>();

            RegisterGenerator(faker => faker.NextRandomInt());
            RegisterGenerator(faker => faker.NextRandomLong());
            RegisterGenerator(faker => faker.NextRandomDouble());
            RegisterGenerator(faker => faker.NextRandomFloat());
            RegisterGenerator<string>(faker => faker.NextRandomString(10));
            RegisterGenerator(faker => faker.NextRandomDateTime());
            RegisterGenerator<Uri>(faker => faker.NextRandomUri());
        }

        public void RegisterGenerator<T>(Func<Faker, T> generator)
        {
            _generators[typeof(T)] = faker => generator(faker) ?? throw new InvalidOperationException();
        }

        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }

        private object Create(Type type)
        {
            if (IsSimpleType(type))
            {
                return GenerateSimpleValue(type);
            }

            if (_processedTypes.Contains(type))
            {
                return null!;
            }

            _processedTypes.Add(type);

            if (_generators.ContainsKey(type))
            {
                var generator = _generators[type];
                return generator(this);
            }

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length > 0)
            {
                var constructor = constructors.First();
                var parameters = constructor.GetParameters();
                var arguments = parameters.Select(p => Create(p.ParameterType)).ToArray();
                return Activator.CreateInstance(type, arguments)!;
            }

            var instance = Activator.CreateInstance(type);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    property.SetValue(instance, Create(property.PropertyType));
                }
            }

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                field.SetValue(instance, Create(field.FieldType));
            }

            _processedTypes.Remove(type);

            return instance!;
        }

        private object GenerateSimpleValue(Type type)
        {
            if (type == typeof(int))
            {
                return NextRandomInt();
            }

            if (type == typeof(long))
            {
                return NextRandomLong();
            }

            if (type == typeof(double))
            {
                return NextRandomDouble();
            }

            if (type == typeof(float))
            {
                return NextRandomFloat();
            }

            if (type == typeof(string))
            {
                return NextRandomString(10);
            }

            if (type == typeof(DateTime))
            {
                return NextRandomDateTime();
            }

            if (type == typeof(Uri))
            {
                return NextRandomUri();
            }

            return null!;
        }

        private bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(TimeSpan)
                || type == typeof(Guid)
                || type == typeof(Uri);
        }

        internal int NextRandomInt()
        {
            return new Random().Next();
        }

        private long NextRandomLong()
        {
            return new Random().Next();
        }

        internal double NextRandomDouble()
        {
            return new Random().NextDouble();
        }

        private float NextRandomFloat()
        {
            return (float)new Random().NextDouble();
        }

        public string NextRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }

        public DateTime NextRandomDateTime()
        {
            var start = new DateTime(1995, 1, 1);
            var end = new DateTime(2050, 12, 31);
            int range = (end - start).Days;
            return start.AddDays(new Random().Next(range));
        }

        public Uri NextRandomUri()
        {
            var random = new Random();

            var scheme = "https";
            var amount = random.Next(7, 13);
            var host = NextRandomString(amount);
            var amount2 = random.Next(7, 13);
            var path = NextRandomString(amount2);
            var domain = NextRandomString(3);

            var url = $"{scheme}://{host}.{domain}/{path}";

            return new Uri(url);
        }

        public List<T> CreateList<T>(int count)
        {
            var list = new List<T>();

            for (int i = 0; i < count; i++)
            {
                list.Add(Create<T>());
            }

            return list;
        }
    }
}