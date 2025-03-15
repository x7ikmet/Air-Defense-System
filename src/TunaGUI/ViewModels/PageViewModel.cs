using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunaGUI.Data;

namespace TunaGUI.ViewModels;

public partial class PageViewModel: ViewModelBase
{
    [ObservableProperty]
    private ApplicationPageNames _pageName;

}
