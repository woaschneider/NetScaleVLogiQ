using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HWB.NETSCALE.BOEF;
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
                  //  Console.WriteLine(reader.TokenType+reader.LineNumber);
                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                    Address boA = new Address();
                    AddressEntity boAE = boA.NewEntity();

                  //  if(reader.TokenType=="PropertyName" &&reader.value )

                    
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



}