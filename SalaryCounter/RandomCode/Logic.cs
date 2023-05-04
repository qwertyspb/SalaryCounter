namespace RandomCode
{
    internal class Logic
    {
        internal static AccountingType GetAccountingType()
        {
            bool c;
            bool p;

            while (true)
            {
                Console.Write("Choose your variant of accounting.\r\nType \"c\" for \"via company profit\" or \"p\" for \"via procedure amount\": ");
                var appType = Console.ReadLine();

                c = appType.Equals("c", StringComparison.OrdinalIgnoreCase);
                p = appType.Equals("p", StringComparison.OrdinalIgnoreCase);

                if (c || p)
                    break;

                Console.WriteLine();
                Console.WriteLine("You chose the unexisting variant.");
                Console.WriteLine();
            }

            if (c)
                return AccountingType.ViaCompanyProfit;

            return AccountingType.ViaWorkingHourAmount;
        }

        internal static void CountViaCompanyProfit(IncomingValues incomingValues)
        {
            Console.Write("Write down the company profit: ");
            var income = Console.ReadLine();

            double companyProfit;

            try
            {
                companyProfit = double.Parse(income);
            }

            catch
            {
                return;
            }

            var percent = companyProfit switch
            {
                < 300000 => 0,
                < 1000000 => 5,
                < 2000000 => 10,
                _ => 15
            };

            var bonus = GetBonus(companyProfit);

            var total = bonus + incomingValues.Salary;

            var procedureAmount = companyProfit / incomingValues.AveragePriceForHour / incomingValues.ShiftsInMonth;

            Console.WriteLine(GetEmployeeAmount(total, incomingValues.CurrentCourse));

            Console.WriteLine($"Approximate amount of work hours a day: {Math.Round(procedureAmount, 1)}");
        }

        internal static void CountViaWorkingHourAmount(IncomingValues incomingValues)
        {
            Console.Write("Write down the desirable amount of work hours a day: ");
            var income = Console.ReadLine();

            double procedureAmount;

            try
            {
                procedureAmount = double.Parse(income);
            }
            catch
            {
                return;
            }

            var companyProfit = procedureAmount * incomingValues.AveragePriceForHour * incomingValues.ShiftsInMonth;

            var bonus = GetBonus(companyProfit);

            var total = bonus + incomingValues.Salary;

            Console.WriteLine(GetEmployeeAmount(total, incomingValues.CurrentCourse));
        }

        internal static bool AskForExit()
        {
            Console.WriteLine();

            Console.Write("Do you want to exit (y/n): ");
            var y = Console.ReadLine();

            return IsYes(y);
        }

        internal static bool AskForNewAccountingType()
        {
            Console.WriteLine();

            Console.Write("Do you want to switch accounting type (y/n): ");
            var y = Console.ReadLine();

            return IsYes(y);
        }

        internal static IncomingValues ChangeIncomingValues(IncomingValues incomingValues)
        {
            var consts = new List<string>
            {
                "Current constants:",
                $"{Consts.Salary}{incomingValues.Salary}",
                $"{Consts.Course}{incomingValues.CurrentCourse}",
                $"{Consts.Price}{incomingValues.AveragePriceForHour}",
                $"{Consts.Shifts}{incomingValues.ShiftsInMonth}"
            };

            Console.WriteLine(string.Join("\r\n", consts));
            Console.WriteLine();

            Console.Write("Do you want to change any of constants? (y/n): ");

            var y = Console.ReadLine();
            Console.WriteLine();

            if (!IsYes(y))
                return incomingValues;

            incomingValues = SetIncomingValues(incomingValues);

            return ChangeIncomingValues(incomingValues);
        }

        internal static IncomingValues SetIncomingValues(IncomingValues incomingValues)
        {
            Console.WriteLine("Set constants in turn. If current constant is ok for you, skip with pressing \"Enter\".");

            var dictionary = new Dictionary<IncomingValueType, string>
            {
                { IncomingValueType.Salary, Consts.Salary },
                { IncomingValueType.CurrentCourse, Consts.Course },
                { IncomingValueType.AveragePrice, Consts.Price },
                { IncomingValueType.ShiftsInMonth, Consts.Shifts }
            };

            foreach (var (key, param) in dictionary)
            {
                Console.Write(param);

                var income = Console.ReadLine();

                if (string.IsNullOrEmpty(income))
                    continue;

                var isUnparsed = true;
                double value = 0;

                while (isUnparsed)
                {
                    isUnparsed = !TryParse(income, out var output);
                    value = output;

                    if (isUnparsed)
                    {
                        Console.WriteLine("Write down the correct value.");

                        Console.Write(param);
                        income = Console.ReadLine();
                    }
                }

                switch (key)
                {
                    case IncomingValueType.Salary:
                        incomingValues.Salary = value;
                        break;

                    case IncomingValueType.CurrentCourse:
                        incomingValues.CurrentCourse = value;
                        break;

                    case IncomingValueType.AveragePrice:
                        incomingValues.AveragePriceForHour = value;
                        break;

                    case IncomingValueType.ShiftsInMonth:
                        incomingValues.ShiftsInMonth = value;
                        break;

                    default:
                        throw new Exception("Unexpected constant type");
                }
            }

            Console.WriteLine();

            return incomingValues;
        }

        private static bool IsYes(string y)
            => y.Equals("y", StringComparison.OrdinalIgnoreCase);

        private static bool TryParse(string income, out double value)
        {
            value = 0;

            try
            {
                value = double.Parse(income);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static string GetEmployeeAmount(double total, double currentCourse)
        => $"Employee total amount: {Math.Round(total, 1)} KZT or {Math.Round(total / currentCourse, 1)} RUB";

        private static double GetBonus(double companyProfit)
        {
            var percent = companyProfit switch
            {
                < 300000 => 0,
                < 1000000 => 5,
                < 2000000 => 10,
                _ => 15
            };

            return companyProfit * percent / 100;
        }
    }
}
