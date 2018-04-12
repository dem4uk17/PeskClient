using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            PleskClient.PleskClient client = new PleskClient.PleskClient("192.168.68.129", "admin", "1234Qwer");
            //var a = client.CreateWebspace("domnametest.com", "192.168.68.129");
            //var a = client.GetWebspaces("test.net");
            //var b = client.GetDomain("domnametest.com");
            //var c = client.CreateMailAccount(6 ,"admintest", "1234Qwer", 1000000);
            //var a = client.UpdateMailAccount(6, "admintest", "1234Qwer", 20000000);
            //var a = client.GetMailAccounts(6);

            client.

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
