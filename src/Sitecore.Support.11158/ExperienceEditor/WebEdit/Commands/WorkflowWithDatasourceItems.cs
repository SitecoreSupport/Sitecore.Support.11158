using Sitecore.Data.Items;
using Sitecore.ExperienceEditor.Utils;
using Sitecore.Workflows.Simple;
using System;
using System.Collections.Generic;

namespace Sitecore.Support.ExperienceEditor.WebEdit.Commands
{
  [Serializable]
  public class WorkflowWithDatasourceItems : Sitecore.Shell.Framework.Commands.Workflow
  {
    [UsedImplicitly]
    protected new void WorkflowCompleteCallback(WorkflowPipelineArgs args)
    {
      base.WorkflowCompleteCallback(args);
      List<Item> definedDatasources = ItemUtility.GetItemsFromLayoutDefinedDatasources(args.DataItem, Context.Device, args.DataItem.Language);
      definedDatasources.AddRange((IEnumerable<Item>)ItemUtility.GetPersonalizationRulesItems(args.DataItem, Context.Device, args.DataItem.Language));
      definedDatasources.AddRange((IEnumerable<Item>)ItemUtility.GetTestItems(args.DataItem, Context.Device, args.DataItem.Language));
      foreach (Item filterSameItem in ItemUtility.FilterSameItems(definedDatasources))
      {
        if (filterSameItem.Access.CanWrite() && (!filterSameItem.Locking.IsLocked() || filterSameItem.Locking.HasLock()))
          WorkflowUtility.ExecuteWorkflowCommandIfAvailable(filterSameItem, args.CommandItem, args.CommentFields);

        #region FIX for SXA "Composites" components
        if (filterSameItem.Children.Count > 0
        && filterSameItem.TemplateID.ToString().Equals("{705CC8B3-BDE5-4CB3-BF1C-E455A2A36EF1}") // /sitecore/templates/Feature/Experience Accelerator/Composites/Datasource/Accordion/Accordion 
        || filterSameItem.TemplateID.ToString().Equals("{ADD22F05-6B4C-4344-95AD-9A1A9BA6A216}") //	/sitecore/templates/Feature/Experience Accelerator/Composites/Datasource/Carousel/Carousel 
        || filterSameItem.TemplateID.ToString().Equals("{5EF07850-4C04-4B83-9DAF-EFD752CFABA8}") //	/sitecore/templates/Feature/Experience Accelerator/Composites/Datasource/Flip/Flip
        || filterSameItem.TemplateID.ToString().Equals("{F26A9560-CCF1-48C5-9542-800E837CAF7A}")) // /sitecore/templates/Feature/Experience Accelerator/Composites/Datasource/Tabs/Tabs 
        {
          foreach (Item child in filterSameItem.Children)
          {
            if (child.Access.CanWrite() && (!child.Locking.IsLocked() || child.Locking.HasLock()))
              WorkflowUtility.ExecuteWorkflowCommandIfAvailable(child, args.CommandItem, args.CommentFields);
          }
        }
        #endregion
      }
    }
  }
}
