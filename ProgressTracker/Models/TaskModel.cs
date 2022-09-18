using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProgressTracker.Tools;

namespace ProgressTracker.Models
{
    public class TaskModel
    {
        [Key]
        public int Id { get; set; }

        #region Props
        public ICollection<TaskModel>? Dependencies { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserReference? Creator { get; set; }
        public UserReference? Target { get; set; }
        public UserReference? Finisher { get; set; }

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
        public bool IsCompleteItself
        {
            get => Finisher != null; // if finisher exists - someone finished task
        } // One task itself,without nesting

        /// <summary>
        ///  is the task fully complete including all dependencies
        /// </summary>
        [NotMapped]
        public bool IsComplete
        {
            get => this.GetStepsCount() == this.GetStepsCompleteCount();
        }
        #endregion

        /// <summary>
        /// Method override uses database id as hashing parameter
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}