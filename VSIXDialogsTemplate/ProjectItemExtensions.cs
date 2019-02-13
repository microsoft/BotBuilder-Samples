using System;
using System.Collections;
using System.Linq;
using EnvDTE;

namespace VSIXDialogsTemplate
{
    public static class ProjectItemsExtensions
    {
        public static ProjectItem FindProjectItem(
            this IEnumerable instance,
            Func<ProjectItem, bool> predicate)
        {
            return instance.Cast<ProjectItem>().FirstOrDefault(predicate);
        }
    }
}
