﻿using System;
using System.IO;


// TextDatei c_textdatei = new TextDatei();

//Einlesen einer Datei
// string fs = c_textdatei.ReadFile(@"c:\text.txt");

//Schreiben einer Datei
// c_textdatei.WriteFile(@"C:\text.txt", "Hallo Welt!");

//Text an eine Datei Anhängen
// c_textdatei.Append(@"C:\text.txt", "Hallo Welt!");

//Zeilen einer Datei ersetzen
// c_textdatei.WriteLine(@"C:\text.txt", 2, "neuer Text für diese Zeile", true);

//Zeilen einer Datei ersetzen
// c_textdatei.WriteLine(@"C:\text.txt", 2, "eine eingefügte Zeile", false);

namespace HWB.NETSCALE.FRONTEND.WPF
{
    public class TextDatei
    {
        ///<summary>
        /// Liefert den Inhalt der Datei zurück.
        ///</summary>
        ///<param name="sFilename">Dateipfad</param>
        public string ReadFile(String sFilename)
        {
            string sContent = "";

            if (File.Exists(sFilename))
            {
                StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                sContent = myFile.ReadToEnd();
                myFile.Close();
            }
            return sContent;
        }

        ///<summary>
        /// Schreibt den übergebenen Inhalt in eine Textdatei.
        ///</summary>
        ///<param name="sFilename">Pfad zur Datei</param>
        ///<param name="sLines">zu schreibender Text</param>
        public void WriteFile(String sFilename, String sLines)
        {
            StreamWriter myFile = new StreamWriter(sFilename);
            myFile.Write(sLines);
            myFile.Close();
        }

        ///<summary>
        /// Fügt den übergebenen Text an das Ende einer Textdatei an.
        ///</summary>
        ///<param name="sFilename">Pfad zur Datei</param>
        ///<param name="sLines">anzufügender Text</param>
        public void Append(string sFilename, string sLines)
        {
            StreamWriter myFile = new StreamWriter(sFilename, true);
            myFile.Write(sLines);
            myFile.Close();
        }

        ///<summary>
        /// Liefert den Inhalt der übergebenen Zeilennummer zurück.
        ///</summary>
        ///<param name="sFilename">Pfad zur Datei</param>
        ///<param name="iLine">Zeilennummer</param>
        public string ReadLine(String sFilename, int iLine)
        {
            string sContent = "";
            float fRow = 0;
            if (File.Exists(sFilename))
            {
                StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                while (!myFile.EndOfStream && fRow < iLine)
                {
                    fRow++;
                    sContent = myFile.ReadLine();
                }
                myFile.Close();
                if (fRow < iLine)
                    sContent = "";
            }
            return sContent;
        }

        /// <summary>
        /// Schreibt den übergebenen Text in eine definierte Zeile.
        ///</summary>
        ///<param name="sFilename">Pfad zur Datei</param>
        ///<param name="iLine">Zeilennummer</param>
        ///<param name="sLines">Text für die übergebene Zeile</param>
        ///<param name="bReplace">Text in dieser Zeile überschreiben (t) oder einfügen (f)</param>
        public void WriteLine(String sFilename, int iLine, string sLines, bool bReplace)
        {
            string sContent = "";
            string[] delimiterstring = {"\r\n"};

            if (File.Exists(sFilename))
            {
                StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                sContent = myFile.ReadToEnd();
                myFile.Close();
            }

            string[] sCols = sContent.Split(delimiterstring, StringSplitOptions.None);

            if (sCols.Length >= iLine)
            {
                if (!bReplace)
                    sCols[iLine - 1] = sLines + "\r\n" + sCols[iLine - 1];
                else
                    sCols[iLine - 1] = sLines;

                sContent = "";
                for (int x = 0; x < sCols.Length - 1; x++)
                {
                    sContent += sCols[x] + "\r\n";
                }
                sContent += sCols[sCols.Length - 1];
            }
            else
            {
                for (int x = 0; x < iLine - sCols.Length; x++)
                    sContent += "\r\n";

                sContent += sLines;
            }


            StreamWriter mySaveFile = new StreamWriter(sFilename);
            mySaveFile.Write(sContent);
            mySaveFile.Close();
        }
    }
}