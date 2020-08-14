using DevCenterGallary.Common.Models;
using DevCenterGallary.Common.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace DevCenterGallary.ViewModels
{
    public class CustomerGroupsViewModel: ViewModelBase
    {
        private StoreService _storeService = SimpleIoc.Default.GetInstance<StoreService>();

        private string _targetAccount;
        public string TargetAccount
        {
            get => _targetAccount;
            set => Set(ref _targetAccount, value);
        }

        private ObservableCollection<CustomerGroup> _groups;
        public ObservableCollection<CustomerGroup> Groups
        {
            get => _groups;
            set => Set(ref _groups, value);
        }

        private CustomerGroup _selectedGroup;
        public CustomerGroup SelectedGroup
        {
            get => _selectedGroup;
            set => Set(ref _selectedGroup, value);
        }

        private string _allMembers;
        public string AllMembers
        {
            get => _allMembers;
            set => Set(ref _allMembers, value);
        }

        public BusyViewModel SearchingVM { get; set; } = new BusyViewModel();

        private SynchronizationContext _context = SynchronizationContext.Current;

        public CustomerGroupsViewModel()
        {
            Groups = new ObservableCollection<CustomerGroup>();
        }

        
        public async void SearchAccount()
        {
            var account = TargetAccount.Trim();
            if (string.IsNullOrEmpty(account))
                return;
            try
            {
                SearchingVM.IsProcessing = true;
                Groups.Clear();

                var groups = await _storeService.GetGroups();
                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = 10;
                var parallelLoopResult = Parallel.For(0, groups.Count, async (i) =>
                  {
                      var group = groups[i];
                      var groupInfo = await _storeService.GetGroupInfo(group.GroupId);
                      if (groupInfo.Members != null
                          && groupInfo.Members.Count > 0
                          && groupInfo.Members.Contains(account))
                      {
                          _context.Post((o) =>
                          {
                              Groups.Add(groupInfo);
                          }, null);
                      }
                  });

                while (!parallelLoopResult.IsCompleted)
                {
                    await Task.Delay(50);
                }
            }
            catch(Exception e)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                new MessageDialog(e.Message).ShowAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            finally
            {
                SearchingVM.IsProcessing = false;
            }
        }

        public void ShowAllMembers()
        {
            if (SelectedGroup == null)
                return;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Group {SelectedGroup.GroupName} has {SelectedGroup.Members.Count} test account.");
            sb.AppendLine();
            foreach (var item in SelectedGroup.Members.OrderBy(m => m))
            {
                sb.AppendLine(item);
            }
            AllMembers = sb.ToString();
        }
    }
}
