﻿using System;

namespace REMME.Auth.Client.Contracts.Models
{    
    public class CertificateCreateDto
    {
        public string CommonName { get; set; }
        public string OrganizationName { get; set; }
        public string Email { get; set; }
        public string CountryName { get; set; }
        public string LocalityName { get; set; }
        public string PostalAddress { get; set; }
        public string PostalCode { get; set; }
        public string StreetAddress { get; set; }
        public string StateName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Pseudonym { get; set; }
        public string GenerationQualifier { get; set; }
        public string Title { get; set; }
        public string Serial { get; set; }
        public string BusinessCategory { get; set; }

        public uint ValidityDays { get; set; }
        public uint ValidAfterDays { get; set; }

        public DateTime NotBefore { get => DateTime.UtcNow.AddDays(ValidAfterDays); }
        public DateTime NotAfter { get => NotBefore.AddDays(ValidityDays);}
    }
}
