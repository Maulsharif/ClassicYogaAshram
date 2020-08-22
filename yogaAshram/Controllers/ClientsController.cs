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
            YogaAshramContext db, ClientServices clientServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _clientServices = clientServices;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateClients(Schedule schedule)
        {
            Client client = new Client
            {
                NameSurname = schedule.ClientsCreateModelView.NameSurname,
                PhoneNumber = schedule.ClientsCreateModelView.PhoneNumber,
                ClientType = schedule.ClientsCreateModelView.ClientType,
                GroupId = schedule.ClientsCreateModelView.GroupId,
                LessonNumbers = schedule.ClientsCreateModelView.LessonNumbers,
                CreatorId = GetUserId.GetCurrentUserId(this.HttpContext)
            };
            Employee employee =
                _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
            if (schedule.ClientsCreateModelView.Comment != null)
                client.Comments = new List<string>
                    {$"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}"};
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




        public IActionResult Trials(DateTime time, long branchId)
        {
            ViewBag.BranchId = branchId;
            if (time < DateTime.Today)
                return View(_db.TrialUserses.Where(p => p.Group.BranchId == branchId).OrderBy(p => p.LessonTime)
                    .ToList());
            List<TrialUsers> users = _db.TrialUserses.Where(p => p.LessonTime.Date == time.Date).ToList();

            return View(users.Where(p => p.Group.BranchId == branchId).ToList());
        }

        [HttpGet]
        public IActionResult CheckAttendanceTrial(long groupId, long clientId, long branchId)
        {

            TrialUsers user = _db.TrialUserses.FirstOrDefault(u => u.ClientId == clientId);


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


        public IActionResult ClientInfo(long Id)
        {
            ViewBag.Lessons = _db.TrialUserses.Where(p => p.ClientId == Id);
            TrialUsers client = _db.TrialUserses.FirstOrDefault(p => p.ClientId == Id);
            return View(client);

        }

        [HttpPost]
        public async Task<IActionResult> NewClientRegister(Schedule schedule)
        {
            if (ModelState.IsValid)
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
                    Paid = Paid.Не_оплачено,
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
                    client.Comments = new List<string>
                        {$"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}"};
                _db.Entry(client).State = EntityState.Added;
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

                var datesOfAttendance = _clientServices.DatesForAttendance(
                    schedule.ClientsCreateModelView.StartDate, schedule.ClientsCreateModelView.GroupId,
                    membership.AttendanceDays + 3);
                AttendanceCount attendanceCount = new AttendanceCount()
                {
                    AttendingTimes = membership.AttendanceDays,
                    AbsenceTimes = 0
                };
                _db.Entry(attendanceCount).State = EntityState.Added;
                for (int i = 0; i < membership?.AttendanceDays + 3; i++)
                {
                    Attendance attendance = new Attendance()
                    {
                        Client = client,
                        MembershipId = membership.Id,
                        Date = datesOfAttendance[i],
                        AttendanceState = AttendanceState.notcheked,
                        GroupId = schedule.ClientsCreateModelView.GroupId,
                        AttendanceCount = attendanceCount
                    };
                    _db.Entry(attendance).State = EntityState.Added;
                }

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
                if (client.Comments.Count == 0 && schedule.ClientsCreateModelView.Comment != null)
                    client.Comments = new List<string>
                        {$"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}"};
                else
                    client.Comments.Add(
                        $"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}");
                client.Paid = Paid.Не_оплачено;
                client.WhatsAppGroup = WhatsAppGroup.Не_состоит_в_группе;
                client.Contract = Contract.Нет_договора;
                client.GroupId = schedule.ClientsCreateModelView.GroupId;
                client.MembershipId = schedule.ClientsCreateModelView.MembershipId;
                client.ClientType = ClientType.AreEngaged;
                client.CreatorId = GetUserId.GetCurrentUserId(this.HttpContext);

                _db.Entry(client).State = EntityState.Modified;
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

                var datesOfAttendance = _clientServices.DatesForAttendance(
                    schedule.ClientsCreateModelView.StartDate, schedule.ClientsCreateModelView.GroupId,
                    membership.AttendanceDays + 3);
                AttendanceCount attendanceCount = new AttendanceCount()
                {
                    AttendingTimes = membership.AttendanceDays,
                    AbsenceTimes = 0
                };
                _db.Entry(attendanceCount).State = EntityState.Added;
                for (int i = 0; i < membership?.AttendanceDays + 3; i++)
                {
                    Attendance attendance = new Attendance()
                    {
                        Client = client,
                        MembershipId = membership.Id,
                        Date = datesOfAttendance[i],
                        AttendanceState = AttendanceState.notcheked,
                        GroupId = schedule.ClientsCreateModelView.GroupId,
                        AttendanceCount = attendanceCount
                    };
                    
                    _db.Entry(attendance).State = EntityState.Added;
                }

                _db.Entry(group).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("RegularClients", "Clients");
        }

        [Authorize]
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
                client.Comments = new List<string> {$"{employee?.UserName}: {comment}, {DateTime.Now:dd.MM.yyyy}"};
            else
                client?.Comments.Add(
                    $"{employee?.UserName}: {comment}, {DateTime.Now:dd.MM.yyyy}");


            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("RegularClients");
        }

        public IActionResult GetPdfDocument(long? clientId)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            PdfDocument document = new PdfDocument();

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Times New Roman", 13, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);

            XRect rect = new XRect(0, 50, 600, 900);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString(client.ToString(), font, XBrushes.Black, rect, XStringFormats.TopLeft);


            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");
            fileStreamResult.FileDownloadName = $"{client.NameSurname}.pdf";
            return fileStreamResult;
        }

        [HttpPost]
        public async Task<IActionResult> RegularAttendance(DateTime date, long clientId, int state, long attendanceId)
        {

            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            int maxDays = client.Membership.AttendanceDays;
            List<Attendance> attendances = _db.Attendances.Where(a => a.ClientId == clientId).ToList();
            List<Attendance> attendanceToCheckState = _db.Attendances
                .Where(a => a.AttendanceState == AttendanceState.notattended)
                .Where(a => a.ClientId == clientId)
                .ToList();



            List<Attendance> attendancesToCheckDate = _db.Attendances.Where(a => a.IsChecked)
                .Where(a => a.ClientId == clientId).ToList();
            if (attendancesToCheckDate.Any(a => a.Date == date))
                return Content("errorCheckedAlready");


            Attendance attendance = _db.Attendances
                .FirstOrDefault(a => a.Id == attendanceId);
            Console.WriteLine(attendance.ClientId);
            Debug.Assert(attendance != null, nameof(attendance) + " != null");

            attendance.IsChecked = true;
            attendance.AttendanceState = (AttendanceState) state;
            attendance.AttendanceCount.AttendingTimes -= 1;

            if (attendance.AttendanceState == AttendanceState.notattended)
                attendance.AttendanceCount.AbsenceTimes += 1;

            _db.Entry(attendance).State = EntityState.Modified;
            
            await _db.SaveChangesAsync();
            return Content("success");
        }
        [HttpPost]
        public async Task<IActionResult> CommentFromAttendance(long attendanceCountId, string comment)
        {
            Employee employee =
                _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
            AttendanceCount attendanceCount = _db.AttendanceCounts.FirstOrDefault(a => a.Id == attendanceCountId);
            
            if (attendanceCount?.Comments is null)
                attendanceCount.Comments = new List<string> {$"{employee?.UserName}: {comment}, {DateTime.Now:dd.MM.yyyy}"};
            else
                attendanceCount?.Comments.Add(
                    $"{employee?.UserName}: {comment}, {DateTime.Now:dd.MM.yyyy}");
            _db.Entry(attendanceCount).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Content("success");
        }
    }
}