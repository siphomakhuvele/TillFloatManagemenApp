using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;


namespace Till_Float_Management
{
    class TillFloaManagement
    {
        static void Main()
        {
            string inputFilePath = "C:\\Users\\sipho\\OneDrive\\Desktop\\TillFloatManagemenApp\\input.txt";
            string outputFilePath = "C:\\Users\\sipho\\OneDrive\\Desktop\\TillFloatManagemenApp\\output.txt";

            Dictionary<int, int> till = new Dictionary<int, int>
            {
                { 50, 5 }, { 20, 5 }, { 10, 6 }, { 5, 12 }, { 2, 10 }, { 1, 10 }
            };

            int tillTotal = CalculateTotal(till);

            using (StreamReader sr = new StreamReader(inputFilePath))
            using (StreamWriter sw = new StreamWriter(outputFilePath))
            {
                sw.WriteLine("Till Float Management");
                sw.WriteLine("====================================================================");
                sw.WriteLine("Till Start Transaction: Total Paid, Change Total, Change Breakdown");
                sw.WriteLine("====================================================================\n");
                
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    string itemsPart = parts[0];
                    string[] paymentParts = parts[1].Split('-');

                    int transactionTotal = CalculateTransactionTotal(itemsPart);
                    int paidTotal = CalculatePaidTotal(paymentParts);

                    int changeTotal = paidTotal - transactionTotal;
                    Dictionary<int, int> changeBreakdown = CalculateChange(changeTotal, till);

                    string changeBreakdownStr = string.Join("-", changeBreakdown.Select(kv => string.Concat(Enumerable.Repeat(kv.Key + " ", kv.Value)).Trim()));
                    sw.WriteLine($"Till Start Total: R{tillTotal}, Transaction Total: R{transactionTotal}, Amount Paid:  R{paidTotal}, Change Total: R{changeTotal}, Change Breackdown: {changeBreakdownStr}");

                    UpdateTill(paymentParts, changeBreakdown, till);
                    tillTotal = CalculateTotal(till);
                }

                sw.WriteLine($"Till Closing Total: R{tillTotal}");
            }
        }

        static int CalculateTransactionTotal(string itemsPart)
        {
            return itemsPart.Split(';').Sum(item => int.Parse(item.Split('R')[1]));
        }

        static int CalculatePaidTotal(string[] paymentParts)
        {
            return paymentParts.Sum(p => int.Parse(p.TrimStart('R')));
        }

        static Dictionary<int, int> CalculateChange(int changeTotal, Dictionary<int, int> till)
        {
            Dictionary<int, int> changeBreakdown = new Dictionary<int, int>();
            foreach (var denom in till.Keys.OrderByDescending(x => x))
            {
                int count = Math.Min(changeTotal / denom, till[denom]);
                if (count > 0)
                {
                    changeBreakdown[denom] = count;
                    changeTotal -= count * denom;
                }
            }

            if (changeTotal != 0)
            {
                throw new InvalidOperationException("Insufficient funds to provide change.");
            }

            return changeBreakdown;
        }

        static void UpdateTill(string[] paymentParts, Dictionary<int, int> changeBreakdown, Dictionary<int, int> till)
        {
            foreach (var payment in paymentParts)
            {
                int denominations = int.Parse(payment.TrimStart('R'));
                if (!till.ContainsKey(denominations))
                    till[denominations] = 0;
                till[denominations]++;
            }

            foreach (var change in changeBreakdown)
            {
                till[change.Key] -= change.Value;
            }
        }

        static int CalculateTotal(Dictionary<int, int> till)
        {
            return till.Sum(kv => kv.Key * kv.Value);
        }
    }
}