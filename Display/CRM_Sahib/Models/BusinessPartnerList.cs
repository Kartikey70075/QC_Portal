using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM_Sahib.Models
{
    public class BusinessPartnerList
    {
        public int Series { get; set; }
        public string CustomerCode { get; set; }
        public char Type { get; set; }
        public string CustomerName { get; set; }
        public int Group { get; set; }
        public string Currency { get; set; }
        public string PhoneNum { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Password { get; set; }
        public List<ContactPersonList> ContactPerson { get; set; }
        public List<BillToPayTolist> BillToAdd { get; set; }
        public List<ShipToShipFromlist> ShipToAdd { get; set; }
    }

    public class ContactPersonList
    {
        public string ContactID { get; set; }
        public string gender { get; set; }
        public string Tittle { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }


    }
    public class BillToPayTolist
    {
        public string AddressID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string StreetPoBox { get; set; }
        public string Block { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string StreetNo { get; set; }
        public string Building_Floor_Room { get; set; }


    }
    public class ShipToShipFromlist
    {
        public string AddressID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string StreetPoBox { get; set; }
        public string Block { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string StreetNo { get; set; }
        public string Building_Floor_Room { get; set; }
    }
}