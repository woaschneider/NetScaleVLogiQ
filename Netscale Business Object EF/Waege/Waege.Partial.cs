using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Reflection;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;
using HWB.NETSCALE.GLOBAL;
using combit.ListLabel17.DataProviders;


namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Summary description for Waege.
    /// </summary>
    public partial class Waege
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool exr;

        public bool ExRechner
        {
            get { return exr; }
            set
            {
                exr = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged(Entity.BonitaetKz);
            }
        }

        protected void OnPropertyChanged(string BonitaetKz)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(BonitaetKz));
            }
        }

        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        private IQueryable<WaegeEntity> LSquery;

        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
            // OK - Ich bin mir hier nicht sicher ob das einfacher geht: So baue ich mir im Moment meinen PresaveHook
            StateChange += new mmBusinessStateChangeDelegate(this.StateChangeHandler);
        }

        public override void StateChangeHandler(mmBaseBusinessObject bizObj, mmBusinessStateChangeEventArgs e)
        {
            if (this.State == mmBusinessState.PreSaving)
            {
                PreSaveHook(this.Entity);
            }

            if (this.State == mmBusinessState.Added)
            {
                OnNew(Entity);
            }

            if (this.State == mmBusinessState.Saved)
            {
            }
        }

        private void OnNew(WaegeEntity boWE)
        {
            Wiegeart boWA = new Wiegeart();
            WiegeartEntity boWAE = boWA.GetDefaultWiegeart();
            if (boWAE != null)
            {
                Entity.WiegeartKz = boWAE.Kennung;
                Entity.WiegeartBezeichnung = boWAE.Bezeichnung;
            }

            Incoterm boI = new Incoterm();
            IncotermEntity boIE = boI.GetDefaultIncoterm();
            if (boIE != null)
            {
                Entity.IncotermKz = boIE.Kennung;
                Entity.IncotermBezeichnung = boIE.Bezeichnung;
            }
        }


        public void WriteToMischer(WaegeEntity oWE)
        {
            Lokaleeinstellungen oLE = new Lokaleeinstellungen();

            oLE = oLE.Load();
            if (oLE.MISCHEREXPORT == "J")
            {
                string exportpath = oLE.MISCHEREXPORT_PATH + '\\';
                Einstellungen oE = new Einstellungen();
                EinstellungenEntity oEE = oE.GetEinstellungen();

                string Exportstring = VFP.PadL(oEE.MischerCounterId.ToString(), 7, '0');
                Exportstring = Exportstring + VFP.PadR(oWE.Kfz1, 15, ' ') + VFP.PadR(oWE.FirmaKU, 40, ' ') +
                               VFP.PadR(oWE.wefirma, 40, ' ') + VFP.PadR(oWE.wename1, 40, ' ') +
                               VFP.PadL((Convert.ToInt16(oWE.Sollwertmischer*1000)).ToString("000000"), 6, ' ') +
                               VFP.PadL(oWE.SortenNr, 6, ' ');

                string FileName = exportpath + VFP.PadL(oEE.MischerCounterId.ToString(), 7, '0') + ".txt";
                VFP.StrToFile(Exportstring, FileName);
                oEE.MischerCounterId = oEE.MischerCounterId + 1;
                oE.SaveEntity(oEE);
            }
        }

        public void CreateKfz()
        {
            if (goApp.AutoKfz)
            {
                if (Entity.Kfz1 == null)
                    return;
                var oCf = new CF();

                if (oCf.IsCfNew(Entity.Kfz1) && Entity.Waegung == 1)
                {
                    CFEntity oCFE = oCf.NewEntity();
                    oCFE.Kfz1 = Entity.Kfz1;
                    oCFE.NrSP = Entity.NrSP;
                    oCFE.FirmaSP = Entity.FirmaSP;
                    oCFE.NrFU = Entity.NrFU;
                    oCFE.FirmaFU = Entity.FirmaFU;
                    if (Entity.WiegeartKz == "A")
                        SaveTara2CF(oCFE);

                    Entity.KfzID = oCFE.KfzID; // Neu 28.05.2013
                    oCf.SaveEntity(oCFE);
                }
                if (oCf.IsCfNew(Entity.Kfz1) == false && Entity.Waegung == 1 && Entity.WiegeartKz == "A")
                {
                    CFEntity oCFE = oCf.GetCFByKennzeichen(Entity.Kfz1);

                    if (oCFE != null)
                    {
                        SaveTara2CF(oCFE);
                        oCf.SaveEntity(oCFE);
                    }
                }
            }
        }

        private void SaveTara2CF(CFEntity oCFE)
        {
            if (goApp.SAVE_ERST2CFTARA)
            {
                oCFE.Tara = Entity.ErstGewicht;
                oCFE.Taradatum = DateTime.Today;
            }
            if (Entity.Abrufid != null)
                oCFE.abruf_PK = Entity.Abrufid;

            if (Entity.Abrufnr != null)
                oCFE.Abrufnr = Entity.Abrufnr;
        }

        private string ConvertKfzToKfzRaw(string kfz)
        {
            if (kfz != null)
            {
                string a = kfz.Replace("-", "");
                return a.Replace(" ", "");
            }
            else
            {
                return "";
            }
        }

        public void PreSaveHook(WaegeEntity boWE)
        {
            if (goApp.AutoAbruf == true)
                CheckAndCreateAbr(boWE);

            this.Entity.Kfz1Raw = ConvertKfzToKfzRaw(this.Entity.Kfz1);

            if (goApp.SAVE_ABR2CF == true)
                WriteAbrufNr2Cf(this.Entity);
            if (this.Entity.Waegung == 2 | Entity.Waegung == 4 | Entity.Waegung == 8)
                // Zweitwägung/LS bearbeiten/LS neu anlegen
            {
                Entity.Username = goApp.username;
                if (Entity.AbrechnungsKZ != "LO")
                {
                    Entity.AbrechnungsKZ = "RG";
                }
                //   Entity.Werksnr = 
                if (Entity.BonitaetKz == "7")
                    CalcRechnung();
            }

            // Mandant PK eintragen
            Entity.PK_Mandant = goApp.Mandant_PK;
            // Kfz anlegen wenn unbekannt
            CreateKfz();
        }

        public void CalcRechnung()
        {
            if (Entity == null)
                return;
            Einstellungen oM = new Einstellungen();
            EinstellungenEntity oME = oM.GetEinstellungen();
            Entity.USt = oME.Ust;
            if (Entity.preisvk != null)
            {
                if (Entity.preisvk > 0)
                {
                    float? preis = 0;
                    float? fracht = 0;
                    float? skontoproz = 0;

                    float? ust = 0;

                    float? nettogewicht = 0;

                    if (Entity.preisvk != null)
                        preis = (float) Entity.preisvk;
                    if (Entity.Frachtpreis != null)
                        fracht = (float) Entity.Frachtpreis;
                    if (Entity.Skontoproz != null)
                        skontoproz = (float) Entity.Skontoproz;
                    if (Entity.Nettogewicht != null)
                        nettogewicht = (float) Entity.Nettogewicht;
                    if (Entity.USt != null)
                        ust = (float) Entity.USt;


                    var netto = ((preis*nettogewicht) + fracht);
                    var endb = netto*(1 + (ust/100));
                    endb = endb - ((endb/100)*skontoproz);
                    // 27.01.2014 Skontobetrag in Feld   
                    var skontob = (endb/100)*skontoproz;
                    float roundedSkontobetrag = (float) (Math.Round((double) skontob, 2, MidpointRounding.AwayFromZero));
                    Entity.Skontobetrag = (decimal) roundedSkontobetrag;
                    // 

                    var ustbetrag = (netto/100)*ust;

                    float roundedUstbetrag = (float) (Math.Round((double) ustbetrag, 2, MidpointRounding.AwayFromZero));
                    float roundedNetto = (float) (Math.Round((double) netto, 2, MidpointRounding.AwayFromZero));
                    float roundedEnd = (float) (Math.Round((double) endb, 2, MidpointRounding.AwayFromZero));
                    Entity.Nettobetrag = (decimal) roundedNetto;
                    Entity.Endbetrag = (decimal) roundedEnd;
                    Entity.Ustbetrag = (decimal) roundedUstbetrag;
                }
            }
        }

        public void CalcRechnung(float gepolltesZweitGewicht)
        {
            Einstellungen oM = new Einstellungen();
            EinstellungenEntity oME = oM.GetEinstellungen();
            Entity.USt = oME.Ust;
            if (Entity.preisvk != null)
            {
                if (Entity.preisvk > 0)
                {
                    float? preis = 0;
                    float? fracht = 0;
                    float? skontoproz = 0;
                    float? ust = 0;
                    float? nettogewicht = 0;

                    if (Entity.preisvk != null)
                        preis = (float) Entity.preisvk;
                    if (Entity.Frachtpreis != null)
                        fracht = (float) Entity.Frachtpreis;
                    if (Entity.Skontoproz != null)
                        skontoproz = (float) Entity.Skontoproz;
                    if (Entity.Nettogewicht != null)
                        nettogewicht = (float) gepolltesZweitGewicht;
                    if (Entity.USt != null)
                        ust = (float) Entity.USt;

                    var netto = ((preis*nettogewicht) + fracht);
                    var endb = netto*(1 + (ust/100));
                    endb = endb - ((endb/100)*skontoproz);
                    var ustbetrag = netto*skontoproz;
                    float roundedUstbetrag = (float) (Math.Round((double) ustbetrag, 2, MidpointRounding.AwayFromZero));
                    float roundedNetto = (float) (Math.Round((double) netto, 2, MidpointRounding.AwayFromZero));
                    float roundedEnd = (float) (Math.Round((double) endb, 2, MidpointRounding.AwayFromZero));
                    Entity.Nettobetrag = (decimal) roundedNetto;
                    Entity.Endbetrag = (decimal) roundedEnd;
                    Entity.Ustbetrag = (decimal) roundedUstbetrag;
                }
            }
        }

        private void CheckAndCreateAbr(WaegeEntity boWE)
        {
            Abruf boA = new Abruf();
            if (boWE.Abrufnr == null)
            {
                if ((this.Entity.NrKU == string.Empty | this.Entity.NrKU == null) &&
                    (this.Entity.FirmaKU == string.Empty | this.Entity.FirmaKU == null) &&
                    (this.Entity.SortenNr == string.Empty | this.Entity.SortenNr == null) &&
                    (this.Entity.Sortenbezeichnung1 == string.Empty | this.Entity.Sortenbezeichnung1 == null))
                {
                }
                else
                {
                    AbrufEntity oAE = boA.CreateAbrufautomatically(boWE);
                    this.Entity.Abrufnr = oAE.Abrufnr;
                    this.Entity.Abrufid = oAE.PK;
                }
            }
            else // Dann müssen wir noch prüfen ob was am Abruf geändert worden ist
            {
                // Kunde / Lieferant
                // Sorte
                // Baustelle vielleicht
                bool uRet = boA.HasAbrufChanged(this.Entity);

                // Gibt es einen Abruf mit solchen Daten
                if (uRet == true)
                {
                    AbrufEntity oAe = boA.CompareAbrufData(this.Entity);
                    if (oAe != null)
                        // Ja, es gibt einen. Vielleicht passt aber die Abrufnr durch Usereingriff nicht mehr
                    {
                        // Also schreiben wir die Abrufnr wieder rein
                        Entity.Abrufnr = oAe.Abrufnr;
                        Entity.Abrufid = oAe.PK;
                    }
                    else
                    {
                        AbrufEntity oAE = boA.CreateAbrufautomatically(boWE);
                        this.Entity.Abrufnr = oAE.Abrufnr;
                        this.Entity.Abrufid = oAE.PK;
                        this.Entity.Restmenge = 0;
                        this.Entity.Sollmenge = 0;
                        this.Entity.Istmenge = 0;
                    }
                }
            }
        }


        public int IsKfzErstVerwogen(string kf)
        {
            string k = ConvertKfzToKfzRaw(kf);

            IQueryable<WaegeEntity> query = from w in this.ObjectContext.WaegeEntities
                                            where w.Kfz1Raw == k && w.Waegung == 1
                                            select w;
            WaegeEntity cWe = this.GetEntity(query);
            if (cWe == null)
                return 0;
            return cWe.PK;
        }

        public mmBindingList<WaegeEntity> GetHofListe()
        {
            IQueryable<WaegeEntity> query = from Waege in this.ObjectContext.WaegeEntities
                                            where Waege.Waegung == 1
                                            select Waege;
            return this.GetEntityList(query);
        }

        // *****************************************************************************************************


        public mmBindingList<WaegeEntity> GetLSListe(DateTime vonDatum, DateTime bisDatum, string FeldName1,
                                                     string MatchCode1, string FeldName2, string MatchCode2,
                                                     string FeldName3, string MatchCode3)
        {
            // Filter-Var
            string kunde = "";
            string kfz = "";
            string baustelle1 = "";
            string sortennr = "";
            string sortenbezeichnung = "";

            if (FeldName1 == "Kunde / Lieferant")
                kunde = MatchCode1;
            if (FeldName1 == "Kfz")
                kfz = MatchCode1;
            if (FeldName1 == "Baustellenbezeichnung 1")
                baustelle1 = MatchCode1;
            if (FeldName1 == "Sorten-Nr.")
                sortennr = MatchCode1;
            if (FeldName1 == "Sortenbezeichnung 1")
                sortenbezeichnung = FeldName1;


            if (FeldName2 == "Kunde / Lieferant")
                kunde = MatchCode2;
            if (FeldName2 == "Kfz")
                kfz = MatchCode2;
            if (FeldName2 == "Baustellenbezeichnung 1")
                baustelle1 = MatchCode2;
            if (FeldName2 == "Sorten-Nr.")
                sortennr = MatchCode2;
            if (FeldName2 == "Sortenbezeichnung 1")
                sortenbezeichnung = FeldName2;

            if (FeldName3 == "Kunde / Lieferant")
                kunde = MatchCode3;
            if (FeldName3 == "Kfz")
                kfz = MatchCode3;
            if (FeldName3 == "Baustellenbezeichnung 1")
                baustelle1 = MatchCode3;
            if (FeldName3 == "Sorten-Nr.")
                sortennr = MatchCode3;
            if (FeldName3 == "Sortenbezeichnung 1")
                sortenbezeichnung = FeldName3;


            LSquery = from w in this.ObjectContext.WaegeEntities
                      where w.Waegung == 2
                            && w.LSDatum >= vonDatum && w.LSDatum <= bisDatum && w.AbrechnungsKZ == "RG"
                            && w.FirmaKU.Contains(kunde)
                            && w.Kfz1.Contains(kfz)
                            && w.wefirma.Contains(baustelle1)
                            && w.SortenNr.Contains(sortennr)
                            && w.Sortenbezeichnung1.Contains(sortenbezeichnung)
                      orderby w.LSNRGlobal descending
                      select w;
            return this.GetEntityList(LSquery);
        }


        //*******************************************************************************************************

        // TAAB ***************************************************
        public mmBindingList<WaegeEntity> Get_taabListe(DateTime Datum)
        {
            LSquery = from w in this.ObjectContext.WaegeEntities
                      where w.Waegung == 2 & w.LSDatum == Datum & w.AbrechnungsKZ == "RG"
                      orderby w.Wiegeart , w.LSNRGlobal ascending
                      select w;

            return this.GetEntityList(LSquery);
        }

        public int Taab(DateTime datum)
        {
            Einstellungen oE = new Einstellungen();
            Lokaleeinstellungen oL = new Lokaleeinstellungen();
            oL = oL.Load();
            string exportpath = oL.EXPORT_PATH + '\\';
            string ex = "";
            int count = 0;


            mmBindingList<WaegeEntity> taab = Get_taabListe(datum);

            string MandantNr = (new Mandant().GetMandantByPK(goApp.Mandant_PK).MandantNr).Substring(0, 2);
            string Werknr = (new Mandant().GetMandantByPK(goApp.Mandant_PK).Werksnr).Substring(0, 3);

            // *************************************************
            string FileName = GetFileName(oE, exportpath, MandantNr, Werknr);

            if (taab != null)
            {
                foreach (WaegeEntity a in taab)
                {
                    if (Convert.ToChar(a.WiegeartKz) == 'A' | Convert.ToChar(a.WiegeartKz) == 'R')
                    {
                        ex = "3";
                        ex = ex + MandantNr; // MandantNr = alte Firma
                        ex = ex + VFP.PadL(Werknr, 6, '0');
                        ex = ex + VFP.PadL(a.NrKU.Trim(), 6, '0');
                        ex = ex + a.kontraktnr.Substring(0, 8); // ??? Das müssen wir noch klären
                        // Abrechnungskennzeichen

                        string boni = a.BonitaetKz;
                        string akz;
                        switch (boni)
                        {
                            case "8":
                                akz = "00";
                                break;
                            case "7":
                                akz = "01";
                                break;
                            default:
                                akz = "00";
                                break;
                        }
                        ex = ex + akz;

                        // LS Datum
                        string lsdatum = a.LSDatum.ToString().Replace(".", "").Replace("/", "").Substring(0, 8);
                        ex = ex + lsdatum;
                        // LS Zeit
                        string lsZeit = a.ZweitZeit.ToString().Substring(11, 8).Replace(":", "");
                        ex = ex + lsZeit;
                        ex = ex + VFP.PadL(a.NrSP == null ? "  " : a.NrSP.Trim(), 2, '0');

                        //  ex = ex + VFP.PadR(a.NrFU.Trim(), 6, '0');
                        ex = ex + VFP.PadL(a.NrFU == null ? "  " : a.NrFU.Trim(), 6, '0');

                        ex = ex + VFP.PadR(a.Kfz1.Trim(), 10, ' ');

                        // Tarifgebiet
                        ex = ex + "0";

                        // Fahrzeug und Wiegeart
                        ex = ex + a.WiegeartKz.Trim() + a.FrachtmittelKz.Trim();
                        //Zusatzleistung und Kilometer
                        ex = ex + "  000";
                        // Material
                        ex = ex + a.SortenNr;

                        // Nettogewicht 
                        ex = ex + VFP.PadL((Convert.ToInt32(a.Nettogewicht*1000)).ToString(), 8, '0');
                        // LSNR 
                        ex = ex + VFP.PadL(a.lsnr.Trim(), 6, '0');
                        // PReisebrechnung klären
                        ex = ex + "0000000";
                        int test = a.wefirma.Length;
                        // ****************************************************************
                        if (a.wefirma.Length >= 30)
                            ex = ex + VFP.PadR(a.wefirma.Substring(0, 30).Trim(), 30, ' ');
                        else
                        {
                            ex = ex + VFP.PadR(a.wefirma.Substring(0, a.wefirma.Length).Trim(), 30, ' ');
                        }

                        if (a.wename1.Length >= 30)
                            ex = ex + VFP.PadR(a.wename1.Substring(0, 30).Trim(), 30, ' ');
                        else
                        {
                            ex = ex + VFP.PadR(a.wename1.Substring(0, a.wename1.Length).Trim(), 30, ' ');
                        }
                        // *****************************************************************
                        ex = ex + 'J';
                        ex = ex + VFP.PadL("", 28, ' ');
                        ex = ex + VFP.PadL(a.LSNRGlobal.Trim(), 7, ' ');
                        ex = ex + "00000000000000000000000000000000000000000000000DEM000000********";
                        // Ende *************************
                        count = count + 1;
                        ex = ex + VFP.Chr(13) + VFP.Chr(10);
                        VFP.StrToFile(ex, FileName, true);
                    }
                }
                string ende = "3$$END" + DateTime.Now.ToString().Substring(0, 10).Replace(".", "");
                ende = ende + DateTime.Now.ToString().Substring(11, 8).Replace(":", "") +
                       VFP.PadL(count.ToString(), 5, ' ');
                VFP.StrToFile(ende, FileName, true);
            }

            return count;
        }

        private string GetFileName(Einstellungen oE, string exportpath, string MandantNr, string Werknr)
        {
            int? FileNameCounter = GetFileNameCounter(oE);
            //**************************************************
            string FileName = exportpath + "L" + MandantNr + Werknr + "." + VFP.PadL(FileNameCounter.ToString(), 3, '0');
            if (File.Exists(FileName))
                File.Delete(FileName);
            return FileName;
        }

        private int? GetFileNameCounter(Einstellungen oE)
        {
            int? FileNameCounter;
            EinstellungenEntity oEE = oE.GetEinstellungen();
            FileNameCounter = oEE.TaabCounterId;
            oEE.TaabCounterId = oEE.TaabCounterId + 1;
            oE.SaveEntity(oEE);
            return FileNameCounter;
        }

        //*********************************************************
        public WaegeEntity GetWaegungByID(int pWaege_id)
        {
            IQueryable<WaegeEntity> query = from Waege in this.ObjectContext.WaegeEntities
                                            where Waege.PK == pWaege_id
                                            select Waege;
            return this.GetEntity(query);
        }

        public ObjectDataProvider GetWaegungODPByID(int pWaege_id)
        {
            IQueryable<WaegeEntity> query = from Waege in this.ObjectContext.WaegeEntities
                                            where Waege.PK == pWaege_id
                                            select Waege;
            ObjectDataProvider oDP = new ObjectDataProvider(query, 2);
            return oDP;
        }

        public ObjectDataProvider GetLSByLSNRGlobal(string LSNRGlobal)
        {
            IQueryable<WaegeEntity> query = from Waege in this.ObjectContext.WaegeEntities
                                            where Waege.LSNRGlobal == LSNRGlobal
                                            select Waege;
            ObjectDataProvider oDP = new ObjectDataProvider(query, 2);
            return oDP;
        }

        public void CalcAbrufMengen(WaegeEntity oWE)
        {
            if (oWE.Abrufnr != null && oWE.Waegung == 2)
            {
                Abruf boA = new Abruf();
                AbrufEntity boAE = boA.GetAbrufByNr(oWE.Abrufnr);
                if (boAE != null)
                {
                    boAE.Istmenge = boAE.Istmenge + oWE.Nettogewicht;
                    boAE.Restmenge = boAE.Restmenge - oWE.Nettogewicht;
                    boA.SaveEntity(boAE);
                }
            }
            return;
        }

        public void WriteAbrufNr2Cf(WaegeEntity oWE)
        {
            if (oWE.Abrufnr != null && oWE.Waegung == 2)
            {
                CF oCF = new CF();
                CFEntity oCFE = oCF.GetCFByKennzeichen(oWE.Kfz1);
                if (oCFE != null)
                {
                    oCFE.Abrufnr = oWE.Abrufnr;
                    // TODO Neues Feld Abruf PK in Waegetabelle
                    oCFE.abruf_PK = oWE.Abrufid;
                    oCFE.abrufdate = DateTime.Today;
                    mmSaveDataResult mmSaveDataResult = oCF.SaveEntity(oCFE);
                }
            }
        }

        public bool FillApKu(int pk, WaegeEntity oWE)
        {
            AP boAP = new AP();
            APEntity boAPE = boAP.GetAPById(pk);
            if (oWE != null)
            {
                if (boAPE != null)
                {
                    oWE.NrKU = boAPE.Nr;
                    oWE.FirmaKU = boAPE.Firma;
                    oWE.Name1KU = boAPE.Name1;
                    oWE.AnschriftKU = boAPE.Anschrift;
                    oWE.PlzKU = boAPE.Plz;
                    oWE.OrtKU = boAPE.Ort;
                    oWE.BonitaetKz = boAPE.bonitaet;

                    if (boAPE.bonitaet != null)
                    {
                        Bonitaet oB = new Bonitaet();
                        oWE.BonitaetBezeichnung = oB.GetBonitaetByKz(boAPE.bonitaet).Bezeichnung;
                    }
                    return true;
                }
                else
                {
                    oWE.NrKU = "";
                    oWE.FirmaKU = "";
                    oWE.Name1KU = "";
                    oWE.AnschriftKU = "";
                    oWE.PlzKU = "";
                    oWE.PlzKU = "";
                    oWE.BonitaetKz = "";
                    oWE.BonitaetBezeichnung = "";
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool FillIncoterm(string kz)
        {
            Incoterm boI = new Incoterm();
            IncotermEntity boIE = boI.GetIncotermByKz(kz);

            if (Entity != null)
            {
                if (boIE != null)
                {
                    Entity.IncotermKz = boIE.Kennung;
                    Entity.IncotermBezeichnung = boIE.Bezeichnung;
                    return true;
                }
                else
                {
                    Entity.IncotermKz = "";
                    Entity.IncotermBezeichnung = "";
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool FillWiegeart(WaegeEntity oWE)
        {
            var bo = new Wiegeart();
            var boE = bo.GetWiegeartByKz(oWE.WiegeartKz);
            if (oWE != null)
            {
                if (boE != null)
                {
                    oWE.WiegeartKz = boE.Kennung;
                    oWE.WiegeartBezeichnung = boE.Bezeichnung;

                    return true;
                }
                else
                {
                    oWE.WiegeartKz = "";
                    oWE.WiegeartBezeichnung = "";
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void FillMG(int? mgpk, WaegeEntity boWE)
        {
            MG boM = new MG();
            MGEntity boME = boM.GetMGById(mgpk);
            if (boWE != null)
            {
                if (boME != null)
                {
                    boWE.PK_MG = boME.PK;
                    boWE.SortenNr = boME.SortenNr;
                    boWE.Sortenbezeichnung1 = boME.Sortenbezeichnung1;
                    boWE.Sortenbezeichnung2 = boME.Sortenbezeichnung2;
                }
                else
                {
                    boWE.PK_MG = null;
                    boWE.SortenNr = "";
                    boWE.Sortenbezeichnung1 = "";
                    boWE.Sortenbezeichnung2 = "";

                }
            }
            else
            {
                return;
            }
        }

        public void Auftrag2Waege(int KKPK, WaegeEntity oWE)
        {
            KK oK = new KK();
            KKEntity oKKE = oK.GetKKByPK(KKPK);
            if (oKKE != null)
            {
                oWE.kontraktnr = oKKE.kontraktnr;
                oWE.wefirma = oKKE.wefirma;
                oWE.wename1 = oKKE.wename1;

                this.FillApKu((int) oKKE.APFK, oWE);
            }
        }
        public void AuftragDetail2Waege(int KMPK, WaegeEntity oWE)
        {
            KM oKM = new KM();
            KMEntity oKME = oKM.GetKMByPK(KMPK);

            KK oK = new KK();
            int? kkpk = 0;
            if (oKME.kkpk != null)
                kkpk = oKME.kkpk;

            KKEntity oKKE = oK.GetKKByPK(kkpk);
            if (oKKE != null)
            {
                oWE.kontraktnr = oKKE.kontraktnr;
                oWE.wefirma = oKKE.wefirma;
                oWE.wename1 = oKKE.wename1;

                this.FillApKu((int)oKKE.APFK, oWE);
                this.FillMG(oKME.mgpk, oWE);
            }

        }
    }
}