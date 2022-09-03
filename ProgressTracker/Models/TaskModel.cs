using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProgressTracker.Models
{
    public sealed partial class TaskModel
    {
        [Key]
        public int Id { get; set; }

        #region Props
        public ICollection<TaskModel>? Dependencies { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public IdentityUser? Creator { get; set; }
        public IdentityUser? Target { get; set; }
        public IdentityUser? Finisher { get; set; }

        public TaskModel()
        {
            Title = String.Empty;
            Description = String.Empty;
            Dependencies = null;
            CreatedDate = DateTime.Now;
            Creator = null;
            Target = null;
            Finisher = null;
        }

        /// <summary>
        /// Just about task itself, ignoring dependencies
        /// </summary>
        private bool IsCompleteItself
        {
            get => Finisher != null; // if finisher exists - someone finished task
        } // Only private, one task itself,without nesting
        /// <summary>
        ///  is the task fully complete including all dependencies
        /// </summary>
        public bool IsComplete
        {
            get => Steps == StepsComplete;
        }
        #endregion
    }
}
