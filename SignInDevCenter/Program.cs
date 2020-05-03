using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using System.Diagnostics;

namespace SignInDevCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Microsoft!");
            Task.Run(Demo).Wait();
            Console.ReadKey();
        }

        public async static Task Demo()
        {
            try
            {
                SignInDevCenter _signIn = new SignInDevCenter();

                string username = null; //Dev center e-mail address
                string passoword = null;//Dev center passowrd
                
                Debug.Assert(string.IsNullOrEmpty(username));
                Debug.Assert(string.IsNullOrEmpty(passoword));
                
                var cookie = await _signIn.SignIn(username, passoword);
                
                Console.WriteLine(cookie);

                DevCenterService _devCenter = new DevCenterService(cookie);
                var groups = await _devCenter.GetGroups();

                
                foreach (var item in groups)
                {
                    var group = await _devCenter.GetGroupInfo(item.GroupId);
                    item.Members = group.Members;
                    item.LastUpdatedTime = group.LastUpdatedTime;

                    Console.WriteLine(JsonSerializer.Serialize(item));
                    Console.WriteLine();
                }

                var lastGroup = groups.Last();
                Console.WriteLine("Before Updateing:");
                Console.WriteLine(JsonSerializer.Serialize(lastGroup));

                UpdateCustomerGroup updateCustomerGroup = new UpdateCustomerGroup();
                updateCustomerGroup.Name = lastGroup.GroupName;
                updateCustomerGroup.Members.AddRange(lastGroup.Members);
                updateCustomerGroup.Members.Add("testaccount2@cq.com");
                var updatedGroup = await _devCenter.UpdateGroup(lastGroup.GroupId, updateCustomerGroup);

                Console.WriteLine("After Updateing:");
                Console.WriteLine(JsonSerializer.Serialize(updatedGroup));
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception:" + e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
