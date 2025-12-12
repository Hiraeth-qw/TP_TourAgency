using System.Security.Claims;
using MicroserviceBooking.DTOs;
using MicroserviceBooking.Models;
using Microsoft.AspNetCore.Mvc;
using MicroserviceBooking.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using MicroserviceBooking.Controllers;
using Microsoft.AspNetCore.Http;

namespace MicroserviceBooking.Tests
{
    public class BookingTests
    {
        private BookingContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<BookingContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new BookingContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        private ControllerContext GetMockUserContext(string userId, string role = "Client")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        // СЦЕНАРИЙ 1: Успешное создание бронирования
        [Fact]
        public async Task CreateBooking_ReturnsOk_WhenRequestIsValid()
        {
            var dbContext = GetDatabaseContext();

            var mockTourService = new Mock<ITourService>();
            var mockPartnerService = new Mock<IPartnerService>();
            var mockPaymentService = new Mock<IPaymentService>();

            // Настройка мока Тура: Тур существует, цена 100, мест 10, партнеров нет (для упрощения)
            var tourDto = new ReadTour
            {
                Id = 1,
                Price = 100m,
                AvailableSeats = 10,
                PartnerIds = new List<int>()
            };

            mockTourService.Setup(s => s.GetTourAsync(It.IsAny<int>())).ReturnsAsync(tourDto);

            // Настройка мока: Резервация места прошла успешно
            mockTourService.Setup(s => s.ReserveSeatAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

            // Создаем контроллер
            var controller = new BookingController(dbContext, mockTourService.Object, mockPartnerService.Object, mockPaymentService.Object);
            controller.ControllerContext = GetMockUserContext("user1");

            var request = new CreateBookingRequest
            {
                TourId = 1,
                TouristsNumber = 2
            };

            var result = await controller.CreateBooking(request);
            var okResult = Assert.IsType<OkObjectResult>(result);

            var bookingInDb = await dbContext.Bookings.FirstOrDefaultAsync();
            Assert.NotNull(bookingInDb);
            Assert.Equal("user1", bookingInDb.UserId);
            Assert.Equal(BookingStatus.PendingPayment, bookingInDb.Status); // Статус должен смениться после резервации
            Assert.Equal(200m, bookingInDb.TotalAmount); // 2 места * 100 цена = 200
        }

        //СЦЕНАРИЙ 2: Ошибка, если тур не найден
        [Fact]
        public async Task CreateBooking_ReturnsNotFound_WhenTourDoesNotExist()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var mockTourService = new Mock<ITourService>();
            var mockPartnerService = new Mock<IPartnerService>();
            var mockPaymentService = new Mock<IPaymentService>();

            mockTourService.Setup(s => s.GetTourAsync(It.IsAny<int>())).ReturnsAsync((ReadTour)null);

            var controller = new BookingController(dbContext, mockTourService.Object, mockPartnerService.Object, mockPaymentService.Object);
            controller.ControllerContext = GetMockUserContext("user1");

            var request = new CreateBookingRequest { TourId = 999, TouristsNumber = 1 };
            var result = await controller.CreateBooking(request);

            Assert.IsType<NotFoundObjectResult>(result); // Ожидаем 404 Not Found
        }

        //СЦЕНАРИЙ 3: Ошибка, если недостаточно мест
        [Fact]
        public async Task CreateBooking_ReturnsConflict_WhenNotEnoughSeats()
        {
            var dbContext = GetDatabaseContext();
            var mockTourService = new Mock<ITourService>();
            var mockPartnerService = new Mock<IPartnerService>();
            var mockPaymentService = new Mock<IPaymentService>();

            // Настройка: Тур есть, но доступно всего 1 место
            var tourDto = new ReadTour { Id = 1, AvailableSeats = 1, Price = 100 };

            mockTourService.Setup(s => s.GetTourAsync(It.IsAny<int>())).ReturnsAsync(tourDto);

            var controller = new BookingController(dbContext, mockTourService.Object, mockPartnerService.Object, mockPaymentService.Object);
            controller.ControllerContext = GetMockUserContext("user1");

            var request = new CreateBookingRequest { TourId = 1, TouristsNumber = 5 };
            var result = await controller.CreateBooking(request);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Not enough seats available.", conflictResult.Value);
        }

        // СЦЕНАРИЙ 4: Получение списка бронирований пользователя
        [Fact]
        public async Task GetMyBookings_ReturnsOnlyUserBookings()
        {
            var dbContext = GetDatabaseContext();
            var mockTourService = new Mock<ITourService>();
            var mockPartnerService = new Mock<IPartnerService>();
            var mockPaymentService = new Mock<IPaymentService>();

            // Добавляем тестовые данные в In-Memory БД
            dbContext.Bookings.Add(new Booking { Id = 1, UserId = "user1", TourId = 10, BookingDate = DateTime.Now });
            dbContext.Bookings.Add(new Booking { Id = 2, UserId = "user1", TourId = 11, BookingDate = DateTime.Now });
            dbContext.Bookings.Add(new Booking { Id = 3, UserId = "user2", TourId = 12, BookingDate = DateTime.Now }); // Чужое бронирование
            await dbContext.SaveChangesAsync();

            var controller = new BookingController(dbContext, mockTourService.Object, mockPartnerService.Object, mockPaymentService.Object);
            controller.ControllerContext = GetMockUserContext("user1");

            var result = await controller.GetMyBookings();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var bookings = Assert.IsType<List<Booking>>(okResult.Value);

            Assert.Equal(2, bookings.Count); // Должно быть только 2 бронирования для user1
            Assert.All(bookings, b => Assert.Equal("user1", b.UserId));
        }

        //СЦЕНАРИЙ 5: Отмена бронирования и освобождение мес
        [Fact]
        public async Task CancelBooking_ChangesStatusAndReleasesSeats()
        {
            var dbContext = GetDatabaseContext();
            var mockTourService = new Mock<ITourService>();
            var mockPartnerService = new Mock<IPartnerService>();
            var mockPaymentService = new Mock<IPaymentService>();

            var booking = new Booking
            {
                Id = 1,
                UserId = "user1",
                TourId = 5,
                NumberOfSeats = 2,
                Status = BookingStatus.PendingPayment
            };
            dbContext.Bookings.Add(booking);
            await dbContext.SaveChangesAsync();

            mockTourService.Setup(s => s.ReleaseSeatAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

            var controller = new BookingController(dbContext, mockTourService.Object, mockPartnerService.Object, mockPaymentService.Object);
            controller.ControllerContext = GetMockUserContext("user1");

            var result = await controller.CancelBooking(1);
            var okResult = Assert.IsType<OkObjectResult>(result);

            var updatedBooking = await dbContext.Bookings.FindAsync(1);
            Assert.Equal(BookingStatus.Cancelled, updatedBooking.Status);
            Assert.Equal("Cancelled by user/manager.", updatedBooking.FailureReason);

            // Проверяем, что метод ReleaseSeatAsync был вызван 1 раз с правильными параметрами
            mockTourService.Verify(s => s.ReleaseSeatAsync(5, 2, It.IsAny<string>()), Times.Once);
        }
    }
}
