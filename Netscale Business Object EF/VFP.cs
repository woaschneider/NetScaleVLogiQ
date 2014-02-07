using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HWB
// Eigene Funktionen
{
  public static   class VFP
    { // Die Vfp Inlist Funktion
      public static bool InList(object toExpression, params object[] toItems) { return Array.IndexOf(toItems, toExpression) > -1; }
    
     public static void StrToFile(string cExpression, string cFileName)
     {	//Check if the sepcified file exists	
         if (System.IO.File.Exists(cFileName) == true)	{		//If so then Erase the file first as in this case we are overwriting		
             System.IO.File.Delete(cFileName);	}	//Create the file if it does not exist and open it	
         FileStream oFs = new FileStream(cFileName,FileMode.CreateNew,FileAccess.ReadWrite);	
         //Create a writer for the file	StreamWriter 
         StreamWriter oWriter = new StreamWriter(oFs);
         oWriter = new StreamWriter(oFs);	//Write the contents	
         oWriter.Write(cExpression);	
         oWriter.Flush();	
         oWriter.Close();	
         oFs.Close();
     }

     public static void StrToFile(string cExpression, string cFileName, bool lAdditive)
     {
         FileStream oFs = new FileStream(cFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
         StreamWriter oWriter = new StreamWriter(oFs);
         oWriter.BaseStream.Seek(0, SeekOrigin.End);
         oWriter.Write(cExpression);
         oWriter.Flush();
         oWriter.Close();
         oFs.Close();
     }

     public static string PadL(string cExpression, int nResultSize)
     {
         return cExpression.PadLeft(nResultSize);
     }
      
      public static string PadL(string cExpression, int nResultSize, char cPaddingChar)
      {
          return cExpression.PadLeft(nResultSize, cPaddingChar);
      }

      public static string PadR(string cExpression, int nResultSize)
      {
          return cExpression.PadRight(nResultSize);
      }
       public static string PadR(string cExpression, int nResultSize, char cPaddingChar)
       {
           return cExpression.PadRight(nResultSize, cPaddingChar);
       }

       public static char Chr(int nAnsiCode) { return (char)nAnsiCode; }
  }


}
