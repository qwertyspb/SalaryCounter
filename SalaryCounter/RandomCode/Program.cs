using RandomCode;

internal class Program
{
    private static void Main(string[] args)
    {
        var incomingValues = new IncomingValues
        {
            Salary = 150000,
            CurrentCourse = 5.5,
            AveragePriceForHour = 13000,
            ShiftsInMonth = 15
        };

        incomingValues = Logic.ChangeIncomingValues(incomingValues);

        var accountingType = Logic.GetAccountingType();

        while (true)
        {
            Console.WriteLine();

            switch (accountingType)
            {
                case AccountingType.ViaCompanyProfit:
                    Logic.CountViaCompanyProfit(incomingValues);
                    break;

                case AccountingType.ViaWorkingHourAmount:
                    Logic.CountViaWorkingHourAmount(incomingValues);
                    break;

                default:
                    throw new ArgumentException("App type does not exist yet.");
            }

            if (Logic.AskForExit())
                break;

            if (Logic.AskForNewAccountingType())
                accountingType = Logic.GetAccountingType();
        }
    }


}