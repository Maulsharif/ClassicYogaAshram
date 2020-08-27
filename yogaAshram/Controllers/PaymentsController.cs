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
using yogaAshram.Services;

namespace yogaAshram.Controllers
{
    public class PaymentsController : Controller
    {
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
            var payments = GetFilteredByDate(model.ByDate);
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
            var payments =
                _db.Payments;
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
                    model.Payments = await GetFilteredByDate(model.ByDate).Where(p => p.ClientsMembership.Client.NameSurname.Contains(model.FilterByName))
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
        public async Task<ClientsMembership> GetClientMembership(long clientId, long membershipId)
        {
            ClientsMembership[] clientsMemberships = await _db.ClientsMemberships.ToArrayAsync();
            for (int i = clientsMemberships.Length - 1; i >= 0; i++)
            {
                if (clientsMemberships[i].ClientId == clientId && clientsMemberships[0].MembershipId == membershipId)
                    return clientsMemberships[i];
            }
            return null;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAjax(PaymentCreateModelView model)
        {
            if (ModelState.IsValid)
            {
                Client client = await _db.Clients.FirstOrDefaultAsync(p => p.Id == model.ClientId);
                ClientsMembership clientsMembership = await GetClientMembership(client.Id, (long)client.MembershipId);
                if (clientsMembership is null)
                    return BadRequest();
                Employee employee = await _userManager.GetUserAsync(User);
                bool check = await _paymentsService.CreatePayment(model, clientsMembership, client, employee.Id);
                if (!check)
                    return BadRequest();
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
                bool check = await _paymentsService.EditPayment(model);
                if (!check)
                    BadRequest();
                return Json(true);
            }
            return BadRequest();
        }
    }
}