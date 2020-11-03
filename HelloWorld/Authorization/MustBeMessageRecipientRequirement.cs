using Microsoft.AspNetCore.Authorization;

namespace HelloWorldWebAPI.Authorization
{
    public class MustBeMessageRecipientRequirement : IAuthorizationRequirement
    {
        public MustBeMessageRecipientRequirement()
        {
        }
    }
}
