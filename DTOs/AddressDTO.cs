using System.Runtime.Serialization;

namespace DTOs
{
    [DataContract]
    public class AddressDTO
    {
        [DataMember(Name = "StreetAddress")] public string StreetAddress;
        [DataMember(Name = "Locality")] public string Locality;
        [DataMember(Name = "PostalCode")] public int PostalCode;
        [DataMember(Name = "Country")] public string Country;

    }
}
