using ProgressTracker.Models;

namespace ProgressTracker.Tools
{
    public static class TaskModelExtensions
    {
        /// <summary>
        /// Method fills existing set, or creates new set using hierarchy to fill it with all dependencies inline.
        /// <para>Practically method is technical to calculate completion percent
        /// </para>
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static HashSet<TaskModel> FillSet(this TaskModel model, HashSet<TaskModel> set = null)
        {
            if (set == null)
                set = new HashSet<TaskModel>(); // create new if this is first call in hierarchy

            set.Add(model);
            if (model.Dependencies != null)
            {
                foreach (var item in model.Dependencies)
                {
                    item.FillSet(set);
                }
            }

            return set;
        }
    
        public static int GetStepsCount(this TaskModel model)
        {
            return model.FillSet().Count;
        }

        public static int GetStepsCompleteCount(this TaskModel model)
        {
            return model.FillSet().Where(x => x.IsCompleteItself).Count();
        }

        public static int GetStepsIncompleteCount(this TaskModel model)
        {
            return model.FillSet().Where(x => !x.IsCompleteItself).Count();
        }
    }
}
