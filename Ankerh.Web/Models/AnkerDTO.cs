using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ankerh.Web.Models
{
    public class AnkerData
    {
        public AnkerData()
        {
            this.Items = new List<AnkerDTO>();
        }
        public List<AnkerDTO> Items { get; set; }
        public string ContinutationToken { get; set; }
        public string SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class AnkerDTO
    {
        public string Id { get; set; }
        public string BilagsNummer { get; set; }
        public string BogføringsId { get; set; }
        public string Godkender1 { get; set; }
        public string Godkender2 { get; set; }
        public string FakturaTypeId { get; set; }
        public string FakturaStatusId { get; set; }
        public string FakturaGodkenderStatusId { get; set; }
        public string Kreditornummer { get; set; }
        public string Kreditornavn { get; set; }
        public string KreditornavnAdr1 { get; set; }
        public string KreditornavnAdr2 { get; set; }
        public string KreditornavnAdr3 { get; set; }
        public string Postnummer { get; set; }
        public DateTime Fakturadato { get; set; }
        public string Forfaldsdato { get; set; }
        public string Fakturanummer { get; set; }
        public string FakturaTekst { get; set; }
        public string Sagsnummer { get; set; }
        public string SagsNavn { get; set; }
        public string Afsnit { get; set; }
        public string AfsnitTekst { get; set; }
        public string Aktivitet { get; set; }
        public string AktivitetTekst { get; set; }
        public string beløb { get; set; }
        public string Filnavn { get; set; }
        public string Bemærkning { get; set; }
        public string GodkendtErudskrevet { get; set; }
        public string dtmoprettet { get; set; }
        public string dtmts { get; set; }
        public string OpdateretAf { get; set; }
        public string OpdateringsInfo { get; set; }
        public string FakturaStatusNewId { get; set; }
        public string OpdaterDataFraXalIgen { get; set; }
        public string GodkendelsesTypeLåstAfGodk1 { get; set; }
        public string BeløbUdenMoms { get; set; }
        public string dtmGodkendt1 { get; set; }
        public string dtmGodkendt2 { get; set; }
        public string dtmGodkendtØkonomi { get; set; }
        public string dtmOpdateretVedGenindlæs { get; set; }
        public string FinansKontonr { get; set; }
        public string FinansKontoTekst { get; set; }
        public string FakturaGodkenderInitialer { get; set; }
        public string ArtesaOpdateringsId { get; set; }
    }

    public class AnkerhRequest
    {
        public string Token { get; set; }
        public int PageSize { get; set; } = 20;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string BilagsNummer { get; set; }
        public string Sagsnummer { get; set; }
        public string SagsNavn { get; set; }
        public string AfsnitTekst { get; set; }
        public string AktivitetTekst { get; set; }
        public string Kreditornummer { get; set; }
        public string Kreditornavn { get; set; }
        public string Filnavn { get; set; }
    }
}