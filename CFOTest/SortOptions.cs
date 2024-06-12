using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public static class SortOptions
    {
        public static List<IncomeSortOptions> GetIncomeSortOptionsList()
        {
            var enumValues = Enum.GetValues(typeof(IncomeSortOptions)).Cast<IncomeSortOptions>();
            return enumValues.ToList();
        }

        public static List<ExpenseSortOptions> GetExpenseSortOptionsList()
        {
            var enumValues = Enum.GetValues(typeof(ExpenseSortOptions)).Cast<ExpenseSortOptions>();
            return enumValues.ToList();
        }

        public static List<BudgetSortOptions> GetBudgetSortOptionsList()
        {
            var enumValues = Enum.GetValues(typeof(BudgetSortOptions)).Cast<BudgetSortOptions>();
            return enumValues.ToList();
        }
    }

    public enum IncomeSortOptions
    {
        [Description("Label")]
        ByLabel,
        [Description("Amount")]
        ByAmount,
        [Description("Category")]
        ByCategory,
        [Description("Date Added")]
        ByAddedDate
    }

    public enum ExpenseSortOptions
    {
        [Description("Label")]
        ByLabel,
        [Description("Amount")]
        ByAmount,
        [Description("Payee")]
        ByPayee,
        [Description("Category")]
        ByCategory,
        [Description("Due Date")]
        ByDueDate,
        [Description("Paid Status")]
        ByPaidStatus,
        [Description("Date Added")]
        ByDateAdded
    }

    public enum BudgetSortOptions
    {
        [Description("Label")]
        ByLabel,
        [Description("Amount")]
        ByAmount,
        [Description("Remaining Amount")]
        ByRemainingAmount,
        [Description("Category")]
        ByCategory,
        [Description("Closed Status")]
        ByClosedStatus,
        [Description("Date Added")]
        ByDateAdded
    }

    public enum ReceiptSortOptions
    {
        [Description("Label")]
        ByLabel,
        [Description("Amount")]
        ByAmount,
        [Description("Payee")]
        ByPayee,
        [Description("Category")]
        ByCategory,
        [Description("TransactionDate")]
        ByTransactionDate,
        [Description("Date Added")]
        ByDateAdded
    }
}
