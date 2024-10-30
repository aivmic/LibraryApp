using backend.Data.Entities;
using backend.Services;
using Xunit;

namespace Tests 
{
    public class ReservationServiceTests
    {
        private readonly ReservationService _reservationService;

        public ReservationServiceTests()
        {
            _reservationService = new ReservationService();
        }

        [Theory]
        [InlineData(BookType.Book, 2, false, 7)]   // 2 days, regular pickup, Book type
        [InlineData(BookType.Audiobook, 2, false, 9)] // 2 days, regular pickup, Audiobook type
        [InlineData(BookType.Book, 4, false, 10.2)]   // 4 days, regular pickup, Book type with 10% discount
        [InlineData(BookType.Book, 11, false, 20.6)]  // 11 days, regular pickup, Book type with 20% discount
        [InlineData(BookType.Book, 11, true, 25.6)]   // 11 days, quick pickup, Book type with 20% discount
        public void CalculateCost_ShouldReturnCorrectCost(BookType type, int days, bool quickPickup, decimal expectedCost)
        {
            decimal result = _reservationService.CalculateCost(type, days, quickPickup);
            
            Assert.Equal(expectedCost, result);
        }

        [Fact]
        public void CalculateCost_ShouldIncludeServiceFee()
        {
            var type = BookType.Book;
            int days = 1;
            bool quickPickup = false;
            
            decimal result = _reservationService.CalculateCost(type, days, quickPickup);
            
            Assert.True(result > 3, "Total cost should include the service fee of €3.");
        }

        [Fact]
        public void CalculateCost_ShouldAddQuickPickupFee_WhenQuickPickupIsTrue()
        {
            var type = BookType.Book;
            int days = 1;
            bool quickPickup = true;
            
            decimal result = _reservationService.CalculateCost(type, days, quickPickup);
            
            Assert.Equal(10, result);
        }
    }
}
