using Microsoft.AspNetCore.Identity;

namespace ProgressTracker.Models
{
    public class UserReference
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        #region Methods

        public IdentityUser GetUserObject(UserManager<IdentityUser> userManager)
        {
            var task = userManager.FindByIdAsync(UserId);
            task.RunSynchronously();
            return task.Result;
        }

        #endregion
    }
}
