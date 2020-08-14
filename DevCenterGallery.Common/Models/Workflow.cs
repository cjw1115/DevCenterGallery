using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCenterGallary.Common.Models
{
    public enum WorkflowState
    {
        WorkflowQueued,
        GeneratePreinstallPackageInProgress,
        GeneratePreinstallPackageComplete,
        GeneratePreinstallPackageFailed
    }

    public class Workflow
    {
        public string WorkflowJobType { get; set; }
        public WorkflowState WorkflowState { get; set; }
    }
}
