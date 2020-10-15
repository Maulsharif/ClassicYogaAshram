using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using yogaAshram.Models;

namespace yogaAshram.Services
{
    public class ContractPdfService
    {
        private readonly YogaAshramContext _db;
        
        public ContractPdfService(YogaAshramContext db)
        {
            _db = db;
        }
        
        public IActionResult RenderPdfDocument(long clientId)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId && c.Id == clientId);
            PdfDocument document = new PdfDocument();
        
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            double x = 50, y = 100;
            XFont fontH1 = new XFont("Times New Roman", 18, XFontStyle.Bold);
            XFont font = new XFont("Times New Roman", 11, XFontStyle.Bold);
            double ls = 10;
            
            gfx.DrawString("",
                fontH1, XBrushes.Black, x, x);

            if (client != null)
            {
                gfx.DrawString($"Я, {client.NameSurname}",
                    font, XBrushes.Black, x, y);
                gfx.DrawString($"(Ф.И.О. полностью)",
                    font, XBrushes.Black, x + 195, y += 2 * ls);
                y += 20 + 2 * ls;
                gfx.DrawString("настоящим подтверждаю, что с Правилами посещения и условиями абонемента йога- центра",
                    font, XBrushes.Black, x, y);
                y += ls;
                gfx.DrawString("Classical Yoga Ashram ознакомлен и согласен. В дальнейшем иметь претензий не буду.",
                    font, XBrushes.Black, x, y);
                y += 20 + 2 * ls;
                gfx.DrawString("Информация о практикующем:", fontH1, XBrushes.Black, x += 115, y);
                y += 20 + 2 * ls;
                gfx.DrawString($"Место работы и должность: {client.WorkPlace}", font, XBrushes.Black,
                    x -= 125, y);
                y += 5 + 2 * ls;
                gfx.DrawString($"Моб.: {client.PhoneNumber}", font, XBrushes.Black, x, y);
                y += 5 + 2 * ls;
                gfx.DrawString($"Дата рождения: {client.DateOfBirth.ToString("dd MMMM yyyy")}", font,
                    XBrushes.Black, x, y);
                y += 5 + 2 * ls;
                gfx.DrawString($"E-mail: {client.Email}", font, XBrushes.Black, x, y);
                y += 5 + 2 * ls;
                gfx.DrawString($"Наличие заболеваний: {client.Sickness.Name}", font, XBrushes.Black, x, y);
                y += 20 + 2 * ls;
                gfx.DrawString($"Дата: {client.DateCreate.ToString("dd MM yyyy")}", font, XBrushes.Black, x, y);
                gfx.DrawString("Подпись: ", font, XBrushes.Black, x += 300, y);
            }
            else
            {
                y += 20 + 2 * ls;
                gfx.DrawString($"Нет никакого клиента", font, XBrushes.Black, x, y);
            }
            
            
            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");
            fileStreamResult.FileDownloadName = $"{client.NameSurname}.pdf";
            return fileStreamResult;

            
            // string filename = $"Files/contract.pdf";
            // document.Save(filename);
            //
            // var fileStream = new FileStream(filename, 
            //     FileMode.Open,
            //     FileAccess.Read
            // );
            // var fsResult = new FileStreamResult(fileStream, "application/pdf");
            // return fsResult;
            
        }
    }
}