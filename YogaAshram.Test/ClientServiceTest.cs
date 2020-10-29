using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;
using yogaAshram.Models;
using yogaAshram.Services;

namespace YogaAshram.Test
{
    public class ClientServiceTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        DateTime _date=new DateTime(2020,10,27);
        public ClientServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }


        [Fact]
        public void ShouldReturnListOfDates()
        {
             var options= new DbContextOptionsBuilder<YogaAshramContext>().UseInMemoryDatabase(databaseName:Guid.NewGuid().ToString()).Options;
                     var context=new YogaAshramContext(options);
                     Seed(context);
            var query = new ClientServices(context);
        
            var result = query.DatesForAttendance(_date, 1, 8);
            _testOutputHelper.WriteLine(result[0].ToShortDateString());
            Assert.Equal(8,result.Count);
        }
        
        [Fact]
        public void ShouldReturnEndDate()
        {
            var options= new DbContextOptionsBuilder<YogaAshramContext>().UseInMemoryDatabase(databaseName:Guid.NewGuid().ToString()).Options;
            var context=new YogaAshramContext(options);
            Seed(context);
            var query = new ClientServices(context);
    
            var result = query.EndDateForClientsMembership(_date, 1, 4);
            _testOutputHelper.WriteLine(result.ToString("d"));
            Assert.Equal(new DateTime(2020, 11,5), result.Date);
        }
        
        [Fact]
        public void ShouldReturnTwoDates()
        {
            var options= new DbContextOptionsBuilder<YogaAshramContext>().UseInMemoryDatabase(databaseName:Guid.NewGuid().ToString()).Options;
            var context=new YogaAshramContext(options);
            Seed(context);
            var query = new ClientServices(context);
        
            var result = query.TwoTimesTrial(1,DateTime.Now);
          
            Assert.Equal(2, result.Count);
        }
        
     
        
        [Fact]
        public void ShouldReturnOneDate()
        {
            
            var options= new DbContextOptionsBuilder<YogaAshramContext>().UseInMemoryDatabase(databaseName:Guid.NewGuid().ToString()).Options;
            var context=new YogaAshramContext(options);
            Seed(context);
            var query = new ClientServices(context);
        
            var result = query.DateIfFrozen(_date,1);
            _testOutputHelper.WriteLine(result.ToString("d"));
            Assert.Equal(DateTime.Now.AddDays(1).Date, result.Date);
           
        }
        
        
        private void Seed(YogaAshramContext context)
        {
            List<CalendarEvent> calendarEvents = new List<CalendarEvent>()
            {
                new CalendarEvent()
                {
                    Id = 1,
                    BranchId = 1,
                    DayOfWeek = DayOfWeek.Tuesday,
                    Action = "/Schedule/Group/?groupId=1&branchId=1",
                    GroupId = 1,
                    Type = "warning",
                    TimeStart = new TimeSpan(11, 00, 00),
                    TimeFinish = new TimeSpan(12, 00, 00)
                },
                new CalendarEvent()
                {
                    Id = 2,
                    BranchId = 1,
                    DayOfWeek = DayOfWeek.Thursday,
                    Action = "/Schedule/Group/?groupId=1&branchId=1",
                    GroupId = 1,
                    Type = "warning",
                    TimeStart = new TimeSpan(11, 00, 00),
                    TimeFinish = new TimeSpan(12, 00, 00)
                }
            };
            
       
            context.CalendarEvents.AddRange(calendarEvents);
            context.SaveChanges();
        }
    }
}