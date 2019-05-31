using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_MemberDesign.xaml
    /// </summary>
    public partial class UC_Joints : UserControl
    {        
        CModel_PFD Model;
        
        public UC_Joints(CModel_PFD model)
        {
            InitializeComponent();
                        
            Model = model;
            
            CJointsVM vm = new CJointsVM();
            vm.PropertyChanged += HandleJointsPropertyChangedEvent;
            this.DataContext = vm;
            
        }
        protected void HandleJointsPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CJointsVM vm = sender as CJointsVM;
            if (vm != null && vm.IsSetFromCode) return;

            if (e.PropertyName == "JointTypeIndex") SetDynamicTabs(vm);



        }

        private void SetDynamicTabs(CJointsVM vm)
        {
            List<TabItem> tabItems = new List<TabItem>();
            TabItem t1 = new TabItem();
            t1.Header = "T1 header";

            TabItem t2 = new TabItem();
            t2.Header = "T2 header";

            TabItem t3 = new TabItem();
            t3.Header = "T3 header";
            if(vm.JointTypeIndex >= 0) tabItems.Add(t1);
            if (vm.JointTypeIndex >= 1) tabItems.Add(t2);
            if (vm.JointTypeIndex >= 2) tabItems.Add(t3);
            vm.TabItems = tabItems;
        }

        
    }
}
