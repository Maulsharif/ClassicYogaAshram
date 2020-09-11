using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;
using yogaAshram.Services;
using State = yogaAshram.Models.State;

namespace yogaAshram.Controllers
{
    
    public class ClientsController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly YogaAshramContext _db;
        private readonly ClientServices _clientServices;

        public ClientsController(UserManager<Employee> userManager, SignInManager<Employee> signInManager,
            YogaAshramContext db, ClientServices clientServices, PaymentsService paymentsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _clientServices = clientServices;
        }
        
        public IActionResult ClientСabinet(long clientId)
        {
            var count = _db.AttendanceCounts.ToList();
            ViewBag.AbsenceCount = count[^1];
            
            ViewBag.Sicknesses = _db.Sicknesses.ToList();
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            return View(client);
        }
        
        [Breadcrumb("Информация по клиентам", FromAction = "Index", FromController = typeof(ChiefController))]
        public IActionResult Index(int? membershipLeftDays, int? pageNumber, int? frozenTimes, double? dateFrozen, double? dateAbsent)
        {
            var clients = _db.Clients
                .Join(_db.Attendances, c => c.Id, a => a.ClientId, 
                    (client, attendance) => new ClientTableViewModel() { Client = client, Attendance = attendance })
                .ToList();
            clients = clients
                .Join(_db.ClientsMemberships, c => c.Client.Id, m => m.ClientId,
                    ((viewModel, membership) => new ClientTableViewModel()
                        {Client = viewModel.Client, Attendance = viewModel.Attendance, ClientsMembership = membership}))
                .ToList();
            clients = clients
                .GroupBy(p => p.Client.Id)
                .Select(g => g.Last()).ToList();

            int pageSize = 5;
            var model = clients.AsQueryable();

            if (membershipLeftDays != null)
            {
                ViewData["CurrentSort"] = membershipLeftDays;
                model = model
                    .Where(m => DateTime.Now <= m.ClientsMembership.DateOfExpiry 
                                && DateTime.Now.AddDays((double) membershipLeftDays) >= m.ClientsMembership.DateOfExpiry);
            }
            if (dateFrozen != null)
            {
                model = model
                    .Where(p => p.ClientsMembership.Client.Comments
                        .Any(x => p.Client.Id == x.Id && x.Reason == Reason.Заморозка && x.Date >= DateTime.Today.AddDays((double) dateFrozen)) );
            }
            if (dateAbsent != null)
            {
                model = model
                    .Where(p => p.ClientsMembership.Client.Comments
                        .Any(x => p.Client.Id == x.Id && x.Reason == Reason.Пропуск && x.Date >= DateTime.Today.AddDays((double) dateAbsent)) );
            }
            if (frozenTimes != null)
            {
                 model = model.Where(c => c.Attendance.AttendanceCount.FrozenTimes == frozenTimes);
            }
            return View(PageViewModel<ClientTableViewModel>.Create(model.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateClients(Schedule schedule)
        {
            
            Client client = new Client()
            {
                NameSurname = schedule.ClientsCreateModelView.NameSurname,
                PhoneNumber = schedule.ClientsCreateModelView.PhoneNumber,
                DateOfBirth = schedule.ClientsCreateModelView.DateOfBirth,
                Email = schedule.ClientsCreateModelView.Email,
                Address = schedule.ClientsCreateModelView.Address,
                WorkPlace = schedule.ClientsCreateModelView.WorkPlace,
                Sickness = schedule.ClientsCreateModelView.Sickness,
                Source = schedule.ClientsCreateModelView.Source,
                GroupId = schedule.ClientsCreateModelView.GroupId,
                ClientType = ClientType.Probe,
                LessonNumbers = schedule.ClientsCreateModelView.LessonNumbers,
                CreatorId = GetUserId.GetCurrentUserId(this.HttpContext)
            };
            
            
            Employee employee =
                _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
            if (schedule.ClientsCreateModelView.Comment != null)
                client.Comments = new List<Comment>
                {
                    new Comment()
                    {
                        Text = $"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}",
                        Reason = Reason.Другое,
                        Client = client
                    }
                };
            _db.Entry(client).State = EntityState.Added;
            await _db.SaveChangesAsync();
            long ClientId = client.Id;
            if (schedule.ClientsCreateModelView.LessonNumbers == 3)
            {
                TrialUsers trialUsers = new TrialUsers
                {
                    GroupId = schedule.ClientsCreateModelView.GroupId,
                    ClientId = ClientId,
                    State = State.willAttend,
                    Color = "grey",
                    LessonTime = schedule.ClientsCreateModelView.StartDate
                };
                _db.Entry(trialUsers).State = EntityState.Added;
                List<DateTime> dates = _clientServices.TwoTimesTrial(schedule.ClientsCreateModelView.GroupId,
                    schedule.ClientsCreateModelView.StartDate);

                for (int i = 0; i < 2; i++)
                {
                    trialUsers = new TrialUsers
                    {
                        GroupId = schedule.ClientsCreateModelView.GroupId,
                        ClientId = ClientId,
                        State = State.willAttend,
                        Color = "grey",
                        LessonTime = dates[i]
                    };
                    _db.Entry(trialUsers).State = EntityState.Added;
                }
            }
            else
            {
                TrialUsers trialUsers = new TrialUsers
                {
                    GroupId = schedule.ClientsCreateModelView.GroupId,
                    ClientId = ClientId,
                    State = State.willAttend,
                    Color = "grey",
                    LessonTime = schedule.ClientsCreateModelView.StartDate
                };
                _db.Entry(trialUsers).State = EntityState.Added;
            }


            await _db.SaveChangesAsync();
            return RedirectToAction("Trials", "Clients", new {branchId = schedule.BranchId});
        }



        [Breadcrumb("Пробники", FromAction = "Index", FromController = typeof(AdminController))]
        public IActionResult Trials(DateTime time, long branchId)
        {
            ViewBag.BranchId = branchId;
            if (time < DateTime.Today)
                return View(_db.TrialUserses.Where(p => p.Group.BranchId == branchId).OrderBy(p => p.LessonTime)
                    .ToList());
            List<TrialUsers> users = _db.TrialUserses.Where(p => p.LessonTime.Date == time.Date).ToList();

            return View(users.Where(p => p.Group.BranchId == branchId).ToList());
        }
        [Breadcrumb("Посещаемость пробников")]
        [HttpGet]
        public IActionResult CheckAttendanceTrial(long groupId, long clientId, long branchId)
        {
            var childNode1 = new MvcBreadcrumbNode("Index", "Admin", "Личный кабинет администратора");
            var childNode2 = new MvcBreadcrumbNode("Trials", "Clients", "Пробники")
            {
                OverwriteTitleOnExactMatch = true,
                Parent = childNode1,
                RouteValues = new {branchId = branchId}
            };
            var childNode3 = new MvcBreadcrumbNode("CheckAttendanceTrial", "Admin", "Посещаемость пробников")
            {
                OverwriteTitleOnExactMatch = true,
                Parent = childNode2,
            };
            ViewData["BreadcrumbNode"] = childNode3;
            
            //исправлено 
            TrialUsers user = _db.TrialUserses.FirstOrDefault(u => u.Id == clientId);
            
            List<TrialUsers> clients = _db.TrialUserses
                .Where(p => p.GroupId == groupId && p.LessonTime == user.LessonTime).ToList();

            ViewBag.BranchIdHidden = branchId;
            return View(clients);
        }

        //Отметка пробников
        [HttpPost]
        public async Task<IActionResult> CheckAttendanceTrial(long[] arrayOfCustomerID, int[] arrayOfState,
            long HbranchId)
        {

            List<TrialCheckModel> models = new List<TrialCheckModel>();
            for (int i = 0; i < arrayOfCustomerID.Length; i++)
            {
                models.Add(new TrialCheckModel(arrayOfCustomerID[i], arrayOfState[i]));
            }

            List<TrialUsers> users = _db.TrialUserses.ToList();
            for (int i = 0; i < users.Count; i++)
            {
                for (int j = 0; j < models.Count; j++)
                {
                    if (users[i].Id == models[j].Id)
                    {
                        users[i].State = (State) models[j].State;
                        if (users[i].State == State.attended)
                        {
                            // users[i].Client.LessonNumbers -= 1;
                            users[i].Color = "yellow";
                        }
                        else if (users[i].State == State.notattended)
                        {
                            users[i].Color = "red";
                        }

                        users[i].IsChecked = true;
                    }
                }

                _db.Entry(users[i]).State = EntityState.Modified;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Trials", "Clients", new {branchId = HbranchId});
        }

        [Breadcrumb("Информация о пробнике")]
        public IActionResult ClientInfo(long Id)
        {
            ViewBag.Lessons = _db.TrialUserses.Where(p => p.ClientId == Id);
            TrialUsers client = _db.TrialUserses.FirstOrDefault(p => p.ClientId == Id);
            var childNode1 = new MvcBreadcrumbNode("Index", "Admin", "Личный кабинет администратора");
            
            var childNode2 = new MvcBreadcrumbNode("Trials", "Clients", "Пробники")
            {
                OverwriteTitleOnExactMatch = true,
                Parent = childNode1,
                RouteValues = new {branchId = client?.Group.BranchId}
            };
            var childNode3 = new MvcBreadcrumbNode("ClientInfo", "Admin", "Информация о пробнике")
            {
                OverwriteTitleOnExactMatch = true,
                Parent = childNode2,
            };
            ViewData["BreadcrumbNode"] = childNode3;
            return View(client);

        }

        [HttpPost]
        public async Task<IActionResult> NewClientRegister(Schedule schedule, string newSikness)
        {
            long _sicknessId;
            if (ModelState.IsValid)
            {
                if (newSikness != null)
                    _sicknessId=AddSickness(newSikness);
                
                _sicknessId = schedule.ClientsCreateModelView.SicknessId;
                Client client = new Client()
                {
                    NameSurname = schedule.ClientsCreateModelView.NameSurname,
                    PhoneNumber = schedule.ClientsCreateModelView.PhoneNumber,
                    DateOfBirth = schedule.ClientsCreateModelView.DateOfBirth,
                    Email = schedule.ClientsCreateModelView.Email,
                    Address = schedule.ClientsCreateModelView.Address,
                    WorkPlace = schedule.ClientsCreateModelView.WorkPlace,
                    SicknessId = _sicknessId,
                    Source = schedule.ClientsCreateModelView.Source,
                    Paid = Paid.Есть_долг,
                    WhatsAppGroup = WhatsAppGroup.Не_состоит_в_группе,
                    Contract = Contract.Нет_договора,
                    GroupId = schedule.ClientsCreateModelView.GroupId,
                    MembershipId = schedule.ClientsCreateModelView.MembershipId,
                    ClientType = ClientType.AreEngaged,
                    CreatorId = GetUserId.GetCurrentUserId(this.HttpContext)
                };
                Employee employee =
                    _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
                if (schedule.ClientsCreateModelView.Comment != null)
                    client.Comments = new List<Comment>
                    {
                        new Comment()
                        {
                            Text = $"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}",
                            Reason = Reason.Другое,
                            Client = client
                        }
                    };
                    
               
                Group group = _db.Groups.FirstOrDefault(g => g.Id == schedule.ClientsCreateModelView.GroupId);
                if (group != null && group.Clients.Count == 0)
                    group.Clients = new List<Client>()
                    {
                        client
                    };
                else
                    group?.Clients.Add(client);

                Membership membership =
                    _db.Memberships.FirstOrDefault(m => m.Id == schedule.ClientsCreateModelView.MembershipId);
                DateTime endDate = _clientServices.EndDateForClientsMembership(
                    schedule.ClientsCreateModelView.StartDate, 
                    schedule.ClientsCreateModelView.GroupId,
                    membership.AttendanceDays);
                ClientsMembership clientsMembership = new ClientsMembership()
                {
                    Client = client,
                    MembershipId = membership.Id,
                    DateOfPurchase = DateTime.Now,
                    DateOfExpiry = endDate
                };
                
                _db.Entry(clientsMembership).State = EntityState.Added;
                int daysFrozen = 0;
                if (membership.AttendanceDays == 12)
                    daysFrozen = 3;
                else if (membership.AttendanceDays == 8)
                    daysFrozen = 2;
                else
                    daysFrozen = 0;
                var datesOfAttendance = _clientServices.DatesForAttendance(
                    schedule.ClientsCreateModelView.StartDate, schedule.ClientsCreateModelView.GroupId,
                    membership.AttendanceDays + daysFrozen);
                AttendanceCount attendanceCount = new AttendanceCount()
                {
                    AttendingTimes = membership.AttendanceDays,
                    AbsenceTimes = 0,
                    FrozenTimes = daysFrozen
                    
                };
                _db.Entry(attendanceCount).State = EntityState.Added;
                for (int i = 0; i < membership?.AttendanceDays + daysFrozen; i++)
                {
                    Attendance attendance = new Attendance()
                    {
                        Client = client,
                        MembershipId = membership.Id,
                        Date = datesOfAttendance[i],
                        AttendanceState = AttendanceState.notcheked,
                        GroupId = schedule.ClientsCreateModelView.GroupId,
                        AttendanceCount = attendanceCount,
                        ClientsMembership = clientsMembership
                    };
                    _db.Entry(attendance).State = EntityState.Added;
                }
                client.Balance -= membership.Price;
                _db.Entry(client).State = EntityState.Added;
                _db.Entry(group).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("RegularClients", "Clients");
        }

        [HttpPost]
        public async Task<IActionResult> OldClientRegister(Schedule schedule, long clientId)
        {
            if (ModelState.IsValid)
            {
                Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);

                Debug.Assert(client != null, nameof(client) + " != null");
                client.NameSurname = schedule.ClientsCreateModelView.NameSurname;
                client.PhoneNumber = schedule.ClientsCreateModelView.PhoneNumber;
                client.DateOfBirth = schedule.ClientsCreateModelView.DateOfBirth;
                client.Email = schedule.ClientsCreateModelView.Email;
                client.Address = schedule.ClientsCreateModelView.Address;
                client.WorkPlace = schedule.ClientsCreateModelView.WorkPlace;
                client.Sickness = schedule.ClientsCreateModelView.Sickness;
                client.Source = schedule.ClientsCreateModelView.Source;
                Employee employee =
                    _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
                if (client.Comments is null && schedule.ClientsCreateModelView.Comment != null)
                    client.Comments = new List<Comment>
                    {
                        new Comment()
                        {
                            Text = $"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}",
                            Reason = Reason.Другое,
                            Client = client
                        }
                    };
                else
                {
                     client.Comments?.Clear();
                     client.Comments = new List<Comment>
                     {
                         new Comment()
                         {
                             Text = $"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}",
                             Reason = Reason.Другое,
                             Client = client
                         }
                     };
                }
                
                client.Paid = Paid.Не_оплачено;
                client.WhatsAppGroup = WhatsAppGroup.Не_состоит_в_группе;
                client.Contract = Contract.Нет_договора;
                client.GroupId = schedule.ClientsCreateModelView.GroupId;
                client.MembershipId = schedule.ClientsCreateModelView.MembershipId;
                client.ClientType = ClientType.AreEngaged;
                client.CreatorId = GetUserId.GetCurrentUserId(this.HttpContext);
                
                Group group = _db.Groups.FirstOrDefault(g => g.Id == schedule.ClientsCreateModelView.GroupId);
                if (group != null && group.Clients.Count == 0)
                    group.Clients = new List<Client>(){client};
                else
                    group?.Clients.Add(client);

                Membership membership =
                    _db.Memberships.FirstOrDefault(m => m.Id == schedule.ClientsCreateModelView.MembershipId);
                client.Balance -= membership.Price;
                _db.Entry(client).State = EntityState.Modified;
                
                DateTime endDate = _clientServices.EndDateForClientsMembership(
                    schedule.ClientsCreateModelView.StartDate, 
                    schedule.ClientsCreateModelView.GroupId,
                    membership.AttendanceDays);
               ClientsMembership clientsMembership = new ClientsMembership()
               {
                   ClientId = client.Id,
                   MembershipId = membership.Id,
                   DateOfPurchase = DateTime.Now,
                   DateOfExpiry = endDate
               };
               _db.Entry(clientsMembership).State = EntityState.Added;
                
                int daysFrozen = 0;
                if (membership.AttendanceDays == 12)
                    daysFrozen = 3;
                else if (membership.AttendanceDays == 8)
                    daysFrozen = 2;
                else
                    daysFrozen = 0;
                var datesOfAttendance = _clientServices.DatesForAttendance(
                    schedule.ClientsCreateModelView.StartDate, schedule.ClientsCreateModelView.GroupId,
                    membership.AttendanceDays + daysFrozen);
                
                AttendanceCount attendanceCount = new AttendanceCount()
                {
                    AttendingTimes = membership.AttendanceDays,
                    AbsenceTimes = 0,
                    FrozenTimes = daysFrozen
                    
                };
                _db.Entry(attendanceCount).State = EntityState.Added;
                for (int i = 0; i < membership?.AttendanceDays + daysFrozen; i++)
                {
                    Attendance attendance = new Attendance()
                    {
                        Client = client,
                        MembershipId = membership.Id,
                        Date = datesOfAttendance[i],
                        AttendanceState = AttendanceState.notcheked,
                        GroupId = schedule.ClientsCreateModelView.GroupId,
                        AttendanceCount = attendanceCount,
                        ClientsMembership = clientsMembership
                    };
                    
                    _db.Entry(attendance).State = EntityState.Added;
                }

                _db.Entry(group).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("RegularClients", "Clients");
        }

        [Authorize]
        [Breadcrumb("Базовые клиенты", FromAction = "Index", FromController = typeof(AdminController))]
        public IActionResult RegularClients()
        {
            List<Client> clients = _db.Clients.Where(c => c.ClientType == ClientType.AreEngaged).ToList();

            return View(clients);
        }

        [HttpPost]
        public async Task<IActionResult> MakeClientUnActive(long? clientId)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client != null) client.ClientType = ClientType.NotEngaged;
            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("RegularClients");
        }

        [HttpPost]
        public async Task<IActionResult> MakeClientJoinInWhatsAppGroup(long? clientId)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client != null) client.WhatsAppGroup = WhatsAppGroup.Состоит_в_группе;
            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("RegularClients");
        }

        [HttpPost]
        public async Task<IActionResult> ClientSignedContract(long? clientId)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client != null) client.Contract = Contract.Есть_договор;
            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("RegularClients");
        }
        
        [HttpPost]
        public async Task<IActionResult> Comment(long? clientId, string comment)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            Employee employee =
                _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
            if (client != null && client.Comments is null)
                client.Comments = new List<Comment>
                {
                    new Comment()
                    {
                        Text = $"{employee?.UserName}: {comment}, {DateTime.Now:dd.MM.yyyy}",
                        Reason = Reason.Другое,
                        ClientId = client.Id
                    }
                };
            else
            {
                Comment cmt = new Comment()
                {
                    Text = $"{employee?.UserName}: {comment}, {DateTime.Now:dd.MM.yyyy}",
                    Reason = Reason.Другое,
                    ClientId = client.Id
                };
                client?.Comments.Add(cmt);
            }
            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("RegularClients");
        }
        
        public IActionResult GetPdfDocument(long? clientId)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            ContractPdfService contractPdf = new ContractPdfService(_db);
            return contractPdf.RenderPdfDocument(client.Id);
        }

        [HttpPost]
        public async Task<IActionResult> RegularAttendance(DateTime date, long clientId, int state, long attendanceId, string reason)
        {
            

            List<Attendance> attendancesToCheckDate = _db.Attendances.Where(a => a.IsChecked 
                                                                                 && a.ClientId == clientId).ToList();
            if (attendancesToCheckDate.Any(a => a.Date == date))
                return Content("errorCheckedAlready");
            
            Attendance attendance = _db.Attendances
                .FirstOrDefault(a => a.Id == attendanceId);
            
            Debug.Assert(attendance != null, nameof(attendance) + " != null");

            attendance.IsChecked = true;
            attendance.AttendanceState = (AttendanceState) state;
            attendance.AttendanceCount.AttendingTimes -= 1;
            if (attendance.AttendanceCount.AttendingTimes == 0)
            {
                foreach (var attendanceDays in _db.Attendances.Where(a => a.ClientId == clientId && a.Date > date))
                {
                    _db.Entry(attendanceDays).State = EntityState.Deleted;
                }
            }
            if (attendance.AttendanceCount.AbsenceTimes < 0)
                return Content("errorAttend");
            
            if (attendance.AttendanceState == AttendanceState.notattended)
                attendance.AttendanceCount.AbsenceTimes += 1;
            else if (attendance.AttendanceState == AttendanceState.frozen && attendance.AttendanceCount.FrozenTimes > 0)
            {
                attendance.AttendanceCount.FrozenTimes -= 1;
                attendance.AttendanceCount.AttendingTimes += 1;
                ClientsMembership clientsMembership = _db.ClientsMemberships
                    .FirstOrDefault(c => c.Id == attendance.ClientsMembershipId);
                if (clientsMembership != null)
                {
                    DateTime lastDay = clientsMembership.DateOfExpiry;
                    clientsMembership.DateOfExpiry = _clientServices.DateIfFrozen(lastDay, attendance.GroupId);
                    _db.Entry(clientsMembership).State = EntityState.Modified;
                }
            }
            else if(attendance.AttendanceState == AttendanceState.frozen && attendance.AttendanceCount.FrozenTimes == 0)
                return Content("errorFrozen");
            
            if (reason != null)
            {
                Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
                Comment comment = new Comment();
                comment.ClientId = clientId;
                if (attendance.AttendanceState == AttendanceState.frozen)
                    comment.Reason = Reason.Заморозка;
                else
                    comment.Reason = Reason.Пропуск;
                Employee employee =
                    _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
                comment.Text = $"{employee?.UserName}: {reason}, {DateTime.Now:dd.MM.yyyy}";
                    client?.Comments.Add(comment);
                    _db.Entry(client).State = EntityState.Modified;
            }

            _db.Entry(attendance).State = EntityState.Modified;
            
            await _db.SaveChangesAsync();
            return Content("success");
        }
        [HttpPost]
        public async Task<IActionResult> CommentFromAttendance(long clientId, string comment)
        {
            Employee employee =
                _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            
            if (client != null && client.Comments is null)
                client.Comments = new List<Comment>
                {
                    new Comment()
                    {
                        Text = $"{employee?.UserName}: {comment}, {DateTime.Now:dd.MM.yyyy}",
                        Reason = Reason.Другое,
                        Client = client
                    }
                };
            else
            {
                Comment cmt = new Comment()
                {
                    Text = $"{employee?.UserName}: {comment}, {DateTime.Now:dd.MM.yyyy}",
                    Reason = Reason.Другое,
                    Client = client
                };
                client?.Comments.Add(cmt);
            }
                
            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Content("success");
        }

        [HttpPost]
        public long AddSickness(string sicknessName)
        {
            try
            {
                Sickness sickness = new Sickness()
                {
                    Name = sicknessName
                };

                _db.Entry(sickness).State = EntityState.Added;
                return sickness.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public bool GetGroupsLink()
        {
            bool res;
            if (_db.Attendances.Where(p => p.IsChecked == false && p.Date == DateTime.Today).ToList().Count > 0)
                return true;
            else
            {
                return false;
            }

            
        }
        
        
        
        [Authorize]
        public IActionResult ClientEdit(long id)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == id); 
            ClientsEditModelView model = new ClientsEditModelView()
            {
                PhoneNumber = client.PhoneNumber,
                NameSurname = client.NameSurname,
                Email = client.Email,
                DateOfBirth = client.DateOfBirth,
                Source = client.Source, 
                WorkPlace = client.WorkPlace,
                Sickness = client.Sickness,
            };
            return View(model);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ClientEdit(ClientsEditModelView model)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == model.Id);
            if (ModelState.IsValid)
            {
                if (client != null)
                {
                    client.PhoneNumber = model.PhoneNumber;
                    client.NameSurname = model.NameSurname;
                    client.Email = model.Email;
                    client.DateOfBirth = model.DateOfBirth;
                    client.Source = model.Source;
                    client.WorkPlace = model.WorkPlace;
                    client.Sickness = model.Sickness;
                    
                    _db.Entry(client).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    
                    return RedirectToAction("ClientСabinet", new {clientId = client.Id});
                }
            }
            return RedirectToAction("ClientСabinet", new {clientId = client.Id});
        }
    }
}