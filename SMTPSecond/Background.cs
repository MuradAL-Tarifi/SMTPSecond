﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SMTPSecond.ThirdModels;
using SMTPSecond.Service;
using Microsoft.EntityFrameworkCore;

namespace SMTPSecond
{
    public class Background : IHostedService, IDisposable
    {
        private Timer timer30M;
        private Timer timer1Day;
        private Timer timer60M;

        private readonly AlertTrackerService _processBackground;
        private readonly MailService mailService;
        private readonly TrackerDBContext trackerDBContext;

        public Background(AlertTrackerService processBackground, MailService mailService, TrackerDBContext trackerDBContext)
        {
            _processBackground = processBackground;
            this.mailService = mailService;
            this.trackerDBContext = trackerDBContext;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {

            timer60M = new Timer(async o =>
            {
                await _processBackground.UpdateSmtpsettingsCountAsync();
            },
            null,
            TimeSpan.FromMinutes(0),
            TimeSpan.FromMinutes(65)
            );
            timer1Day = new Timer(async o =>
            {
                var smtpsettings = await _processBackground.GetSmtpsettings();
                if (smtpsettings.Count() != 0)
                {
                    var settingsCount = smtpsettings.Count();

                    var count = 0;
                    var serials = await _processBackground.GetInventoryHistoryNotActive();
                    if (serials.Count != 0)
                    {
                        foreach (var item in serials)
                        {
                            var smtpsetting = smtpsettings[count];

                            var inv = await trackerDBContext.Inventory.Where(x => x.Id == item.InventoryId).FirstOrDefaultAsync();
                            var wr = await trackerDBContext.Warehouse.Where(x => x.Id == item.WarehouseId).FirstOrDefaultAsync();
                            var senior = await trackerDBContext.Sensor.Where(x => x.Serial == item.Serial).FirstOrDefaultAsync();


                            var emails = item.ToEmails.Split(",");
                            if (emails != null)
                            {
                                bool result = false;
                                foreach (var email in emails)
                                {
                                    result = mailService.SendEmailIsNotActive(item.ToEmails, senior.Name, inv.Name, wr.Name, smtpsetting.UserName, smtpsetting.Password, smtpsetting.MailAddress);
                                    if (!result)
                                    {
                                        Console.WriteLine("SMTP ERROR Cant send to this email " + email);
                                    }
                                    else
                                    {
                                        count++;
                                        result = false;
                                        Console.WriteLine("Done Send By " + smtpsetting.UserName);
                                    }
                                }

                            }
                            else
                            {
                                Console.WriteLine("Email should not be Null");
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("No Thing Found Not Active");
                    }
                    if (count == settingsCount)
                    {
                        count = 0;
                    }
                    var serialsLowVoltage = await _processBackground.GetInventoryHistoryIsLowVoltage();
                    if (serialsLowVoltage.Count != 0)
                    {
                        foreach (var item in serialsLowVoltage)
                        {
                            var inv = await trackerDBContext.Inventory.Where(x => x.Id == item.InventoryId).FirstOrDefaultAsync();
                            var wr = await trackerDBContext.Warehouse.Where(x => x.Id == item.WarehouseId).FirstOrDefaultAsync();
                            var senior = await trackerDBContext.Sensor.Where(x => x.Serial == item.Serial).FirstOrDefaultAsync();

                            var smtpsetting = smtpsettings[count];

                            var emails = item.ToEmails.Split(",");
                            if (emails != null)
                            {
                                bool result = false;
                                foreach (var email in emails)
                                {
                                    result = mailService.SendEmailIsLowVoltage(item.ToEmails, senior.Name, inv.Name, wr.Name, smtpsetting.UserName, smtpsetting.Password, smtpsetting.MailAddress);
                                    if (!result)
                                    {
                                        Console.WriteLine("SMTP ERROR Cant send to this email " + email);
                                    }
                                    else
                                    {
                                        count++;
                                        Console.WriteLine("Done Send By " + smtpsetting.UserName);
                                    }
                                }

                            }
                            else
                            {
                                Console.WriteLine("Email should not be Null");
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("No Thing Found Low Voltage");
                    }
                    if (count == settingsCount)
                    {
                        count = 0;
                    }
                }
                else
                {
                    Console.WriteLine("No SMTP Settings Found");
                }
            },
           null,
           TimeSpan.FromMinutes(60),
           TimeSpan.FromDays(1)
           );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("printing worker stoping");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer30M?.Dispose();
            timer1Day?.Dispose();
        }
    }
}
