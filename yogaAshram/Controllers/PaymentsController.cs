using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;
using yogaAshram.Services;

namespace yogaAshram.Controllers
{
    
    public class PaymentsController : Controller
    {
        public static string[] months = {"Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
        "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"};
        private readonly YogaAshramContext _db;
        private readonly UserManager<Employee> _userManager;
        private readonly PaymentsService _paymentsService;

        public PaymentsController(YogaAshramContext db, UserManager<Employee> userManager, PaymentsService paymentsService)
        {
            _db = db;
            _userManager = userManager;
            _paymentsService = paymentsService;
        }
        private async Task<PaymentsIndexModelView> SortPayments(PaymentsIndexModelView model, int pageTo)
        {
            var payments = GetFilteredByDate(model.From, model.To);
            switch (model.SortSelect)
            {
                case SortPaymentsBy.None:
                    return model;
                case SortPaymentsBy.Memberships:
                    if (model.SortReverse)
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.ClientsMembership.MembershipId).ToListAsync();
                    else
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.ClientsMembership.MembershipId).ToListAsync();
                    break;
                case SortPaymentsBy.Price:
                    if (model.SortReverse)
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => (p.ClientsMembership.Membership.Price - p.Debts)).ToListAsync();
                    else
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => (p.ClientsMembership.Membership.Price - p.Debts)).ToListAsync();
                    break;
                case SortPaymentsBy.Group:
                    if (model.SortReverse)
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.ClientsMembership.Client.GroupId).ToListAsync();
                    else
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.ClientsMembership.Client.GroupId).ToListAsync();
                    break;
                case SortPaymentsBy.Comment:
                    if (model.SortReverse)
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.Comment).ToListAsync();
                    else
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.Comment).ToListAsync();
                    break;
                case SortPaymentsBy.Sickness:
                    if (model.SortReverse)
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.ClientsMembership.Client.Sickness).ToListAsync();
                    else
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.ClientsMembership.Client.Sickness).ToListAsync();
                    break;
                case SortPaymentsBy.Debtors:
                    if (model.SortReverse)
                        model.Payments = await payments.Where(p => p.ClientsMembership.Client.Balance < 0).Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.ClientsMembership.Client.Balance).ToListAsync();
                    else
                        model.Payments = await payments.Where(p => p.ClientsMembership.Client.Balance < 0).Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.ClientsMembership.Client.Balance).ToListAsync();
                    break;
                case SortPaymentsBy.Debts:
                    if (model.SortReverse)
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.Debts).ToListAsync();
                    else
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.Debts).ToListAsync();
                    break;
                default:
                    break;
            }
            return model;
        }
        private IQueryable<Payment> GetFilteredByDate(DateTime from, DateTime to)
        {
            if(from == new DateTime() || to == new DateTime())
            {
                return _db.Payments;
            }
            else
            {
                return _db.Payments.Where(p => p.CateringDate >= from && p.CateringDate <= to).AsQueryable();
            }
        }
        
        [Breadcrumb("Касса", FromAction = "Index", FromController = typeof(ChiefController))]
        public async Task<IActionResult> Index(PaymentsIndexModelView model, int pageTo = 1)
        {
            var payments =
                _db.Payments;
            if (payments.Count() > pageTo * model.PaymentsLength)
                model.IsNextPage = true;
            if (String.IsNullOrEmpty(model.FilterByName) 
                && model.SortSelect == SortPaymentsBy.None 
                && (model.From == new DateTime() || model.To == new DateTime()))
            {
                model = new PaymentsIndexModelView()
                {
                    Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.CateringDate).ToListAsync()
                };
            }
            else
            {
                if (model.SortSelect != SortPaymentsBy.None)
                    model = await SortPayments(model, pageTo);
                else
                {
                    if (model.SicknessId != null)
                    {
                        Sickness sickness = await _db.Sicknesses.FindAsync(model.SicknessId);
                        if (sickness is null)
                            return NotFound();
                        model.Payments = await GetFilteredByDate(model.From, model.To)
                            .Where(p => p.ClientsMembership.Client.NameSurname.Contains(model.FilterByName) 
                                && p.ClientsMembership.Client.SicknessId == sickness.Id)
                            .Skip((pageTo - 1) * model.PaymentsLength).Take(model.PaymentsLength).ToListAsync();
                    }
                    model.Payments = await GetFilteredByDate(model.From, model.To).Where(p => p.ClientsMembership.Client.NameSurname.Contains(model.FilterByName))
                        .Skip((pageTo - 1) * model.PaymentsLength).Take(model.PaymentsLength).ToListAsync();
                }
            }
            if (User.IsInRole("admin"))
            {
                Employee employee = await _userManager.GetUserAsync(User);
                model.Payments = model.Payments.Where(p => p.Branch.AdminId == employee.Id).ToList();
            }
            ViewBag.Branches = await _db.Branches.ToArrayAsync();
            ViewBag.Sicknesses = await _db.Sicknesses.ToArrayAsync();
            model.SetAmount();
            return View(model);
        }
        
        [Authorize]
        public async Task<IActionResult> GetCreateModalAjax(long clientId)
        {
            Client client = await _db.Clients.FindAsync(clientId);
            Employee emp = await _userManager.GetUserAsync(User);
           Branch branch = _db.Branches.FirstOrDefault(p => p.Admin.Id == emp.Id);
            if (client is null)
                return NotFound();
            PaymentCreateModelView model = new PaymentCreateModelView { ClientId = clientId, Client = client, BranchId= branch.Id};
            
           
            return PartialView("PartialViews/CreatePartial", model);
        }

        public int GetAxisYStep(int max)
        {
            if (max % 15 != 0)
                return max / 10;
            return max / 15;
        }
        public int GetMaxYValue(int maxSum)
        {
            string numStr = maxSum.ToString();
            if (numStr.Length == 1)
                return 10;
            decimal num = (decimal)maxSum;
            int multiplier = 1;
            for (int i = 0; i < numStr.Length - 1; i++)
            {
                num *= (decimal)0.1;
                multiplier *= 10;
            }
            if (numStr.Length < 6 && Convert.ToInt32(numStr[1]) < 5)
            {
                return (int)Math.Ceiling(num * 10) * (int)(multiplier * 0.1);
            }
            return (int)Math.Ceiling(num) * multiplier;
        }
        public LineChartModelView SetChartParams(dynamic sums)
        {
            int max = 0;
            foreach (var item in sums)
            {
                if (item.sum > max)
                    max = item.sum;
            }
            LineChartModelView model = new LineChartModelView();
            model.Data = sums;
            model.MaxY = GetMaxYValue(max);
            model.Step = GetAxisYStep(model.MaxY);
            return model;
        }
        public IActionResult GetPaymentsBarChartAjax(long? branchId, DateTime fromDate, DateTime toDate)
        {
            object sums;
            if (branchId is null)
            {
                sums = from p in _db.Payments
                       where p.CateringDate >= fromDate && p.CateringDate <= toDate
                       group p by new DateTime(p.CateringDate.Year, p.CateringDate.Month, p.CateringDate.Day) into clientsSums
                       select new BarChartSelector
                       {
                           Date = clientsSums.Key,
                           Sum = clientsSums.Sum(p => p.CardSum + p.CashSum)
                       };
            }
            else
            {
                if (!_db.Branches.Any(p => p.Id == branchId))
                    return NotFound();
                sums = from p in _db.Payments
                       where p.CateringDate >= fromDate && p.CateringDate <= toDate && p.ClientsMembership.Client.Group.BranchId == branchId
                       group p by new DateTime(p.CateringDate.Year, p.CateringDate.Month, p.CateringDate.Day) into clientsSums
                       select new BarChartSelector
                       {
                           Date = clientsSums.Key,
                           Sum = clientsSums.Sum(p => p.CardSum + p.CashSum)
                       };
            }
            BarChartModelView model = SetBarChartQuery((sums as IEnumerable<BarChartSelector>).ToArray());
            if (model.Columns.Length == 0)
                return Json(false);
            model.From = fromDate;
            model.To = toDate;
            int max = 0;
            foreach (var item in model.Columns)
            {
                if (item.Sum > max)
                    max = item.Sum;
            }
            model.MaxY = GetMaxYValue(max);
            model.Step = GetAxisYStep(model.MaxY);
            return PartialView("PartialViews/BarChartPartial", model);
        }
        private BarChartColumn[] SetColumnsByWeek(BarChartSelector[] sums)
        {
            int days = (sums[sums.Length - 1].Date - sums[0].Date).Days;
            int weeks = (int)Math.Ceiling((double)days / 7);
            List<BarChartColumn> columns = new List<BarChartColumn> { };
            IEnumerable<int> moneys = from s in sums
                                      group s by (s.Date - sums[0].Date).Days / 7 into sumQuery
                                      select sumQuery.Sum(p => p.Sum);
            var moneysQuery = moneys.ToArray();
            DateTime firstDate = sums[0].Date;
            for (int i = 1; i <= weeks; i++)
            {

                columns.Add(new BarChartColumn()
                {
                    Sum = moneysQuery[i],
                    Date = $"{firstDate.AddDays((double)i * 7).Day} {firstDate.DayOfWeek.ToString()} - {firstDate.AddDays(6).Day} {firstDate.DayOfWeek.ToString()}"
                });
            }
            return columns.ToArray();
        }
        private BarChartModelView SetBarChartQuery(BarChartSelector[] sums)
        {
            if (sums.Length == 0)
                return new BarChartModelView { Columns = new BarChartColumn[0] { } };
            if (sums.Length > 7)
            {
                if (sums.Length > 65)
                {
                    var query = from s in sums
                                group s by s.Date.Month into monthSums
                                select new BarChartColumn()
                                {
                                    Date = months[monthSums.Key],
                                    Sum = monthSums.Sum(p => p.Sum)
                                };
                    return new BarChartModelView() { Columns = query.ToArray() };
                }
                return new BarChartModelView() { Columns = SetColumnsByWeek(sums) };
            }
            List<BarChartColumn> columns = new List<BarChartColumn> { };
            for (int i = 0; i < sums.Length; i++)
            {
                columns.Add(new BarChartColumn() { Date = sums[i].Date.ToShortDateString(), Sum = sums[i].Sum });
            }
            return new BarChartModelView { Columns = columns.ToArray() };
        }
        public IActionResult GetNewClientsChartAjax(long? branchId)
        {
            dynamic sums;
            if (branchId is null)
            {
                sums = from p in _db.Clients
                       where p.DateCreate.Year == DateTime.Now.Year
                       group p by p.DateCreate.Month into clientsSums
                       select new
                       {
                           month = clientsSums.Key,
                           sum = clientsSums.Sum(p => 1)
                       };
            }
            else
            {
                if (!_db.Branches.Any(p => p.Id == branchId))
                    return NotFound();
                sums = from p in _db.Clients
                       where p.DateCreate.Year == DateTime.Now.Year && p.Group.BranchId == branchId
                       group p by p.DateCreate.Month into clientsSums
                       select new
                       {
                           month = clientsSums.Key,
                           sum = clientsSums.Sum(p => 1)
                       };
            }
            return PartialView("PartialViews/PaymentsSumChart", SetChartParams(sums));
        }

        public IActionResult GetNewPaymentsChartAjax(long? branchId)
        {
            dynamic sums;
            if (branchId is null)
            {
                sums = from p in _db.Payments
                       where p.ClientsMembership.Client.DateCreate.Year == DateTime.Now.Year
                       && p.ClientsMembership.Client.DateCreate.Month == p.CateringDate.Month
                       group p by p.CateringDate.Month into paymentsSums
                       select new
                       {
                           month = paymentsSums.Key,
                           sum = paymentsSums.Sum(p => p.CardSum + p.CashSum)
                       };
            }
            else
            {
                if (!_db.Branches.Any(p => p.Id == branchId))
                    return NotFound();
                sums = from p in _db.Payments
                       where p.ClientsMembership.Client.DateCreate.Year == DateTime.Now.Year
                       && p.ClientsMembership.Client.DateCreate.Month == p.CateringDate.Month
                       && p.ClientsMembership.Client.Group.BranchId == branchId
                       group p by p.CateringDate.Month into paymentsSums
                       select new
                       {
                           month = paymentsSums.Key,
                           sum = paymentsSums.Sum(p => p.CardSum + p.CashSum)
                       };
            }
            return PartialView("PartialViews/PaymentsSumChart", SetChartParams(sums));
        }
        public IActionResult GetPaymentsChartAjax(long? branchId)
        {
            dynamic sums;
            if (branchId is null)
            {
                sums = from p in _db.Payments
                       where p.CateringDate.Year == DateTime.Now.Year
                       group p by p.CateringDate.Month into paymentsSums
                       select new
                       {
                           month = paymentsSums.Key,
                           sum = paymentsSums.Sum(p => p.CardSum + p.CashSum)
                       };
            }
            else
            {
                if (!_db.Branches.Any(p => p.Id == branchId))
                    return NotFound();
                sums = from p in _db.Payments
                       where p.CateringDate.Year == DateTime.Now.Year && p.ClientsMembership.Client.Group.BranchId == branchId
                       group p by p.CateringDate.Month into paymentsSums
                       select new
                       {
                           month = paymentsSums.Key,
                           sum = paymentsSums.Sum(p => p.CardSum + p.CashSum)
                       };
            }
            return PartialView("PartialViews/PaymentsSumChart", SetChartParams(sums));
        }
        public async Task<ClientsMembership> GetClientMembership(long clientId, long membershipId)
        {
            ClientsMembership[] clientsMemberships = await _db.ClientsMemberships.ToArrayAsync();
            for (int i = clientsMemberships.Length - 1; i >= 0; i--)
            {
                if (clientsMemberships[i].ClientId == clientId && clientsMemberships[i].MembershipId == membershipId)
                    return clientsMemberships[i];
            }
            return null;
        }
        
        [Authorize]
        [HttpPost]
        //Создание оплаты
        public async Task<IActionResult> CreateAjax(PaymentCreateModelView model)
        {
           
                Client client = await _db.Clients.FirstOrDefaultAsync(p => p.Id == model.ClientId);
                model.BranchId = client.Group.BranchId;
                ClientsMembership clientsMembership = await GetClientMembership(client.Id, (long)client.MembershipId);
                
                if (clientsMembership is null)
                    return BadRequest();
                Employee employee = await _userManager.GetUserAsync(User);
               
                bool check = await _paymentsService.CreatePayment(model, clientsMembership, client, employee.Id);
                if (!check)
                    return BadRequest();
                return Json(true);
        }

        [Authorize(Roles = "chief")]
        public async Task<IActionResult> GetEditModalAjax(long paymentId)
        {
            Payment payment = await _db.Payments.FindAsync(paymentId);
            if (payment is null)
                return NotFound();
            PaymentEditModelView model = new PaymentEditModelView { 
                PaymentId = paymentId,
                Comment = payment.Comment,
                CashSum = payment.CashSum,
                CardSum = payment.CardSum,
                Payment = payment
            };
            ViewBag.Memberships = await _db.Memberships.ToArrayAsync();
            return PartialView("PartialViews/EditPartial", model);
        }
        [Authorize(Roles = "chief")]
        public async Task<IActionResult> EditAjax(PaymentEditModelView model)
        {
            if (ModelState.IsValid)
            {
                bool check = await _paymentsService.EditPayment(model);
                if (!check)
                    BadRequest();
                return Json(true);
            }
            return BadRequest();
        }
        
      
        
        
        
    }
}