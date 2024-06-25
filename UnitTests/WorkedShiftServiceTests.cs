using Castle.Core.Logging;
using Domain.Entities.UKG;
using Microsoft.Extensions.Logging;
using Moq;
using UKG_Integration_Application.Database.BaseRepo;
using UKG_Integration_Application.Database.DAL;
using UKG_Integration_Application.Services.PersistenceServices;

namespace UnitTests
{
    public class WorkedShiftServiceTests
    {
        private readonly Mock<IBaseRepository<WorkedShift>> _mockWorkedShiftRepo;
        private readonly Mock<IScheduleShiftDAL> _mockScheduleShiftDAL;
        private readonly Mock<IWorkedShiftDAL> _mockWorkedShiftDAL;
        private readonly WorkedShiftService _workedShiftService;
        private readonly Mock<ILogger<WorkedShift>> _mockWorkedShiftLogger;

        public WorkedShiftServiceTests()
        {
            _mockWorkedShiftRepo = new Mock<IBaseRepository<WorkedShift>>();
            _mockScheduleShiftDAL = new Mock<IScheduleShiftDAL>();
            _mockWorkedShiftDAL = new Mock<IWorkedShiftDAL>();
            _mockWorkedShiftLogger = new Mock<ILogger<WorkedShift>>();
            //TODO: figure out what to do with null param
            _workedShiftService = new WorkedShiftService(_mockWorkedShiftRepo.Object, _mockScheduleShiftDAL.Object, _mockWorkedShiftDAL.Object, null);
        }

        [Fact]
        public async Task AddOrUpdateWorkedShiftsAsync_ShouldAddNewShifts_WhenDatabaseIsEmpty()
        {
            // Arrange
            var employee1 = new Employee { PersonId = 101 };
            var employee2 = new Employee { PersonId = 102 };

            var scheduleShift1 = new ScheduleShift { Id = 1, Type = "REGULAR_SEGMENT", Employee = employee1 };
            var scheduleShift2 = new ScheduleShift { Id = 2, Type = "REGULAR_SEGMENT", Employee = employee2 };

            var workedShifts = new List<WorkedShift>
            {
                new WorkedShift
                {
                    Id = 1001,
                    PersonId = 101,
                    ScheduledShiftId = 1,
                    PunchIn = DateTime.Now,
                    PunchOut = DateTime.Now.AddHours(8),
                    Exception = null,
                    Employee = employee1,
                    ScheduleShift = scheduleShift1
                },
                new WorkedShift
                {
                    Id = 2001,
                    PersonId = 102,
                    ScheduledShiftId = 2,
                    PunchIn = DateTime.Now,
                    PunchOut = DateTime.Now.AddHours(7),
                    Exception = null,
                    Employee = employee2,
                    ScheduleShift = scheduleShift2
                }
            };

            _mockWorkedShiftRepo.Setup(repo => repo.Any()).Returns(false);
            _mockScheduleShiftDAL.Setup(dal => dal.GetMaxDateScheduleShiftIds()).ReturnsAsync(new List<int> { 100, 101 });

            // Act
            await _workedShiftService.AddOrUpdateWorkedShiftsAsync(workedShifts);

            // Assert
            _mockWorkedShiftRepo.Verify(repo => repo.AddRangeAsync(It.Is<List<WorkedShift>>(list => list.Count == 2), It.IsAny<CancellationToken>()), Times.Once);
            _mockWorkedShiftRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }


    }
}