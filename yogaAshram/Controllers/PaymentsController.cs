using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;

namespace yogaAshram.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly YogaAshramContext _db;
        private readonly UserManager<Employee> _userManager;

        public PaymentsController(YogaAshramContext db, UserManager<Employee> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        private async Task<PaymentsIndexModelView> SortPayments(PaymentsIndexModelView model, int pageTo)
        {
            var payments = GetFilteredByDate(model.ByDate);
            switch (model.SortSelect)
            {
                case SortPaymentsBy.None:
                    return model;
                case SortPaymentsBy.Memberships:
                    if (model.SortReverse)
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.MembershipId).ToListAsync();
                    else
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.MembershipId).ToListAsync();
                    break;
                case SortPaymentsBy.Price:
                    if (model.SortReverse)
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => (p.Membership.Price - p.Debts)).ToListAsync();
                    else
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => (p.Membership.Price - p.Debts)).ToListAsync();
                    break;
                case SortPaymentsBy.Group:
                    if (model.SortReverse)
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.Client.GroupId).ToListAsync();
                    else
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.Client.GroupId).ToListAsync();
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
                            .Take(model.PaymentsLength).OrderByDescending(p => p.Client.Sickness).ToListAsync();
                    else
                        model.Payments = await payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.Client.Sickness).ToListAsync();
                    break;
                case SortPaymentsBy.Debtors:
                    if (model.SortReverse)
                        model.Payments = await payments.Where(p => p.Client.Balance < 0).Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.Client.Balance).ToListAsync();
                    else
                        model.Payments = await payments.Where(p => p.Client.Balance < 0).Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.Client.Balance).ToListAsync();
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
        private IQueryable<Payment> GetFilteredByDate(PaymentsDates paymentsDates)
        {
            DateTime now = DateTime.Now;
            switch (paymentsDates)
            {
                case PaymentsDates.AllTime:
                    return _db.Payments;
                case PaymentsDates.LastMonth:
                    return _db.Payments.Where(p => p.CateringDate >= now.AddMonths(-1) && p.CateringDate <= now);
                case PaymentsDates.LastWeek:
                    return _db.Payments.Where(p => p.CateringDate >= now.AddDays(-7) && p.CateringDate <= now);
                case PaymentsDates.LastDay:
                    return _db.Payments.Where(p => p.CateringDate >= now.AddDays(-1) && p.CateringDate <= now);
                default:
                    return null;
            }
        }
        public async Task<IActionResult> Index(PaymentsIndexModelView model, int pageTo = 1)
        {
            var payments = _db.Payments;
            if (payments.Count() > pageTo * model.PaymentsLength)
                model.IsNextPage = true;
            if (String.IsNullOrEmpty(model.FilterByName) && model.SortSelect == SortPaymentsBy.None && model.ByDate == PaymentsDates.AllTime)
            {
                model = new PaymentsIndexModelView()
                {
                    Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.CateringDate).ToListAsync()
                };
            }
            else
            {
                if(model.SortSelect != SortPaymentsBy.None)
                    model = await SortPayments(model, pageTo);
                else 
                    model.Payments = await GetFilteredByDate(model.ByDate).Where(p => p.Client.NameSurname.Contains(model.FilterByName))
                        .Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).ToListAsync();
            }          
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> GetCreateModalAjax(long clientId)
        {
            Client client = await _db.Clients.FindAsync(clientId);
            if (client is null)
                return NotFound();
            PaymentCreateModelView model = new PaymentCreateModelView { ClientId = clientId, Client = client };
            return PartialView("PartialViews/CreatePartial", model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAjax(PaymentCreateModelView model)
        {
            if (ModelState.IsValid)
            {
                if (model.CashSum is null)
                    model.CashSum = 0;
                if (model.CardSum is null)
                    model.CardSum = 0;
                int sum = (int)model.CashSum + (int)model.CardSum;
                Employee employee = await _userManager.GetUserAsync(User);
                Client client = await _db.Clients.FindAsync(model.ClientId);
                if (client.Balance < 0 && model.Type == PaymentType.Pay)
                    return BadRequest();
                if (client.Membership is null && model.Type == PaymentType.Pay)
                    return BadRequest();
                int balance = client.Balance;
                if (balance < 0)
                    balance = 0;
                Membership membership = await _db.Memberships.FindAsync(model.MembershipId);
                int debts = membership.Price - sum - balance;               
                Payment payment = new Payment()
                {
                    Comment = model.Comment,
                    MembershipId = model.MembershipId,
                    ClientId = model.ClientId,
                    CreatorId = employee.Id,
                    CashSum = (int)model.CashSum,
                    CardSum = (int)model.CardSum,
                    Type = model.Type
                };
                if (debts > 0 && model.Type == PaymentType.Pay)
                {
                    client.Paid = Paid.Есть_долг;
                    client.Color = "dark";
                    client.Balance -= debts;
                    payment.Debts = -client.Balance;
                }
                else
                {
                    if(model.Type == PaymentType.Pay)
                    {
                        client.Paid = Paid.Оплачено;
                        client.Color = "";
                        client.Balance -= debts;
                        payment.Debts = 0;
                    }
                    else
                    {
                        client.Balance += sum;
                        if (client.Balance >= 0)
                        {
                            client.Paid = Paid.Оплачено;
                            client.Color = "";
                            payment.Debts = 0;
                        }
                        else
                            payment.Debts = -client.Balance;
                    }             
                }                              
                _db.Entry(client).State = EntityState.Modified;
                _db.Entry(payment).State = EntityState.Added;
                await _db.SaveChangesAsync();
                return Json(true);
            }
            return BadRequest();
        }
        public async Task<IActionResult> GetSumAjax(PaymentsDates date)
        {
            return Json(await GetFilteredByDate(date).SumAsync(p => p.CashSum + p.CardSum));
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
                if (model.CashSum is null)
                    model.CashSum = 0;
                if (model.CardSum is null)
                    model.CardSum = 0;
                int sum = (int)model.CashSum + (int)model.CardSum;
                Payment payment = await _db.Payments.FindAsync(model.PaymentId);
                int oldSum = payment.CashSum + payment.CardSum;
                Client client = payment.Client;
                if (payment is null)
                    return NotFound();                             
                payment.Comment = model.Comment;
                payment.CashSum = (int)model.CashSum;
                payment.CardSum = (int)model.CardSum;
                payment.LastUpdate = DateTime.Now;
                if (payment.Type == PaymentType.Pay)
                    client.Balance += payment.Debts;
                else
                    client.Balance -= oldSum;
                payment.Type = model.Type;
                int balance = client.Balance;
                if (balance < 0)
                    balance = 0;
                int debts = payment.Membership.Price - sum - balance;
                if (debts > 0 && client.Balance < debts && model.Type == PaymentType.Pay)
                {
                    client.Paid = Paid.Есть_долг;
                    client.Color = "dark";
                    client.Balance -= debts;
                    payment.Debts = -client.Balance;
                }
                else
                {
                    if (model.Type == PaymentType.Pay)
                    {
                        client.Paid = Paid.Оплачено;
                        client.Color = "";
                        client.Balance -= debts;
                    }
                    else
                    {
                        client.Balance += sum;
                        if (client.Balance >= 0)
                        {
                            client.Paid = Paid.Оплачено;
                            client.Color = "";
                            payment.Debts = 0;
                        }
                        else
                            payment.Debts = -client.Balance;
                    }
                }
                _db.Entry(client).State = EntityState.Modified;
                _db.Entry(payment).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return Json(true);
            }
            return BadRequest();
        }
    }
}