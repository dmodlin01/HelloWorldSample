using HelloWorldWeb.Models;

namespace HelloWorldWeb.ViewModels
{
    public class MessageDeliveryInfoVM
    {
        public string FullName { get; set; }
        public AddressVM Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public MessageDeliveryInfoVM(string fullName, AddressVM address, string phone, string email)
        {
            FullName = fullName;
            Address = address;
            Phone = phone;
            Email = email;
        }
    }
}
