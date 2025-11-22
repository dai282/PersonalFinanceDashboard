namespace PersonalFinance.API.DTOs
{
    public class BudgetDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Spent { get; set; }
        public decimal Remaining { get; set; }
        public decimal PercentageUsed { get; set; }
        public string Status { get; set; } = string.Empty; // "Under", "Near", "Over"
    }

    public class CreateBudgetDto
    {
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class UpdateBudgetDto
    {
        public decimal Amount { get; set; }
    }

    public class BudgetStatusDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalBudgeted { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalRemaining { get; set; }
        public IEnumerable<BudgetDto> Budgets { get; set; } = new List<BudgetDto>();
    }
}