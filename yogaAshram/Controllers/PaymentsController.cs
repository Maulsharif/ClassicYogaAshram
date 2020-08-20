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
            switch (model.SortSelect)
            {
                case SortPaymentsBy.None:
                    return model;
                case SortPaymentsBy.Memberships:
                    if (model.SortReverse)
                        model.Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.MembershipId).ToListAsync();
                    else
                        model.Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.MembershipId).ToListAsync();
                    break;
                case SortPaymentsBy.Price:
                    if (model.SortReverse)
                        model.Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => (p.Membership.Price - p.Debts)).ToListAsync();
                    else
                        model.Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => (p.Membership.Price - p.Debts)).ToListAsync();
                    break;
                case SortPaymentsBy.Group:
                    if (model.SortReverse)
                        model.Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.Client.GroupId).ToListAsync();
                    else
                        model.Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.Client.GroupId).ToListAsync();
                    break;
                case SortPaymentsBy.Comment:
                    if (model.SortReverse)
                        model.Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.Comment).ToListAsync();
                    else
                        model.Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.Comment).ToListAsync();
                    break;
                case SortPaymentsBy.Sickness:
                    if (model.SortReverse)
                        model.Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderByDescending(p => p.Client.Sickness).ToListAsync();
                    else
                        model.Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.Client.Sickness).ToListAsync();
                    break;
                default:
                    break;
            }
            return model;
        }
        public async Task<IActionResult> Index(PaymentsIndexModelView model, int pageTo = 1)
        {
            var payments = _db.Payments;
            if (payments.Count() > pageTo * model.PaymentsLength)
                model.IsNextPage = true;
            if (String.IsNullOrEmpty(model.FilterByName) && model.SortSelect == SortPaymentsBy.None)
            {
                model = new PaymentsIndexModelView()
                {
                    Payments = await _db.Payments.Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).OrderBy(p => p.CateringDate).ToListAsync()
                };
            }
            else
            {
                if(model.SortSelect == SortPaymentsBy.None)
                    model = await SortPayments(model, pageTo);
                else 
                    model.Payments = await _db.Payments.Where(p => p.Client.NameSurname.Contains(model.FilterByName))
                        .Skip((pageTo - 1) * model.PaymentsLength)
                            .Take(model.PaymentsLength).ToListAsync();
            }          
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> GetCreateModalAjax(long clientId)
        {
            if (!_db.Clients.Any(p => p.Id == clientId))
                return NotFound();
            PaymentCreateModelView model = new PaymentCreateModelView{ ClientId = clientId };
            ViewBag.Memberships = await _db.Memberships.ToArrayAsync();
            return PartialView("PartialViews/CreatePartial", model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAjax(PaymentCreateModelView model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = await _userManager.GetUserAsync(User);
                Client client = await _db.Clients.FindAsync(model.ClientId);
                Membership membership = await _db.Memberships.FindAsync(model.MembershipId);
                if (membership.Price < model.Debts)
                    return BadRequest();
                Payment payment = new Payment()
                {
                    Comment = model.Comment,
                    MembershipId = model.MembershipId,
                    ClientId = model.ClientId,
                    CreatorId = employee.Id,
                    Debts = model.Debts
                };
                if (model.Debts > 0)
                {
                    client.Paid = Paid.Есть_долг;
                    client.Color = "dark";
                }
                else
                {
                    client.Paid = Paid.Оплачено;
                    client.Color = "";
                }
                _db.Entry(client).State = EntityState.Modified;
                _db.Entry(payment).State = EntityState.Added;
                await _db.SaveChangesAsync();
                return Json(true);
            }
            return BadRequest();
        }
        [Authorize(Roles = "chief")]
        public async Task<IActionResult> GetEditModalAjax(long paymentId)
        {
            Payment payment = await _db.Payments.FindAsync(paymentId);
            if (payment is null)
                return NotFound();
            PaymentEditModelView model = new PaymentEditModelView { PaymentId = paymentId,
                Comment = payment.Comment,
                MembershipId = payment.MembershipId,
                Debts = payment.Debts
            };
            ViewBag.Memberships = await _db.Memberships.ToArrayAsync();
            return PartialView("PartialViews/EditPartial", model);
        }
        [Authorize(Roles = "chief")]
        public async Task<IActionResult> EditAjax(PaymentEditModelView model)
        {
            if (ModelState.IsValid)
            {
                Membership membership = await _db.Memberships.FindAsync(model.MembershipId);
                if (membership.Price < model.Debts)
                    return BadRequest();
                Payment payment = await _db.Payments.FindAsync(model.PaymentId);
                Client client = await _db.Clients.FindAsync(payment.ClientId);
                payment.Comment = model.Comment;
                payment.MembershipId = model.MembershipId;
                payment.Debts = model.Debts;
                payment.LastUpdate = DateTime.Now;
                if (model.Debts > 0)
                {
                    client.Paid = Paid.Есть_долг;
                    client.Color = "dark";
                }
                else
                {
                    client.Paid = Paid.Оплачено;
                    client.Color = "";
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