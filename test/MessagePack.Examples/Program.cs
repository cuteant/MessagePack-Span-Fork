using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;
using MessagePack.Resolvers;

namespace MessagePack.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = new AccessToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresOnDate = DateTime.Now,
                AccountId = 1,
                Scope = new List<string> { "a", "b" }
            };

            var serializeData = MessagePackSerializer.Serialize<object>(token, TypelessDefaultResolver.Instance);
            var json = MessagePackSerializer.ToJson(serializeData);

            Console.WriteLine(json);

            var js = JsonConvert.DeserializeObject(json);
            Console.WriteLine(JsonConvert.SerializeObject(js, Formatting.Indented));
            //Console.WriteLine(JsonConvert.SerializeObject(token, new JsonSerializerSettings
            //{
            //    Formatting = Formatting.Indented,
            //    TypeNameHandling = TypeNameHandling.All,
            //    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            //}));

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }
    }

    //[MessagePackObject]
    public class AccessToken
    {
        //[Key(0)]
        public string Token { get; set; }

        //[Key(1)]
        public DateTime? ExpiresOnDate { get; set; }

        //[Key(2)]
        public int? AccountId { get; set; }

        //[Key(3)]
        public List<string> Scope { get; set; }
    }
}
