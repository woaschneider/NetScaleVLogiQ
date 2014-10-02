using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Windows.Controls;
using Newtonsoft.Json;


namespace HWB.NETSCALE.FRONTEND.WPF.Import
{
    public class ImportAddress

    {
        public bool Import(string FullQualifiedFileName)
        {
            try
            {
                StreamReader re = new StreamReader(FullQualifiedFileName);
                JsonTextReader reader = new JsonTextReader(re);
                JsonSerializer se = new JsonSerializer();
              //  object parsedData = se.Deserialize(reader);
               
                while(reader.Read())
                {
                    Console.WriteLine(reader.TokenType+reader.LineNumber);
                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                }


            }
            catch (Exception e)
            {
                
                MessageBox.Show(e.Message.ToString());
            }
         

            return true;
        }


        private void ReadJsonObject()
        {
        }

        private void WriteToEntity()
        {
        }
    }


    public class Country
    {
        public int id { get; set; }
        public string isoCode { get; set; }
    }

    public class Address2
    {
        public string name { get; set; }
        public string street { get; set; }
        public string zipCode { get; set; }
        public string city { get; set; }
        public Country country { get; set; }
    }

    public class AddressableEntity
    {
        public int id { get; set; }
        public string businessIdentifier { get; set; }
        public string name { get; set; }
        public string owningLocationId { get; set; }
        public string subName { get; set; }
        public Address2 address { get; set; }

        
    }

    public class Address
    {
        public IList<AddressableEntity> addressableEntities { get; set; }
    }
}