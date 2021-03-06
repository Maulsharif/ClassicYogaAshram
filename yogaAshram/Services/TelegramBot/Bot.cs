﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Telegram.Bot;
using yogaAshram.Services;
using yogaAshram.Workers;
using Message = Telegram.Bot.Types.Message;

namespace yogaAshram.Models
{
   
    public class Bot:IBot
    { 
        private readonly TelegramBotClient _bot;
        private readonly YogaAshramContext _db;
        private readonly string tgToken="1075772843:AAGgxPk2l_FG6EDEbNtn4pEDipx2vKHVVUI";
        private readonly string grId="-1001160591311";
   
        public Bot(YogaAshramContext db)
        {
            _db = db;
            _bot = new TelegramBotClient(tgToken);
        }
       
      
        public void StartBot()
        {
            //делать выборку 
            List<Attendance> clients =
                _db.Attendances.Where(p => p.IsChecked == false && p.Date == DateTime.Today).ToList();
      
            string mes =string.Empty;
            if (clients.Count() > 0)
            {
                
                mes = $"#напоминание Список неотмеченных клиентов {DateTime.Today.ToShortDateString()}:"+"\n";
                foreach (var client in clients)
                {
                    
                    mes += "Филиал: "+ client.Group.Branch.Name + " " + "Группа: "+ client.Group.Name + " " +  client.Client.NameSurname + "\n";
                    Console.WriteLine(mes);
                }
            }
            else
            {
                mes = "Все клиенты отмечены";
            }

           
            _bot.SendTextMessageAsync(grId,mes);
            _bot.StartReceiving();
            while (true)
            {
               Thread.Sleep(int.MaxValue);
            }
        }

        
    }
   
}