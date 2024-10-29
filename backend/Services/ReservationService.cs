using backend.Data.Entities;

namespace backend.Services
{
    public class ReservationService
    {
        public decimal CalculateCost(BookType type, int days, bool quickPickup)
        {
            decimal dailyRate = type == BookType.Book ? 2 : 3;
            decimal baseCost = dailyRate * days;

            // Apply discounts
            if (days > 10)
            {
                baseCost *= 0.8m; // 20% off
            }
            else if (days > 3)
            {
                baseCost *= 0.9m; // 10% off
            }

            // Add service and quick-pickup fees
            decimal totalCost = baseCost + 3; // €3 service fee
            if (quickPickup)
            {
                totalCost += 5; // €5 quick pickup fee
            }

            return totalCost;
        }
    }
}
    

