using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Telegram.Bot;
using yogaAshram.Workers;
using Message = Telegram.Bot.Types.Message;

namespace yogaAshram.Models
{
   
    public class Bot:IBot
    { 
        private readonly TelegramBotClient _bot;

     
   
        public Bot()
        {
            _bot = new TelegramBotClient("1075772843:AAGgxPk2l_FG6EDEbNtn4pEDipx2vKHVVUI");
        }
       
      
        public void StartBot()
        {
            string mes = "";
            // if (clients.Count > 0)
            // {
            //     foreach (var client in clients)
            //     {
            //         mes += client.NameSurname + " " + client.Group.Name + "/n";
            //     }
            // }

            mes = "Все клиенты отмечены";
            _bot.SendTextMessageAsync("-1001160591311",mes);
            _bot.StartReceiving();
            while (true)
            {
                Console.WriteLine("Bot is worked all right");
                Thread.Sleep(int.MaxValue);
            }
        }

        private  async void OnMessageReceived(object? sender, Telegram.Bot.Args.MessageEventArgs messageEventArgs)
        {
            try
            {
                Message message = messageEventArgs.Message;
                string mes = "Надира Маратова";
                Console.WriteLine(message.Chat.Id);
                await _bot.SendTextMessageAsync(message.Chat.Id, mes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
   
}