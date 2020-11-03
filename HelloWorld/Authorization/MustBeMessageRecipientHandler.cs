using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorldWebAPI.Authorization
{
    public class MustBeMessageRecipientHandler : AuthorizationHandler<MustBeMessageRecipientRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MustBeMessageRecipientHandler(IHttpContextAccessor httpContextAccessor)
        {

            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MustBeMessageRecipientRequirement requirement)
        {

            if (context.User.IsInRole("Admin")) //admin can view any message
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var requestorId = context.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            if (!int.TryParse(requestorId, out int requestorIdAsInt))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var recipientId = _httpContextAccessor.HttpContext.GetRouteValue("id").ToString();
            if (!int.TryParse(recipientId, out int recipientIdAsInt))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (recipientIdAsInt != requestorIdAsInt) //requestor should be requesting own messages
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // all checks out
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
