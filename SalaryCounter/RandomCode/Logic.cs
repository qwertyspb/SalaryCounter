namespace RandomCode
{
    internal class Logic
    {
        internal static IncomingValues GetIncomingValues(IncomingValues incomingValues)
        {
            incomingValues.CompanyProfit = null;

            Console.Write("Would you like to use detailed salary calculation? (y/n): ");

            var y = Console.ReadLine();

            Console.WriteLine();

            return IsYes(y)
                ? SetDetailedIncomingValues(incomingValues)
                : ChangeIncomingValues(incomingValues);
        }

        internal static CalculationType GetCalculationType()
        {
            bool c;
            bool w;

            while (true)
            {
                Console.Write("Choose your variant of calculation.\r\nType \"c\" for \"via company profit\" or \"w\" for \"via working hour amount\": ");
                var appType = Console.ReadLine();

                c = appType.Equals("c", StringComparison.OrdinalIgnoreCase);
                w = appType.Equals("w", StringComparison.OrdinalIgnoreCase);

                if (c || w)
                    break;

                Console.WriteLine();
                Console.WriteLine("You chose the unexisting variant.");
                Console.WriteLine();
            }

            if (c)
                return CalculationType.ViaCompanyProfit;

            return CalculationType.ViaWorkingHourAmount;
        }

        internal static void CountViaCompanyProfit(IncomingValues incomingValues)
        {
            double companyProfit;

            if (!incomingValues.CompanyProfit.HasValue)
            {
                Console.Write("Write down the company profit: ");
                var income = Console.ReadLine();

                try
                {
                    companyProfit = double.Parse(income);
                }

                catch
                {
                    return;
                }
            }

            else
                companyProfit = incomingValues.CompanyProfit.Value;

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

            Console.WriteLine();

            return IsYes(y);
        }

        internal static bool AskForNewCalculationType()
        {
            Console.Write("Do you want to switch calculation type (y/n): ");
            var y = Console.ReadLine();

            Console.WriteLine();

            return IsYes(y);
        }

        internal static bool AskForDetailedCalculation()
        {
            Console.Write("Do you want to change for detailed/non-detailed salary calculation? (y/n): ");
            var y = Console.ReadLine();

            Console.WriteLine();

            return IsYes(y);
        }

        private static IncomingValues ChangeIncomingValues(IncomingValues incomingValues)
        {
            var consts = new List<string>
            {
                "Current constants:",
                $"{Consts.Salary}{incomingValues.Salary}",
                $"{Consts.Course}{incomingValues.CurrentCourse}",
                $"{Consts.Price}{incomingValues.AveragePriceForHour}",
                $"{Consts.Shifts}{incomingValues.ShiftsInMonth}",
                $"{Consts.Profit}{incomingValues.CompanyProfit}"
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

        private static IncomingValues SetIncomingValues(IncomingValues incomingValues)
        {
            Console.WriteLine("Set constants in turn. If current constant is ok for you, skip by pressing \"Enter\".");

            var dictionary = new Dictionary<IncomingValueType, string>
            {
                { IncomingValueType.Salary, Consts.Salary },
                { IncomingValueType.CurrentCourse, Consts.Course },
                { IncomingValueType.AveragePrice, Consts.Price },
                { IncomingValueType.ShiftsInMonth, Consts.Shifts }
            };

            foreach (var (key, param) in dictionary)
            {
                var value = GetParsedIncomeForParam(param);

                if (value < 0)
                    continue;

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
                        throw new Exception("Unexpected constant type.");
                }
            }

            Console.WriteLine();

            return incomingValues;
        }

        private static IncomingValues SetDetailedIncomingValues(IncomingValues incomingValues)
        {
            incomingValues.Salary = 0;
            var hours = new List<double>();

            incomingValues.ShiftsInMonth = GetParsedIncomeForParam(Consts.Shifts);

            Console.WriteLine("Write down the amount of work hours.");

            for (var i = 1;  i <= incomingValues.ShiftsInMonth; i++)
            {
                var workHours = GetParsedIncomeForParam($"Shift number {i}: ");

                hours.Add(workHours);

                var daySalary = Consts.DaySalary;

                if (workHours <= Consts.HalfHoursOfShift)
                {
                    Console.Write($"Was it a full day? (y/n): ");

                    var y = Console.ReadLine();

                    if (!IsYes(y))
                        daySalary /= 2;
                }

                incomingValues.Salary += daySalary;
            }

            incomingValues.CompanyProfit = incomingValues.AveragePriceForHour * hours.Sum();

            Console.WriteLine();

            return ChangeIncomingValues(incomingValues);
        }

        private static double GetParsedIncomeForParam(string param)
        {
            Console.Write(param);

            var income = Console.ReadLine();

            if (string.IsNullOrEmpty(income))
                return -1;

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

            return value;
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
