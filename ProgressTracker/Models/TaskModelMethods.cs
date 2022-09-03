using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressTracker.Models
{
    public sealed partial class TaskModel
    {
        /// <summary>
        /// Amount of tasks including full tree of tasks that the task depends
        /// </summary>
        [NotMapped]
        public int Steps
        {
            get => GetStepsFull();
        }

        /// <summary>
        /// Amount of tasks including full tree of tasks that the task depends, filteres only complete ones
        /// </summary>
        [NotMapped]
        public int StepsComplete
        {
            get => GetStepsComplete();
        }

        /// <summary>
        /// Amount of tasks including full tree of tasks that the task depends, filteres only incomplete ones
        /// </summary>
        [NotMapped]
        public int StepsIncomplete
        {
            get => GetStepsIncomplete();
        }

        /// <summary>
        /// Return ratio of complete tasks, including full tree of dependencies
        /// </summary>
        [NotMapped]
        public double StepsCompleteToFullRatio
        {
            get => (double)StepsComplete / (double)Steps;
        }

        #region Methods
        /// <summary>
        /// Method override uses database id as hashing parameter
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        /// <summary>
        /// Method fills existing set, or creates new set using hierarchy to fill it with all dependencies inline.
        /// <para>Practically method is technical to calculate completion percent
        /// </para>
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public HashSet<TaskModel> FillSet(HashSet<TaskModel> set = null)
        {
            if (set == null)
                set = new HashSet<TaskModel>(); // create new if this is first call in hierarchy

            set.Add(this);
            if (Dependencies != null)
            {
                foreach (var item in Dependencies)
                {
                    item.FillSet(set);
                }
            }
            
            return set;
        }
        /// <summary>
        /// Ignores completeness, includes itself
        /// </summary>
        /// <returns></returns>
        public int GetStepsFull()
        {
            return FillSet().Count;
        }
        /// <summary>
        /// Counts only complete steps
        /// </summary>
        /// <returns></returns>
        public int GetStepsComplete()
        {
            return FillSet().Where(x => x.IsCompleteItself).Count();
        }
        /// <summary>
        /// Counts only incomplete steps
        /// </summary>
        /// <returns></returns>
        public int GetStepsIncomplete()
        {
            return FillSet().Where(x => !x.IsCompleteItself).Count();
        }

        public bool Finish(IdentityUser finisher, bool forceFinish = false)
        {
            // someone finishes task but he is not a targeted finisher
            if (forceFinish)
            {
                Finisher = finisher;
                return true;
            }

            // Test the actual finisher is a targeted one if it's not forcefinish
            if (object.ReferenceEquals(finisher, Target))
            {
                Finisher = finisher;
                return true;
            }

            // else task still not finished
            return false;
        }
        #endregion
    }
}
