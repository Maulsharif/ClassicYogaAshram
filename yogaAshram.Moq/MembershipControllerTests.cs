using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using yogaAshram.Moq;
using NUnit.Framework;
using Telegram.Bot.Types;
using Xunit;
using yogaAshram.Controllers;
using yogaAshram.Models;
using yogaAshram.Services;
using Assert = NUnit.Framework.Assert;

namespace yogaAshram.Moq
{
    public partial interface IRepository
    {
        IEnumerable<Membership> GetAll();
        Membership Get(int id);
        void Create(Membership membership);
    }
    
    public class MembershipController : Controller
    {
        IRepository repo;
        public MembershipController(IRepository r)
        {
            repo = r;
        }
        public IActionResult Index()
        {
            return View("Index", repo.GetAll());
        }
    }

    public class MembershipControllerTests
    {
        private YogaAshramContext _db;
        private UserManager<User> _userManager;

       

        [Fact]
        public void IndexViewResultNotNull()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.Setup(repo => repo.GetAll()).Returns(GetTestOrders());
            var controller = new MembershipController(mock.Object);
            // Act
            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.NotNull(result);
        }




        private List<Membership> GetTestOrders()
        {
            var memberships = new List<Membership>
            {
                new Membership
                {
                    Id = 100111,
                    Name = "Абонемент",
                    Price = 13333,
                    AttendanceDays = 5,
                    CategoryId = 1

                },
                new Membership
                {
                    Id = 100112,
                    Name = "Абонемент 2",
                    Price = 13333,
                    AttendanceDays = 12,
                    CategoryId = 2
                }

            };
            return memberships;
        }







        // public class RenderPdfDocument
        // {
        //     private readonly ContractPdfService _service;
        //     private readonly Mock<IRepository<Client>> _db=new Mock<IRepository<Client>>();
        //
        //     public RenderPdfDocument(ContractPdfService service)
        //     {
        //         _service = service;
        //     }
        //
        //     [Fact]
        //     public void RenderPdfDocument_ReturnRes()
        //     {
        //         long id = 1;
        //         
        //         var action = _service.RenderPdfDocument(id);
        //        
        //         var expected = "application/pdf";
        //         Assert.AreEqual(expected, action);
        //     }
        //     
        // }

    }
}