using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleCoreDelegate
{
    public delegate double Calc(double x, double y);
    class Program
    {
        static void Main(string[] args)
        {
            // invoke directly
            Calculator calc = new Calculator();
            double x = 300;
            double y = 100;
            calc.Report();
            Console.WriteLine(calc.Add(x, y));
            Console.WriteLine(calc.Add(x, y));
            Console.WriteLine(calc.Add(x, y));
            Console.WriteLine(calc.Add(x, y));

            // action: delegate with function WITHOUT return value
            Action action = new Action(calc.Report);
            action.Invoke();
            action();
            // func: delegate with function WITH return value
            Func<double, double, double> func1 = new Func<double, double, double>(calc.Add);
            Func<double, double, double> func2 = new Func<double, double, double>(calc.Sub);
            Func<double, double, double> func3 = new Func<double, double, double>(calc.Mul);
            Func<double, double, double> func4 = new Func<double, double, double>(calc.Div);
            Console.WriteLine(func1.Invoke(x, y));
            Console.WriteLine(func2.Invoke(x, y));
            Console.WriteLine(func3.Invoke(x, y));
            Console.WriteLine(func4.Invoke(x, y));
            Console.WriteLine(func1(x, y));
            Console.WriteLine(func2(x, y));
            Console.WriteLine(func3(x, y));
            Console.WriteLine(func4(x, y));

            // delegate
            Calc calc1 = new Calc(calc.Add);
            Calc calc2 = new Calc(calc.Sub);
            Calc calc3 = new Calc(calc.Mul);
            Calc calc4 = new Calc(calc.Div);
            Console.WriteLine(calc1.Invoke(x, y));
            Console.WriteLine(calc2.Invoke(x, y));
            Console.WriteLine(calc3.Invoke(x, y));
            Console.WriteLine(calc4.Invoke(x, y));
            Console.WriteLine(calc1(x, y));
            Console.WriteLine(calc2(x, y));
            Console.WriteLine(calc3(x, y));
            Console.WriteLine(calc4(x, y));

            // template
            ProductFactory productFactory = new ProductFactory();
            BoxFactory boxFactory = new BoxFactory();
            Logger logger = new Logger();
            Box burgerBox = boxFactory.Package(productFactory.ProduceBurger, logger.Log);
            Box pizzaBox = boxFactory.Package(productFactory.ProducePizza, logger.Log);
            Console.WriteLine(burgerBox.Product.Name);
            Console.WriteLine(pizzaBox.Product.Name);


            // interface instead of delegate
            Console.WriteLine("// interface instead of delegate");
            IProductFactory goldBurgerFactory = new GoldBurgerFactory();
            IProductFactory pizzaFactory = new PizzaFactory();
            Box burgerBoxNew = boxFactory.InterfacePackage(goldBurgerFactory);
            Box pizzaBoxNew = boxFactory.InterfacePackage(pizzaFactory);
            Console.WriteLine(burgerBox.Product.Name);
            Console.WriteLine(pizzaBox.Product.Name);

            // single / multi thread
            Console.WriteLine("single / multi thread demo");
            // explicit sync
            Console.WriteLine("->explicit sync");
            Student stu1 = new Student() { ID = 1, ConsoleColor = ConsoleColor.Red};
            Student stu2 = new Student() { ID = 2, ConsoleColor = ConsoleColor.Yellow };
            Student stu3 = new Student() { ID = 3, ConsoleColor = ConsoleColor.Blue };
            stu1.DoneHomework();
            stu2.DoneHomework();
            stu3.DoneHomework();

            // implicit sync
            Console.WriteLine("->implicit sync");
            //Student stu1 = new Student() { ID = 1, consoleColor = ConsoleColor.Red };
            //Student stu2 = new Student() { ID = 2, consoleColor = ConsoleColor.Yellow };
            //Student stu3 = new Student() { ID = 3, consoleColor = ConsoleColor.Blue };
            Action stuAction1 = new Action(stu1.DoneHomework);
            Action stuAction2 = new Action(stu2.DoneHomework);
            Action stuAction3 = new Action(stu3.DoneHomework);

            Console.WriteLine("->->a Single cast");
            stuAction1.Invoke();
            stuAction2.Invoke();
            stuAction3.Invoke();

            Console.WriteLine("->->b Multi cast");
            stuAction1 += stuAction2;
            stuAction1 += stuAction3;

            // explicit async using thread / task
            Console.WriteLine("->explicit async");
            // old way
            Thread thread1 = new Thread(new ThreadStart(stu1.DoneHomework));
            Thread thread2 = new Thread(new ThreadStart(stu2.DoneHomework));
            Thread thread3 = new Thread(new ThreadStart(stu3.DoneHomework));
            thread1.Start();
            thread2.Start();
            thread3.Start();
            // new way
            Task task1 = new Task(stu1.DoneHomework);
            Task task2 = new Task(stu2.DoneHomework);
            Task task3 = new Task(stu3.DoneHomework);
            task1.Start();
            task2.Start();
            task3.Start();

            // implicit async
            Console.WriteLine("->implicit async");

            //Action stuAction1 = new Action(stu1.DoneHomework);
            //Action stuAction2 = new Action(stu2.DoneHomework);
            //Action stuAction3 = new Action(stu3.DoneHomework);

            Console.WriteLine("->->a Single cast");
            stuAction1.BeginInvoke(null, null);
            stuAction2.BeginInvoke(null, null);
            stuAction3.BeginInvoke(null, null);

            Console.WriteLine("->->b Multi cast");
            stuAction1 += stuAction2;
            stuAction1 += stuAction3;
            stuAction1.BeginInvoke(null, null);
        }
    }
    class Calculator
    {
        public void Report()
        {
            Console.WriteLine("This is a void functin without parameters");
        }
        public double Add(double x, double y)
        {
            return x + y;
        }
        public double Sub(double x, double y)
        {
            return x - y;
        }
        public double Mul(double x, double y)
        {
            return x * y;
        }
        public double Div(double x, double y)
        {
            return x / y;
        }
    }

    class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

    }
    class Box
    {
        public Product Product { get; set; }

    }
    class ProductFactory
    {
        public Product ProduceBurger()
        {
            return new Product()
            {
                Name = "Gold Burger",
                Price = 110
            };
        }

        public Product ProducePizza()
        {
            return new Product()
            {
                Name = "Pizza",
                Price = 10
            };
        }
    }
    class BoxFactory
    {
        public Box Package(Func<Product> getProduct, Action<Product> logCallback)
        {
            Product product = getProduct.Invoke();
            Box box = new Box();
            box.Product = product;
            if(product.Price > 100)
            {
                logCallback(product);
            }
            return box;
        }

        public Box InterfacePackage(IProductFactory productFactory)
        {
            Product product = productFactory.Make();
            Box box = new Box();
            box.Product = product;
            return box;
        }
    }

    class Logger
    {
        public void Log(Product product)
        {
            Console.WriteLine($"{product.Name} was created on {DateTime.UtcNow}. Price: {product.Price}");
        }
    }

    interface IProductFactory
    {
        Product Make();
    }

    class GoldBurgerFactory : IProductFactory
    {
        public Product Make()
        {
            return new Product()
            {
                Name = "Gold Burger",
                Price = 110
            };
        }
    }

    class PizzaFactory : IProductFactory
    {
        public Product Make()
        {
            return new Product()
            {
                Name = "Pizza",
                Price = 10
            };
        }
    }

    class Student
    {
        public int ID { get; set; }
        public ConsoleColor ConsoleColor { get; set; }

        public void DoneHomework()
        {
            
            for (int i = 0; i < 5; i++)
            {
                Console.BackgroundColor = ConsoleColor;
                Console.WriteLine($"Student {ID} is doing home work for {i} hours.");
                Thread.Sleep(1000);
            }

        }
    }
}
