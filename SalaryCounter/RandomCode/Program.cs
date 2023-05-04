using RandomCode;

internal class Program
{
    private static void Main(string[] args)
    {
        var incomingValues = new IncomingValues
        {
            Salary = 150000,
            CurrentCourse = 5.6,
            AveragePriceForHour = 14800,
            ShiftsInMonth = 15
        };

        incomingValues = Logic.GetIncomingValues(incomingValues);

        var calculationType = Logic.GetCalculationType();

        while (true)
        {
            Console.WriteLine();

            switch (calculationType)
            {
                case CalculationType.ViaCompanyProfit:
                    Logic.CountViaCompanyProfit(incomingValues);
                    break;

                case CalculationType.ViaWorkingHourAmount:
                    Logic.CountViaWorkingHourAmount(incomingValues);
                    break;

                default:
                    throw new ArgumentException("App type does not exist yet.");
            }

            if (Logic.AskForExit())
                break;

            else if (Logic.AskForDetailedCalculation())
            {
                incomingValues = Logic.GetIncomingValues(incomingValues);
                calculationType = Logic.GetCalculationType();
            }

            else if (Logic.AskForNewCalculationType())
                calculationType = Logic.GetCalculationType();
        }
    }
}