using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;

namespace yogaAshram.Quartz
{
    public class DataScheduler
    {
        public static async void Start(IServiceProvider serviceProvider)
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.JobFactory = serviceProvider.GetService<JobFactory>();
            await scheduler.Start();

            IJobDetail jobDetail = JobBuilder.Create<DataJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("MailingTrigger", "default")
                //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(1, 00))
                .StartNow() // if a start time is not given (if this line were omitted), "now" is implied
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(15)
                    .WithRepeatCount(10))
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger);
            
        }
    }
}