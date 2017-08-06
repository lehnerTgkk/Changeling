using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Changeling
{
    class mailer
    {
      public mailer()
       {
       }
        public void smtp_mailer(string address, string password)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(address, password),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                client.Send(address, address, "test", "testbody");
                Console.WriteLine("Sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
